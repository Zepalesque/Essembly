using System.Text;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

#pragma warning disable CS8981
public readonly unsafe struct ImmutableSlice<T>(byte* start, usize lengthInElements) : IReference<ImmutableSlice<T>> where T: unmanaged, ISizedValue<T> {
    public byte* Start { get; } = start;
    usize LengthInElements { get; } = lengthInElements;

    public int ByteCountInMemory { get; } = usize.ByteCount + lengthInElements * T.ByteCount;

    public static ImmutableSlice<T> FromRefPtr(byte* ptr) {
        ReadOnlySpan<byte> span = new(ptr, usize.ByteCount);
        
        usize size = usize.FromSpan(span);
        
        return new(ptr, size);
    }

    public T this[int i] {
        get {

            if (i >= LengthInElements)
                throw new InvalidIndexError($"Invalid index {i} for slice with length {LengthInElements}");
            
            int size = T.ByteCount;
            
            int offestBytes = usize.ByteCount + i * size;
            return T.FromSpan(new(Start + offestBytes, size));
        }
    }
}
#pragma warning restore CS8981

// Null-terminated
public readonly unsafe struct StringRef(byte* start, int length) : IReference<StringRef> {
    public byte* Start { get; } = start;
    int Length { get; } = length;


    public int ByteCountInMemory => Length;

    public static StringRef FromRefPtr(byte* ptr) {
        var i = 0;
        while (*(ptr + i) != 0x00) i++;

        return new(ptr, i + 1);
    }

    public ReadOnlySpan<byte> ToStringSpan() => new(Start, Length);

    public override string ToString() => Encoding.ASCII.GetString(ToStringSpan());
}

// Null-terminated
public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IByteSerializable<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;
    
    public Slice<u8> Utf8 => _utf8;

    public static ReadOnlySpan<byte> DataToSerialize(byte* start) {
        var i = 0;
        while (*(start + i) != '\0') {
            i++;
        }

        return new(start, i);
    }

    public override string ToString() => Encoding.ASCII.GetString(_utf8.Bytes);
    
    public void ToSpan(Span<byte> span) {
        _utf8.ToSpan(span);
    }
    public void ToPtr(byte* ptr) {
        _utf8.ToPtr(ptr);
    }
    public static StringSlice FromSpan(ReadOnlySpan<byte> bytes) 
        => new(Slice<u8>.FromSpan(bytes));

    public static StringSlice FromPtr(byte* ptr) 
        => new(Slice<u8>.FromPtr(ptr));
    public int InstanceSize => _utf8.InstanceSize;
}