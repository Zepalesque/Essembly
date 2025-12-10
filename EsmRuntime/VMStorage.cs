using EsmRuntime.Common;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Common {

public readonly unsafe ref struct Slice<T>(ReadOnlySpan<byte> bytes): IByteSerializable<Slice<T>> where T: struct, ISizedValue<T>, allows ref struct {
    readonly ReadOnlySpan<byte> _bytes = bytes;

    public ReadOnlySpan<byte> Bytes => _bytes;

    public T this[int index] {
        get {
            int start = usize.ByteCount + index * T.ByteCount;
            int end = start + T.ByteCount;
            return T.FromSpan(_bytes[start..end]);
        }
    }

    public usize Length => usize.FromSpan(_bytes[..usize.ByteCount]);

    public void ToSpan(Span<byte> span) {
        usize length = Length;
        length.ToSpan(span[..usize.ByteCount]);
        for (usize i = 0; i < length; i++) {
            int start = usize.ByteCount + T.ByteCount * i;
            int end = start + T.ByteCount;
            Span<byte> varSpan = span[start..end];
            this[i].ToSpan(varSpan);
        }
    }
    public void ToPtr(byte* ptr) {
        ToSpan(new(ptr, InstanceSize));
    }

    public static Slice<T> FromSpan(ReadOnlySpan<byte> bytes) 
        => new(bytes);

    public static Slice<T> FromPtr(byte* ptr) {
        usize length = usize.FromPtr(ptr);
        return new(new(ptr, usize.ByteCount + length * T.ByteCount));
    }
    public int InstanceSize => usize.ByteCount + _bytes.Length * T.ByteCount;
}

}

