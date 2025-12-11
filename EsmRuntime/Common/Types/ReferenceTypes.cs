using System.Text;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

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