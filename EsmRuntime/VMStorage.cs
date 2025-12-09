using EsmRuntime.Common.Types;

namespace EsmRuntime;

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

    public readonly byte* Loc(int offset) => startPtr + offset;

    public u8 this[Index offset] {
        get => _span[^(offset.GetOffset(_span.Length) + 1)];
        set => _span[^(offset.GetOffset(_span.Length) + 1)] = value;
    }

    internal int LastIndexOf(u8 b) {
        for (int i = _offs; i >= 0; i--) {
            if (this[i] == b) return i;
        }

        return -1;
    }

    public T Pop<T>() where T : struct, ISizedValue<T> {
        int size = T.ByteCount;
        
        if (_offs + 1 < T.ByteCount) throw new StackUnderflowError(
            $"Tried to pop element of size {T.ByteCount} from operand stack with size {_offs + 1}"
        );
        if (size == 0) return T.FromSpan(_span[^0..]);
        ReadOnlySpan<byte> res = this[(_offs - size + 1)..(_offs+1)];
        _offs -= size;
        return T.FromSpan(res);
    }
    
    internal ReadOnlySpan<byte> Pop(int count) {
        if (_offs + 1 < count) throw new StackUnderflowError($"Tried to pop {count} elements from operand stack with size {_offs + 1}");
        else if (count == 0) return _span[^0..];
        ReadOnlySpan<byte> res = this[(_offs - count + 1)..(_offs+1)];
        _offs -= count;
        return res;
    }
    
    internal ReadOnlySpan<byte> PopTo(int index) 
        => Pop(_offs - index);
    
    internal ReadOnlySpan<byte> PopStr() 
        => Pop(_offs - LastIndexOf((byte) '\0') + 1)[..^1];

    public u8 Pop() => _offs == -1
        ? throw new StackUnderflowError("Operand stack is empty!")
        : this[_offs--];

    public void Push(u8 value) {
        if (_offs == _span.Length - 1) throw new StackOverflowError("Operand stack is full!");
        this[++_offs] = value;
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

public unsafe ref struct Registry(byte* start, int length) {
    int _offs = -1;

    int NextAvailableOffset => _offs == -1 ? 0 : _offs + *(start + _offs);

    public Frame Curr { get; private set; } = default;

    public Frame Pop() {
        switch (_offs) {
            case -1: throw new StackUnderflowError("Cannot pop frame stack as it is empty!");
            case 0: {
                Frame prev = Curr;
                Curr = default;
                _offs = -1;
                return prev;
            }
            default: {
                Frame prev = Curr;
                usize prevSize = usize.FromPtr(start + _offs - usize.ByteCount);
                _offs -= prevSize;
                Curr = Frame.FromPtr(start + _offs);
                return prev;
            }
        }
    }
    
    public void Push(byte* operand) {
        byte operandSize = *operand;
        byte* varDecStart = operand + 1;
        byte tableSize = operandSize;
        // one usize for header, one for var count, size of operand for offset table ...
        usize frameSize = usize.ByteCount * 2 + tableSize;
        int newOffs = NextAvailableOffset;
        byte* frameStart = start + newOffs;
        Span<byte> tableSpan = new(start + newOffs + usize.ByteCount * 2, tableSize);
        for (var i = 0; i < operandSize; i += usize.ByteCount) {
            usize size = usize.FromPtr(varDecStart + i);
            frameSize.ToSpan(tableSpan[i..(i+usize.ByteCount)]);
            usize partialOffs = size + usize.ByteCount;
            size.ToPtr(frameStart + frameSize);
            frameSize += partialOffs;
        }
        
        if (_offs + frameSize >= length)
            throw new StackOverflowError("Frame stack is full!");

        // ... and another for size in footer
        frameSize += usize.ByteCount;
        
        Span<byte> alloc = new(start + newOffs, frameSize);
        frameSize.ToSpan(alloc[..usize.ByteCount]);
        frameSize.ToSpan(alloc[^usize.ByteCount..]);
        OffsetTable table = new(tableSpan);
        var frame = new Frame(frameStart, table);
        Curr = frame;
        _offs = NextAvailableOffset;
    }
}

public readonly unsafe ref struct OffsetTable(ReadOnlySpan<byte> span) {
    readonly ReadOnlySpan<byte> _span = span;
    public usize this[byte index] => usize.FromSpan(_span[(index * usize.ByteCount)..(index * usize.ByteCount + usize.ByteCount)]);
}

public readonly unsafe ref struct Frame(byte* start, OffsetTable table) {
    public usize Size => usize.FromPtr(start);

    OffsetTable Table {
        get {
            int tableSize = usize.ByteCount * usize.FromPtr(start + usize.ByteCount);
            return new(new(start + 2*usize.ByteCount, tableSize));
        }
    }

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
        usize varNum = usize.FromPtr(ptr + usize.ByteCount);
        OffsetTable table = new(new(ptr + (2 * usize.ByteCount), usize.ByteCount * varNum));
        return new(ptr, table);
    }
}

public unsafe ref struct Heap(byte* start, int length) {
    public readonly int Length = length;

    int _endCursor = 0;
    // int _startCursor = 0;

    public byte* this[usize offset] {
        get {
            CheckOffset(offset);
            return start + offset - 1;
        }
    }

    public void AllocateUnsized(usize loc, ReadOnlySpan<byte> data) {
        int u = data.Length;
        byte* ptr = start + Length - _endCursor - u;
        data.CopyTo(new (ptr, u));
        ptr -= usize.ByteCount;
        ((usize)u).ToSpan(new(ptr, usize.ByteCount));
        _endCursor += u + usize.ByteCount;

        var addr = (usize) ptr;

        addr.ToPtr((byte*) loc);
    }
    
    public void Free(usize loc) {
        byte* ptr = (byte*) loc;

        usize length = usize.FromPtr(ptr);
    }

    public Span<byte> this[Range range] => new Span<byte>(start, Length)[range];

    void CheckOffset(usize offset) {
        if (offset >= Length) throw new MemoryAccessError($"Invalid index {offset} for length {Length + 1}");
    }
}

