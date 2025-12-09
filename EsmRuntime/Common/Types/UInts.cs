// ReSharper disable InconsistentNaming

using System.Numerics;

// ReSharper disable UseSymbolAlias
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common.Types;



public readonly record struct u8(byte value):
    ISpanFormattable,
    INumberFormattable,
    IAsciiFormattable<u8>,
    ISizedValue<u8>,
    IBitwiseOperators<u8, u8, u8>,
    IAdditionOperators<u8, u8, u8>,
    IMultiplyOperators<u8, u8, u8>,
    IDivisionOperators<u8, u8, u8>,
    IModulusOperators<u8, u8, u8>,
    IShiftOperators<u8, u8, u8>,
    IComparisonOperators<u8, u8, bool>,
    IUnaryPlusOperators<u8, u8>,
    IUnaryNegationOperators<u8, u8>,
    IAdditiveIdentity<u8, u8>,
    IMultiplicativeIdentity<u8, u8>,
    IComparable<u8> {
    public static implicit operator u8(byte val) => new(val);
    public static implicit operator byte(u8 val) => val.value;
    
    public static u8 operator ~(u8 self) => unchecked((byte)~self.value);
    public static u8 operator &(u8 a, u8 b) => unchecked((byte)(a.value & b.value));
    public static u8 operator |(u8 a, u8 b) => unchecked((byte)(a.value | b.value));
    public static u8 operator ^(u8 a, u8 b) => unchecked((byte)(a.value ^ b.value));
    public static u8 operator <<(u8 a, u8 b) => unchecked((byte)(a.value << b.value));
    public static u8 operator >>(u8 a, u8 b) => unchecked((byte)(a.value >> b.value));
    public static u8 operator >>>(u8 a, u8 b) => unchecked((byte)(a.value >>> b.value));

    public static explicit operator char(u8 val) => (char) val.value;
    public static explicit operator u8(char val) => new(unchecked((byte)val));

    public string Bin => $"{value:B8}";
    public string Hex => $"{value:X2}";
    public string Dec => $"{value:D}";


    public int CompareTo(u8 other) => value.CompareTo(other.value);

    public static u8 operator %(u8 left, u8 right) => unchecked((byte) (left.value % right.value));

    public static u8 operator +(u8 value) => value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static u8 Parse(string s, IFormatProvider? provider) => byte.Parse(s, provider);

    public static u8 operator +(u8 left, u8 right) => unchecked((byte) (left.value + right.value));
    
    public static u8 AdditiveIdentity { get; } = 0;
    
    public static bool operator >(u8 left, u8 right) => left.value > right.value;

    public static bool operator >=(u8 left, u8 right) => left.value >= right.value;

    public static bool operator <(u8 left, u8 right) => left.value < right.value;

    public static bool operator <=(u8 left, u8 right) => left.value <= right.value;

    public static u8 operator --(u8 value) => value - 1;

    public static u8 operator /(u8 left, u8 right) => unchecked((byte) (left.value / right.value));

    public static u8 operator ++(u8 value) => value + 1;
    public static u8 MultiplicativeIdentity { get; } = 1;
    public static u8 operator *(u8 left, u8 right) => unchecked((byte) (left.value * right.value));

    public static u8 operator -(u8 left, u8 right) => unchecked((byte) (left.value - right.value));

    public static u8 operator -(u8 value) => unchecked((byte)-value.value);

    public static unsafe u8 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static int ByteCount => 1;

    public void ToSpan(Span<byte> span) => span[0] = this;
    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static u8 FromSpan(ReadOnlySpan<byte> bytes) => bytes[0];
}


