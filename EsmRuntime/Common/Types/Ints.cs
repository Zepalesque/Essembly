// ReSharper disable InconsistentNaming

using System.Numerics;

// ReSharper disable UseSymbolAlias
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common.Types;



public readonly record struct i8(sbyte value):
    ISpanFormattable,
    INumberFormattable,
    IAsciiFormattable<i8>,
    ISizedValue<i8>,
    IBitwiseOperators<i8, i8, i8>,
    IAdditionOperators<i8, i8, i8>,
    ISubtractionOperators<i8, i8, i8>,
    IMultiplyOperators<i8, i8, i8>,
    IDivisionOperators<i8, i8, i8>,
    IModulusOperators<i8, i8, i8>,
    IShiftOperators<i8, i8, i8>,
    IComparisonOperators<i8, i8, bool>,
    IUnaryPlusOperators<i8, i8>,
    IUnaryNegationOperators<i8, i8>,
    IAdditiveIdentity<i8, i8>,
    IMultiplicativeIdentity<i8, i8>,
    IComparable<i8>,
    IMinMaxValue<i8> {
    public static implicit operator i8(sbyte val) => new(val);
    public static implicit operator sbyte(i8 val) => val.value;
    
    public static i8 operator ~(i8 self) => unchecked((sbyte)~self.value);
    public static i8 operator &(i8 a, i8 b) => unchecked((sbyte)(a.value & b.value));
    public static i8 operator |(i8 a, i8 b) => unchecked((sbyte)(a.value | b.value));
    public static i8 operator ^(i8 a, i8 b) => unchecked((sbyte)(a.value ^ b.value));
    public static i8 operator <<(i8 a, i8 b) => unchecked((sbyte)(a.value << b.value));
    public static i8 operator >>(i8 a, i8 b) => unchecked((sbyte)(a.value >> b.value));
    public static i8 operator >>>(i8 a, i8 b) => unchecked((sbyte)(a.value >>> b.value));

    public static explicit operator char(i8 val) => (char) val.value;
    public static explicit operator i8(char val) => new(unchecked((sbyte)val));

    public string Bin => $"{value:B8}";
    public string Hex => $"{value:X2}";
    public string Dec => $"{value:D}";


    public int CompareTo(i8 other) => value.CompareTo(other.value);

    public static i8 operator %(i8 left, i8 right) => unchecked((sbyte) (left.value % right.value));

    public static i8 operator +(i8 value) => value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static i8 Parse(string s, IFormatProvider? provider) => sbyte.Parse(s, provider);

    public static i8 operator +(i8 left, i8 right) => unchecked((sbyte) (left.value + right.value));
    
    public static i8 AdditiveIdentity { get; } = 0;
    
    public static bool operator >(i8 left, i8 right) => left.value > right.value;

    public static bool operator >=(i8 left, i8 right) => left.value >= right.value;

    public static bool operator <(i8 left, i8 right) => left.value < right.value;

    public static bool operator <=(i8 left, i8 right) => left.value <= right.value;

    public static i8 operator --(i8 value) => value - 1;

    public static i8 operator /(i8 left, i8 right) => unchecked((sbyte) (left.value / right.value));

    public static i8 operator ++(i8 value) => value + 1;
    public static i8 MultiplicativeIdentity { get; } = 1;
    public static i8 operator *(i8 left, i8 right) => unchecked((sbyte) (left.value * right.value));

    public static i8 operator -(i8 left, i8 right) => unchecked((sbyte) (left.value - right.value));

    public static i8 operator -(i8 value) => unchecked((sbyte)-value.value);

    public static unsafe i8 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static int ByteCount => 1;

    public void ToSpan(Span<byte> span) => span[0] = unchecked((byte)value);
    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static i8 FromSpan(ReadOnlySpan<byte> bytes) => unchecked((sbyte) bytes[0]);
    
    public static i8 MaxValue { get; } = sbyte.MaxValue;
    public static i8 MinValue { get; } = sbyte.MinValue;
}