namespace EsmRuntime {

// end-based
public unsafe ref struct OpStack(byte* startPtr, int length) {


    public int Length => _span.Length;
    
    public ReadOnlySpan<byte> Values => this[..(_offs+1)];

    public Span<byte> this[Range range] {
        get {
            (int offs, int length) = range.GetOffsetAndLength(_span.Length);
            int start = length - offs;
            return _span[^start..^offs];
            
        }
    }
    
    int _offs = -1;
    readonly Span<byte> _span = new(startPtr, length);
    
    public u8 this[Index offset] {
        get => _span[^(offset.GetOffset(_span.Length) + 1)];
        set => _span[^(offset.GetOffset(_span.Length) + 1)] = value;
    }

    public T Pop<T>() where T : struct, ISizedValue<T>, allows ref struct {
        int size = T.ByteCount;
        
        if (_offs + 1 < T.ByteCount) throw new StackUnderflowError(
            $"Tried to pop element of size {T.ByteCount} from operand stack with size {_offs + 1}"
        );
        if (size == 0) return T.FromSpan(_span[^0..]);
        ReadOnlySpan<byte> res = this[(_offs - size + 1)..(_offs+1)];
        _offs -= size;
        return T.FromSpan(res);
    }

    public void Push<T>(T value) where T: struct, ISizedValue<T>, allows ref struct {
        int size = T.ByteCount;
        if (_offs == _span.Length - size) throw new StackOverflowError("Operand stack is full!");
        var span = this[(_offs + 1)..(_offs + size + 1)];
        _offs += size;

        value.ToSpan(span);
    }

    public void Push(ReadOnlySpan<byte> value) {
        var span = this[(_offs + 1)..(_offs + value.Length + 1)];
        _offs += value.Length;
        value.CopyTo(span);
    }
}

public unsafe ref struct FrameStack(byte* start, int length) {
    int _offs = -1;

    int NextAvailableOffset => _offs == -1 ? 0 : _offs + *(start + _offs);

    public Frame Curr { get; private set; } = default;
    
    public Frame Pop() {
        switch (_offs) {
            case -1: throw new StackUnderflowError("Cannot pop frame stack as it is empty!");
            case 0: {
                Frame prev = Curr;
                prev.Clear();
                Curr = default;
                _offs = -1;
                return prev;
            }
            default: {
                Frame prev = Curr;
                prev.Clear();
                usize prevSize = usize.FromPtr(start + _offs - usize.ByteCount);
                _offs -= prevSize;
                Curr = Frame.FromPtr(start + _offs);
                return prev;
            }
        }
    }
    
    public void Push(byte* operand) {
        int newOffs = NextAvailableOffset;
        byte* frameStart = start + newOffs;
        var varSizes = Slice<usize>.FromPtr(operand);
        var table = OffsetTable.CreateFromSizes(varSizes, frameStart + usize.ByteCount,
            out usize fullSize);
        
        fullSize.ToPtr(frameStart);
        
        if (_offs + fullSize >= length)
            throw new StackOverflowError("Frame stack is full!");

        for (var i = 0; i < varSizes.Length; i++) {
            usize offset = table[i];
            usize varSize = varSizes[i];
            
            varSize.ToPtr(frameStart + offset);
        }
        
        
        fullSize.ToPtr(frameStart + fullSize - usize.ByteCount);
        
        var frame = new Frame(frameStart, table);
        Curr = frame;
        _offs = NextAvailableOffset;
    }
}

public readonly unsafe ref struct OffsetTable(Slice<usize> offsets): IByteSerializable<OffsetTable> {
    readonly Slice<usize> _offsets = offsets;
    public usize this[int index] => _offsets[index];

    
    public static OffsetTable CreateFromSizes(Slice<usize> sizes, byte* dest, out usize frameSize) {
        sizes.Length.ToPtr(dest);
        usize tableSize = usize.ByteCount + sizes.Bytes.Length;
        usize partialFrameSize = tableSize;
        for (var i = 0; i < sizes.Length; i++) {
            usize size = sizes[i];
            byte* offsetLoc = dest + usize.ByteCount + i * usize.ByteCount;
            partialFrameSize.ToPtr(offsetLoc);
            usize partialOffs = size + usize.ByteCount;
            partialFrameSize += partialOffs;
        }

        frameSize = partialFrameSize + usize.ByteCount;

        return new(Slice<usize>.FromPtr(dest));
    }

    
    public int InstanceSize => (_offsets.Length + 1) * usize.ByteCount;
    
    public void ToSpan(Span<byte> bytes) {
        fixed (byte* b = &bytes[0]) 
            ToPtr(b);
    }
    public void ToPtr(byte* ptr) {
        _offsets.Length.ToPtr(ptr);
        for (var i = 0; i < _offsets.Length; i ++)
            this[i].ToPtr(ptr + usize.ByteCount + i * usize.ByteCount);
    }

    public static OffsetTable FromSpan(ReadOnlySpan<byte> bytes) {
        var slice = Slice<usize>.FromSpan(bytes);
        return new(slice);
    }

    public static OffsetTable FromPtr(byte* ptr) {
        var slice = Slice<usize>.FromPtr(ptr);
        return new(slice);
    }
}

public readonly unsafe ref struct Frame(byte* start, OffsetTable table) {
    public usize Size => usize.FromPtr(start);

    OffsetTable Table { get; } = table;

    public Span<byte> this[byte index] {
        get {
            OffsetTable table = Table;
            usize offset = table[index];
            byte* var = start + offset;
            usize size = *var;
            return new(var + usize.ByteCount, size - usize.ByteCount);
        }
    }
    
    public static Frame FromPtr(byte* ptr) {
        OffsetTable table = OffsetTable.FromPtr(ptr + usize.ByteCount);
        return new(ptr, table);
    }

    public void Clear() {
        usize size = usize.FromPtr(start);
        var byteSpan = new Span<byte>(start, size);
        byteSpan.Clear();
    }
}

public unsafe ref struct Heap(byte* start, int size) {
    public readonly int Size = size;

    int _endCursor = 0;
    // int _startCursor = 0;

    public usize this[usize addr] {
        get {
            CheckAddr(addr);
            return  (usize) (start + addr);
        }
    }

    public usize Transform(usize addr) => (usize) (start + addr);


    public Reference<T> AllocateUnsized<T>(ReadOnlySpan<byte> data) where T: struct, IByteSerializable<T>, allows ref struct {
        int u = data.Length;
        _endCursor += u;
        byte* ptr = start + Size - _endCursor;
        Span<byte> dest = new(ptr, u);
        data.CopyTo(dest);
        T value = T.FromSpan(dest);
        
        return Reference<T>.CreateAt(ptr, value);
    }
    
    // TODO
    public void Free<T>(Reference<T> reference) where T: struct, IByteSerializable<T>, allows ref struct {
        
    }

    public Span<byte> this[Range range] => new Span<byte>(start, Size)[range];

    void CheckAddr(usize offset) {
        if (offset >= Size) throw new MemoryAccessError($"Invalid index {offset} for heap with size {Size}");
    }
}

}

