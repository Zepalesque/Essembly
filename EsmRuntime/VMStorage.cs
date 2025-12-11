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







}

