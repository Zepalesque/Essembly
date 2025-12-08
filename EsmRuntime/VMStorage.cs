namespace EsmRuntime;

// end-based
internal ref struct OpStack(Span<byte> span) {
    
    public ReadOnlySpan<byte> Values => this[..(_offs+1)];

    public Span<byte> this[Range range] {
        get {
            (int offs, int length) = range.GetOffsetAndLength(_span.Length);

            int start = length - offs;
            
            return _span[^start..^offs];
        }
    }
    
    int _offs = -1;
    readonly Span<byte> _span = span;

    int FullSize => _span.Length;

    u8 this[Index offset] {
        get => _span[^(offset.GetOffset(_span.Length) + 1)];
        set => _span[^(offset.GetOffset(_span.Length) + 1)] = value;
    }

    internal int LastIndexOf(u8 b) {
        for (int i = _offs; i >= 0; i--) {
            if (this[i] == b) return i;
        }

        return -1;
    }
    
    internal ReadOnlySpan<byte> Pop(int count) {
        if (_offs + 1 < count) throw new StackUnderflowError($"Tried to pop {count} elements from stack with size {_offs + 1}");
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
        ? throw new StackUnderflowError("Stack is empty!")
        : this[_offs--];

    public void operator +=(u8 value) {
        if (_offs == _span.Length - 1) throw new StackOverflowError("Stack is full!");
        this[++_offs] = value;
    }

    public void operator +=(ReadOnlySpan<byte> value) {
        var span = this[(_offs + 1)..(_offs + value.Length + 1)];
        _offs += value.Length;
        value.CopyTo(span);
    }
}

internal readonly unsafe ref struct Memory(byte* start, int lastIndex) {
    public u8 this[int offset] {
        get {
            CheckOffset(offset);
            return *(start + offset - 1);
        }
        set {
            CheckOffset(offset);
            *(start + offset - 1) = value;
        }
    }

    void CheckOffset(int offset) {
        if (offset > lastIndex) throw new MemoryAccessError($"Invalid index {offset} for length {lastIndex + 1}");
    }
}