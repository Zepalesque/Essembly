namespace EsmRuntime;

internal unsafe ref struct OpStack(byte* start, byte size) {

    byte _offs = 0;

    byte this[byte offset] {
        get => *(start + offset - 1);
        set => *(start + offset - 1) = value;
    }

    public u8 Pop() => _offs == 0 
        ? throw new InvalidOperationException("Stack is empty!")
        : this[_offs--];

    public void operator +=(u8 value) {
        if (_offs == size) throw new InvalidOperationException("Stack is full!");
        this[++_offs] = value;
    }
}