using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory;

// end-based
public unsafe ref struct OpStack(byte* startPtr, nint length) {
    
    nint _offs = -1;

    public T Pop<T>() where T : struct, ISizedValue<T>, allows ref struct {
        nuint size = T.ByteCount;
        
        if (_offs + 1 < (long) T.ByteCount) throw new StackUnderflowError(
            $"Tried to pop element of size {T.ByteCount} from operand stack with size {_offs + 1}"
        );
        if (size == 0) return T.FromFatPtr(null, 0);
        _offs -= (nint) size;
        return T.FromFatPtr(startPtr + _offs - size + 1, size);
    }

    public void Push<T>(T value) where T : struct, IByteSerializable<T>, allows ref struct {
        nuint size = value.InstSize;
        if (_offs + (long) size > length) throw new StackOverflowError("Operand stack is full!");
        byte* ptr = startPtr + _offs + 1;
        _offs += (nint) size;

        value.ToPtr(ptr);
    }
}