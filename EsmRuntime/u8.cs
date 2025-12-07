// ReSharper disable InconsistentNaming
namespace EsmRuntime;

public readonly record struct u8(byte b): ISpanFormattable {
    public static implicit operator u8(byte val) => new(val);
    public static implicit operator byte(u8 val) => val.b;
    
    public static u8 operator ~(u8 self) => unchecked((byte)~self.b);
    public static u8 operator &(u8 a, u8 b) => unchecked((byte)(a.b & b.b));
    public static u8 operator |(u8 a, u8 b) => unchecked((byte)(a.b | b.b));
    public static u8 operator ^(u8 a, u8 b) => unchecked((byte)(a.b ^ b.b));
    public static u8 operator <<(u8 a, u8 b) => unchecked((byte)(a.b << b.b));
    public static u8 operator >>(u8 a, u8 b) => unchecked((byte)(a.b >>> b.b));

    public static explicit operator char(u8 val) => (char) val.b;
    public static explicit operator u8(char val) => new(unchecked((byte)val));

    public string Bin => $"{b:B8}";
    public string Hex => $"{b:X2}";
    public string Dec => $"{b:D}";

    public override string ToString() => b.ToString();
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => b.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => b.TryFormat(destination, out charsWritten, format, provider);
}