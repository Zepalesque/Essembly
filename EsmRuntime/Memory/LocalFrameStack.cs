using EsmRuntime.Common;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Storage;

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
            byte* ptr = start + offset;
            usize size = usize.FromPtr(ptr);
            return new(ptr + usize.ByteCount, size - usize.ByteCount);
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