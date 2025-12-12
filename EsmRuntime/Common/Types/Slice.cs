namespace EsmRuntime.Common.Types;

public readonly unsafe ref struct Slice<T>(ReadOnlySpan<byte> bytes): IByteSerializable<Slice<T>> where T: struct, ISizedValue<T>, allows ref struct {
    readonly ReadOnlySpan<byte> _bytes = bytes;

    public ReadOnlySpan<byte> Bytes => _bytes;
    
    public usize Length { get; } = bytes.Length / T.ByteCount;

    public T this[int index] {
        get {
            int start = usize.ByteCount + index * T.ByteCount;
            int end = start + T.ByteCount;
            return T.FromSpan(_bytes[start..end]);
        }
    }

    public void ToSpan(Span<byte> span) {
        for (var i = 0; i < span.Length; i += T.ByteCount) {
            int end = i + T.ByteCount;
            var varSpan = span[i..end];
            this[i].ToSpan(varSpan);
        }
    }
    public void ToPtr(byte* ptr) {
        ToSpan(new(ptr, InstanceSize));
    }

    public static Slice<T> FromSpan(ReadOnlySpan<byte> bytes) 
        => new(bytes);
    
    public int InstanceSize => usize.ByteCount + _bytes.Length * T.ByteCount;
}