using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory;

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

    public void Push<T>(T value) where T: struct, IByteSerializable<T>, allows ref struct {
        int size = value.InstanceSize;
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