public readonly record struct u16(ushort value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<u16>,
    ISizedValue<u16>,
    IBitwiseOperators<u16, u16, u16>,
    IAdditionOperators<u16, u16, u16>,
    IMultiplyOperators<u16, u16, u16>,
    IDivisionOperators<u16, u16, u16>,
    IModulusOperators<u16, u16, u16>,
    IShiftOperators<u16, u16, u16>,
    IComparisonOperators<u16, u16, bool>,
    IUnaryPlusOperators<u16, u16>,
    IUnaryNegationOperators<u16, u16>,
    IAdditiveIdentity<u16, u16>,
    IMultiplicativeIdentity<u16, u16>,
    IComparable<u16>,
    IMinMaxValue<u16>
{
    public static implicit operator u16(ushort val) => new(val);
    public static implicit operator ushort(u16 val) => val.value;
    
    public static u16 operator ~(u16 self) => unchecked((ushort)~self.value);
    public static u16 operator &(u16 a, u16 b) => unchecked((ushort)(a.value & b.value));
    public static u16 operator |(u16 a, u16 b) => unchecked((ushort)(a.value | b.value));
    public static u16 operator ^(u16 a, u16 b) => unchecked((ushort)(a.value ^ b.value));
    public static u16 operator <<(u16 a, u16 b) => unchecked((ushort)(a.value << b.value));
    public static u16 operator >>(u16 a, u16 b) => unchecked((ushort)(a.value >> b.value));
    public static u16 operator >>>(u16 a, u16 b) => unchecked((ushort)(a.value >>> b.value));


    public string Bin => $"{value:B16}";
    public string Hex => $"{value:X4}";
    public string Dec => $"{value:D}";


    public int CompareTo(u16 other) => value.CompareTo(other.value);

    public static u16 operator %(u16 left, u16 right) => unchecked((ushort) (left.value % right.value));

    public static u16 operator +(u16 value) => value;
    public static explicit operator char(u16 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static u16 Parse(string s, IFormatProvider? provider) => ushort.Parse(s, provider);

    public static u16 operator +(u16 left, u16 right) => unchecked((ushort) (left.value + right.value));
    
    public static u16 AdditiveIdentity { get; } = 0;
    public static bool operator >(u16 left, u16 right) => left.value > right.value;

    public static bool operator >=(u16 left, u16 right) => left.value >= right.value;

    public static bool operator <(u16 left, u16 right) => left.value < right.value;

    public static bool operator <=(u16 left, u16 right) => left.value <= right.value;

    public static u16 operator --(u16 value) => value - 1;

    public static u16 operator /(u16 left, u16 right) => unchecked((ushort) (left.value / right.value));

    public static u16 operator ++(u16 value) => value + 1;
    public static u16 operator *(u16 left, u16 right) => unchecked((ushort) (left.value * right.value));

    public static u16 operator -(u16 left, u16 right) => unchecked((ushort) (left.value - right.value));

    public static u16 operator -(u16 value) => unchecked((ushort)-value.value);

    public static unsafe u16 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static u16 MultiplicativeIdentity { get; } = 1;
    public static int ByteCount => 2;

    public void ToSpan(Span<byte> span) {
        span[0] = unchecked((byte) (value >>> 8));
        span[1] = unchecked((byte) (value & 0xFFFF));
    }

    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static u16 FromSpan(ReadOnlySpan<byte> bytes) 
        => unchecked((ushort) ((bytes[0] << 8) | bytes[1]));

    public static u16 MaxValue => ushort.MaxValue;
    public static u16 MinValue => ushort.MinValue;
}


public readonly record struct u32(uint value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<u32>,
    ISizedValue<u32>,
    IBitwiseOperators<u32, u32, u32>,
    IAdditionOperators<u32, u32, u32>,
    IMultiplyOperators<u32, u32, u32>,
    IDivisionOperators<u32, u32, u32>,
    IModulusOperators<u32, u32, u32>,
    IShiftOperators<u32, u32, u32>,
    IComparisonOperators<u32, u32, bool>,
    IUnaryPlusOperators<u32, u32>,
    IUnaryNegationOperators<u32, u32>,
    IAdditiveIdentity<u32, u32>,
    IMultiplicativeIdentity<u32, u32>,
    IComparable<u32> {
    public static implicit operator u32(uint val) => new(val);
    public static implicit operator uint(u32 val) => val.value;
    
    public static u32 operator ~(u32 self) => ~self.value;
    public static u32 operator &(u32 a, u32 b) => a.value & b.value;
    public static u32 operator |(u32 a, u32 b) => a.value | b.value;
    public static u32 operator ^(u32 a, u32 b) => a.value ^ b.value;
    public static u32 operator <<(u32 a, u32 b) => unchecked(a.value << (int) b.value);
    public static u32 operator >>(u32 a, u32 b) => unchecked(a.value >> (int) b.value);
    public static u32 operator >>>(u32 a, u32 b) => unchecked(a.value >>> (int) b.value);


    public string Bin => $"{value:B32}";
    public string Hex => $"{value:X8}";
    public string Dec => $"{value:D}";


    public int CompareTo(u32 other) => value.CompareTo(other.value);

    public static u32 operator %(u32 left, u32 right) => left.value % right.value;

    public static u32 operator +(u32 value) => value;
    public static explicit operator char(u32 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static u32 Parse(string s, IFormatProvider? provider) => uint.Parse(s, provider);

    public static u32 operator +(u32 left, u32 right) => unchecked(left.value + right.value);
    
    public static u32 AdditiveIdentity { get; } = 0;
    public static bool operator >(u32 left, u32 right) => left.value > right.value;

    public static bool operator >=(u32 left, u32 right) => left.value >= right.value;

    public static bool operator <(u32 left, u32 right) => left.value < right.value;

    public static bool operator <=(u32 left, u32 right) => left.value <= right.value;

    public static u32 operator --(u32 value) => value - 1;

    public static u32 operator /(u32 left, u32 right) => left.value / right.value;

    public static u32 operator ++(u32 value) => value + 1;
    public static u32 operator *(u32 left, u32 right) => unchecked(left.value * right.value);

    public static u32 operator -(u32 left, u32 right) => unchecked(left.value - right.value);

    public static u32 operator -(u32 value) => unchecked((uint)-value.value);

    public static unsafe u32 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static u32 MultiplicativeIdentity { get; } = 1;
    public static int ByteCount => 4;
    
    public void ToSpan(Span<byte> span) {
        span[0] = unchecked((byte) (value >>> 24));
        span[1] = unchecked((byte) (value >>> 16));
        span[2] = unchecked((byte) (value >>> 8));
        span[3] = unchecked((byte) (value & 0xFF));
    }

    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static u32 FromSpan(ReadOnlySpan<byte> bytes) 
        => unchecked((uint) ((bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3]));
}


public readonly record struct u64(ulong value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<u64>,
    ISizedValue<u64>,
    IBitwiseOperators<u64, u64, u64>,
    IAdditionOperators<u64, u64, u64>,
    IMultiplyOperators<u64, u64, u64>,
    IDivisionOperators<u64, u64, u64>,
    IModulusOperators<u64, u64, u64>,
    IShiftOperators<u64, u64, u64>,
    IComparisonOperators<u64, u64, bool>,
    IUnaryPlusOperators<u64, u64>,
    IUnaryNegationOperators<u64, u64>,
    IAdditiveIdentity<u64, u64>,
    IMultiplicativeIdentity<u64, u64>,
    IComparable<u64> {
    public static implicit operator u64(ulong val) => new(val);
    public static implicit operator ulong(u64 val) => val.value;
    
    public static u64 operator ~(u64 self) => ~self.value;
    public static u64 operator &(u64 a, u64 b) => a.value & b.value;
    public static u64 operator |(u64 a, u64 b) => a.value | b.value;
    public static u64 operator ^(u64 a, u64 b) => a.value ^ b.value;
    public static u64 operator <<(u64 a, u64 b) => unchecked(a.value << (int) b.value);
    public static u64 operator >>(u64 a, u64 b) => unchecked(a.value >> (int) b.value);
    public static u64 operator >>>(u64 a, u64 b) => unchecked(a.value >>> (int) b.value);


    public string Bin => $"{value:B64}";
    public string Hex => $"{value:X16}";
    public string Dec => $"{value:D}";


    public int CompareTo(u64 other) => value.CompareTo(other.value);

    public static u64 operator %(u64 left, u64 right) => left.value % right.value;

    public static u64 operator +(u64 value) => value;
    public static explicit operator char(u64 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static u64 Parse(string s, IFormatProvider? provider) => ulong.Parse(s, provider);

    public static u64 operator +(u64 left, u64 right) => unchecked(left.value + right.value);
    
    public static u64 AdditiveIdentity { get; } = 0;
    public static bool operator >(u64 left, u64 right) => left.value > right.value;

    public static bool operator >=(u64 left, u64 right) => left.value >= right.value;

    public static bool operator <(u64 left, u64 right) => left.value < right.value;

    public static bool operator <=(u64 left, u64 right) => left.value <= right.value;

    public static u64 operator --(u64 value) => value - 1;

    public static u64 operator /(u64 left, u64 right) => left.value / right.value;

    public static u64 operator ++(u64 value) => value + 1;
    public static u64 operator *(u64 left, u64 right) => unchecked(left.value * right.value);

    public static u64 operator -(u64 left, u64 right) => unchecked(left.value - right.value);
    
    
    
    public static u64 operator -(u64 value) => unchecked(0u - value.value);

    public static unsafe u64 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static u64 MultiplicativeIdentity { get; } = 1;
    public static int ByteCount => 8;

    public void ToSpan(Span<byte> span) {
        span[0] = unchecked((byte) (value >>> 56));
        span[1] = unchecked((byte) (value >>> 48));
        span[2] = unchecked((byte) (value >>> 40));
        span[3] = unchecked((byte) (value >>> 32));
        span[4] = unchecked((byte) (value >>> 24));
        span[5] = unchecked((byte) (value >>> 16));
        span[6] = unchecked((byte) (value >>> 8));
        span[7] = unchecked((byte) (value & 0xFF));
    }

    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static u64 FromSpan(ReadOnlySpan<byte> bytes) 
        => (ulong) bytes[0] << 56
        | ((ulong) bytes[1] << 48)
        | ((ulong) bytes[2] << 40)
        | ((ulong) bytes[3] << 32)
        | ((ulong) bytes[4] << 24)
        | ((ulong) bytes[5] << 16)
        | ((ulong) bytes[6] << 8)
        | bytes[7];
}