public readonly record struct i16(short value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<i16>,
    ISizedValue<i16>,
    IBitwiseOperators<i16, i16, i16>,
    IAdditionOperators<i16, i16, i16>,
    ISubtractionOperators<i16, i16, i16>,
    IMultiplyOperators<i16, i16, i16>,
    IDivisionOperators<i16, i16, i16>,
    IModulusOperators<i16, i16, i16>,
    IShiftOperators<i16, i16, i16>,
    IComparisonOperators<i16, i16, bool>,
    IUnaryPlusOperators<i16, i16>,
    IUnaryNegationOperators<i16, i16>,
    IAdditiveIdentity<i16, i16>,
    IMultiplicativeIdentity<i16, i16>,
    IComparable<i16>,
    IMinMaxValue<i16> {
    public static implicit operator i16(short val) => new(val);
    public static implicit operator short(i16 val) => val.value;
    
    public static i16 operator ~(i16 self) => unchecked((short)~self.value);
    public static i16 operator &(i16 a, i16 b) => unchecked((short)(a.value & b.value));
    public static i16 operator |(i16 a, i16 b) => unchecked((short)(a.value | b.value));
    public static i16 operator ^(i16 a, i16 b) => unchecked((short)(a.value ^ b.value));
    public static i16 operator <<(i16 a, i16 b) => unchecked((short)(a.value << b.value));
    public static i16 operator >>(i16 a, i16 b) => unchecked((short)(a.value >> b.value));
    public static i16 operator >>>(i16 a, i16 b) => unchecked((short)(a.value >>> b.value));


    public string Bin => $"{value:B16}";
    public string Hex => $"{value:X4}";
    public string Dec => $"{value:D}";


    public int CompareTo(i16 other) => value.CompareTo(other.value);

    public static i16 operator %(i16 left, i16 right) => unchecked((short) (left.value % right.value));

    public static i16 operator +(i16 value) => value;
    public static explicit operator char(i16 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static i16 Parse(string s, IFormatProvider? provider) => short.Parse(s, provider);

    public static i16 operator +(i16 left, i16 right) => unchecked((short) (left.value + right.value));
    
    public static i16 AdditiveIdentity { get; } = 0;
    public static bool operator >(i16 left, i16 right) => left.value > right.value;

    public static bool operator >=(i16 left, i16 right) => left.value >= right.value;

    public static bool operator <(i16 left, i16 right) => left.value < right.value;

    public static bool operator <=(i16 left, i16 right) => left.value <= right.value;

    public static i16 operator --(i16 value) => value - 1;

    public static i16 operator /(i16 left, i16 right) => unchecked((short) (left.value / right.value));

    public static i16 operator ++(i16 value) => value + 1;
    public static i16 operator *(i16 left, i16 right) => unchecked((short) (left.value * right.value));

    public static i16 operator -(i16 left, i16 right) => unchecked((short) (left.value - right.value));

    public static i16 operator -(i16 value) => unchecked((short)-value.value);

    public static unsafe i16 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static i16 MultiplicativeIdentity { get; } = 1;
    public static int ByteCount => 2;

    public void ToSpan(Span<byte> span) {
        span[0] = unchecked((byte) (value >>> 8));
        span[1] = unchecked((byte) (value & 0xFFFF));
    }

    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static i16 FromSpan(ReadOnlySpan<byte> bytes) 
        => unchecked((short) ((bytes[0] << 8) | bytes[1]));
    
    public static i16 MaxValue { get; } = short.MaxValue;
    public static i16 MinValue { get; } = short.MinValue;
}


public readonly record struct i32(int value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<i32>,
    ISizedValue<i32>,
    IBitwiseOperators<i32, i32, i32>,
    IAdditionOperators<i32, i32, i32>,
    ISubtractionOperators<i32, i32, i32>,
    IMultiplyOperators<i32, i32, i32>,
    IDivisionOperators<i32, i32, i32>,
    IModulusOperators<i32, i32, i32>,
    IShiftOperators<i32, i32, i32>,
    IComparisonOperators<i32, i32, bool>,
    IUnaryPlusOperators<i32, i32>,
    IUnaryNegationOperators<i32, i32>,
    IAdditiveIdentity<i32, i32>,
    IMultiplicativeIdentity<i32, i32>,
    IComparable<i32>,
    IMinMaxValue<i32> {
    public static implicit operator i32(int val) => new(val);
    public static implicit operator int(i32 val) => val.value;
    
    public static i32 operator ~(i32 self) => ~self.value;
    public static i32 operator &(i32 a, i32 b) => a.value & b.value;
    public static i32 operator |(i32 a, i32 b) => a.value | b.value;
    public static i32 operator ^(i32 a, i32 b) => a.value ^ b.value;
    public static i32 operator <<(i32 a, i32 b) => unchecked(a.value << (int) b.value);
    public static i32 operator >>(i32 a, i32 b) => unchecked(a.value >> (int) b.value);
    public static i32 operator >>>(i32 a, i32 b) => unchecked(a.value >>> (int) b.value);


    public string Bin => $"{value:B32}";
    public string Hex => $"{value:X8}";
    public string Dec => $"{value:D}";


    public int CompareTo(i32 other) => value.CompareTo(other.value);

    public static i32 operator %(i32 left, i32 right) => left.value % right.value;

    public static i32 operator +(i32 value) => value;
    public static explicit operator char(i32 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static i32 Parse(string s, IFormatProvider? provider) => int.Parse(s, provider);

    public static i32 operator +(i32 left, i32 right) => unchecked(left.value + right.value);
    
    public static i32 AdditiveIdentity { get; } = 0;
    public static bool operator >(i32 left, i32 right) => left.value > right.value;

    public static bool operator >=(i32 left, i32 right) => left.value >= right.value;

    public static bool operator <(i32 left, i32 right) => left.value < right.value;

    public static bool operator <=(i32 left, i32 right) => left.value <= right.value;

    public static i32 operator --(i32 value) => value - 1;

    public static i32 operator /(i32 left, i32 right) => left.value / right.value;

    public static i32 operator ++(i32 value) => value + 1;
    public static i32 operator *(i32 left, i32 right) => unchecked(left.value * right.value);

    public static i32 operator -(i32 left, i32 right) => unchecked(left.value - right.value);

    public static i32 operator -(i32 value) => unchecked((int)-value.value);

    public static unsafe i32 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static i32 MultiplicativeIdentity { get; } = 1;
    public static int ByteCount => 4;

    public void ToSpan(Span<byte> span) {
        span[0] = unchecked((byte) (value >>> 24));
        span[1] = unchecked((byte) (value >>> 16));
        span[2] = unchecked((byte) (value >>> 8));
        span[3] = unchecked((byte) (value & 0xFF));
    }

    public unsafe void ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));

    public static i32 FromSpan(ReadOnlySpan<byte> bytes) 
        => (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];

    public static i32 MaxValue { get; } = int.MaxValue;
    public static i32 MinValue { get; } = int.MinValue;
}


public readonly record struct i64(long value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<i64>,
    ISizedValue<i64>,
    IBitwiseOperators<i64, i64, i64>,
    IAdditionOperators<i64, i64, i64>,
    ISubtractionOperators<i64, i64, i64>,
    IMultiplyOperators<i64, i64, i64>,
    IDivisionOperators<i64, i64, i64>,
    IModulusOperators<i64, i64, i64>,
    IShiftOperators<i64, i64, i64>,
    IComparisonOperators<i64, i64, bool>,
    IUnaryPlusOperators<i64, i64>,
    IUnaryNegationOperators<i64, i64>,
    IAdditiveIdentity<i64, i64>,
    IMultiplicativeIdentity<i64, i64>,
    IComparable<i64> {
    public static implicit operator i64(long val) => new(val);
    public static implicit operator long(i64 val) => val.value;
    
    public static i64 operator ~(i64 self) => ~self.value;
    public static i64 operator &(i64 a, i64 b) => a.value & b.value;
    public static i64 operator |(i64 a, i64 b) => a.value | b.value;
    public static i64 operator ^(i64 a, i64 b) => a.value ^ b.value;
    public static i64 operator <<(i64 a, i64 b) => unchecked(a.value << (int) b.value);
    public static i64 operator >>(i64 a, i64 b) => unchecked(a.value >> (int) b.value);
    public static i64 operator >>>(i64 a, i64 b) => unchecked(a.value >>> (int) b.value);


    public string Bin => $"{value:B64}";
    public string Hex => $"{value:X16}";
    public string Dec => $"{value:D}";


    public int CompareTo(i64 other) => value.CompareTo(other.value);

    public static i64 operator %(i64 left, i64 right) => left.value % right.value;

    public static i64 operator +(i64 value) => value;
    public static explicit operator char(i64 value) => (char) value.value;
    public override string ToString() => value.ToString();
    public int CompareTo(object? obj) => value.CompareTo(obj);

    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    public static i64 Parse(string s, IFormatProvider? provider) => long.Parse(s, provider);

    public static i64 operator +(i64 left, i64 right) => unchecked(left.value + right.value);
    
    public static i64 AdditiveIdentity { get; } = 0;
    public static bool operator >(i64 left, i64 right) => left.value > right.value;

    public static bool operator >=(i64 left, i64 right) => left.value >= right.value;

    public static bool operator <(i64 left, i64 right) => left.value < right.value;

    public static bool operator <=(i64 left, i64 right) => left.value <= right.value;

    public static i64 operator --(i64 value) => value - 1;

    public static i64 operator /(i64 left, i64 right) => left.value / right.value;

    public static i64 operator ++(i64 value) => value + 1;
    public static i64 operator *(i64 left, i64 right) => unchecked(left.value * right.value);

    public static i64 operator -(i64 left, i64 right) => unchecked(left.value - right.value);
    
    
    
    public static i64 operator -(i64 value) => unchecked(0u - value.value);

    public static unsafe i64 FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));
    public static i64 MultiplicativeIdentity { get; } = 1;
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

    public static i64 FromSpan(ReadOnlySpan<byte> bytes) 
        => unchecked ((long)(((ulong) bytes[0] << 56)
                           | ((ulong) bytes[1] << 48)
                           | ((ulong) bytes[2] << 40)
                           | ((ulong) bytes[3] << 32)
                           | ((ulong) bytes[4] << 24)
                           | ((ulong) bytes[5] << 16)
                           | ((ulong) bytes[6] << 8)
                           | bytes[7]
                           ));
    
}