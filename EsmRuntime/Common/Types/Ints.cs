// ReSharper disable InconsistentNaming

using System.Numerics;
using System.Runtime.CompilerServices;
using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;
using static EsmRuntime.Constants;

// ReSharper disable UseSymbolAlias
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common.Types;

[method: MethodImpl(Inline)]
public readonly record struct i8(sbyte value):
    ISpanFormattable,
    INumberFormattable,
    IAsciiFormattable<i8>,
    ISizedPrimValue<i8>,
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
    IComparable<i8> ,
    IMinMaxValue<i8>
{

    [MethodImpl(Inline)]
    public static implicit operator i8(sbyte val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator sbyte(i8 val) => val.value;
    
    [MethodImpl(Inline)]
    public static i8 operator ~(i8 self) => unchecked((sbyte)~self.value);
    [MethodImpl(Inline)]
    public static i8 operator &(i8 a, i8 b) => unchecked((sbyte)(a.value & b.value));
    [MethodImpl(Inline)]
    public static i8 operator |(i8 a, i8 b) => unchecked((sbyte)(a.value | b.value));
    [MethodImpl(Inline)]
    public static i8 operator ^(i8 a, i8 b) => unchecked((sbyte)(a.value ^ b.value));
    [MethodImpl(Inline)]
    public static i8 operator <<(i8 a, i8 b) => unchecked((sbyte)(a.value << b.value));
    [MethodImpl(Inline)]
    public static i8 operator >>(i8 a, i8 b) => unchecked((sbyte)(a.value >> b.value));
    [MethodImpl(Inline)]
    public static i8 operator >>>(i8 a, i8 b) => unchecked((sbyte)(a.value >>> b.value));

    [MethodImpl(Inline)]
    public static explicit operator char(i8 val) => (char) val.value;
    [MethodImpl(Inline)]
    public static explicit operator i8(char val) => new(unchecked((sbyte)val));
    
    public string Bin {
        [MethodImpl(Inline)]
        get => $"{value:B8}";
    }

    public string Hex {
        [MethodImpl(Inline)]
        get => $"{value:X2}";
    }

    public string Dec {
        [MethodImpl(Inline)]
        get => $"{value:D}";
    }


    [MethodImpl(Inline)]
    public int CompareTo(i8 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static i8 operator %(i8 left, i8 right) => unchecked((sbyte) (left.value % right.value));

    [MethodImpl(Inline)]
    public static i8 operator +(i8 value) => value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static i8 operator +(i8 left, i8 right) => unchecked((sbyte) (left.value + right.value));

    public static i8 AdditiveIdentity {
        [MethodImpl(Inline)]
        get => 0;
    }

    [MethodImpl(Inline)]
    public static bool operator >(i8 left, i8 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(i8 left, i8 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(i8 left, i8 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(i8 left, i8 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static i8 operator --(i8 value) => value - 1;

    [MethodImpl(Inline)]
    public static i8 operator /(i8 left, i8 right) => unchecked((sbyte) (left.value / right.value));

    [MethodImpl(Inline)]
    public static i8 operator ++(i8 value) => value + 1;
    public static i8 MultiplicativeIdentity { [MethodImpl(Inline)] get => 1; }
    [MethodImpl(Inline)]
    public static i8 operator *(i8 left, i8 right) => unchecked((sbyte) (left.value * right.value));

    [MethodImpl(Inline)]
    public static i8 operator -(i8 left, i8 right) => unchecked((sbyte) (left.value - right.value));

    [MethodImpl(Inline)]
    public static i8 operator -(i8 value) => unchecked((sbyte)-value.value);

    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => 1;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Inline)]
        get => true;
    }

    [MethodImpl(Inline)]
    public static unsafe i8 FromPtr(byte* ptr) => ReadUnaligned<sbyte>(ptr);

    [MethodImpl(Inline)]
    public static unsafe i8 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe i8 FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(sbyte);
        return IsLittleEndian ? ReverseEndianness(ReadUnaligned<sbyte>(start)) : ReadUnaligned<sbyte>(start);
    }

    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(sbyte)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }
    
    public static i8 MaxValue { [MethodImpl(Inline)] get => sbyte.MaxValue; }
    
    public static i8 MinValue { [MethodImpl(Inline)] get => sbyte.MinValue; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$i8"u8; }
}


[method: MethodImpl(Inline)]
public readonly record struct i16(short value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<i16>,
    ISizedPrimValue<i16>,
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
    IMinMaxValue<i16>
{
    [MethodImpl(Inline)]
    public static implicit operator i16(short val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator short(i16 val) => val.value;
    
    [MethodImpl(Inline)]
    public static i16 operator ~(i16 self) => unchecked((short)~self.value);
    [MethodImpl(Inline)]
    public static i16 operator &(i16 a, i16 b) => unchecked((short)(a.value & b.value));
    [MethodImpl(Inline)]
    public static i16 operator |(i16 a, i16 b) => unchecked((short)(a.value | b.value));
    [MethodImpl(Inline)]
    public static i16 operator ^(i16 a, i16 b) => unchecked((short)(a.value ^ b.value));
    [MethodImpl(Inline)]
    public static i16 operator <<(i16 a, i16 b) => unchecked((short)(a.value << b.value));
    [MethodImpl(Inline)]
    public static i16 operator >>(i16 a, i16 b) => unchecked((short)(a.value >> b.value));
    [MethodImpl(Inline)]
    public static i16 operator >>>(i16 a, i16 b) => unchecked((short)(a.value >>> b.value));


    public string Bin {
        [MethodImpl(Inline)] get => $"{value:B16}";
    }

    public string Hex {
        [MethodImpl(Inline)] get => $"{value:X4}";
    }

    public string Dec {
        [MethodImpl(Inline)] get => $"{value:D}";
    }


    [MethodImpl(Inline)]
    public int CompareTo(i16 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static i16 operator %(i16 left, i16 right) => unchecked((short) (left.value % right.value));

    [MethodImpl(Inline)]
    public static i16 operator +(i16 value) => value;
    [MethodImpl(Inline)]
    public static explicit operator char(i16 value) => (char) value.value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static i16 operator +(i16 left, i16 right) => unchecked((short) (left.value + right.value));
    
    public static i16 AdditiveIdentity { [MethodImpl(Inline)] get => 0; }
   
    [MethodImpl(Inline)]
    public static bool operator >(i16 left, i16 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(i16 left, i16 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(i16 left, i16 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(i16 left, i16 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static i16 operator --(i16 value) => value - 1;

    [MethodImpl(Inline)]
    public static i16 operator /(i16 left, i16 right) => unchecked((short) (left.value / right.value));

    [MethodImpl(Inline)]
    public static i16 operator ++(i16 value) => value + 1;
    
    [MethodImpl(Inline)]
    public static i16 operator *(i16 left, i16 right) => unchecked((short) (left.value * right.value));

    [MethodImpl(Inline)]
    public static i16 operator -(i16 left, i16 right) => unchecked((short) (left.value - right.value));

    [MethodImpl(Inline)]
    public static i16 operator -(i16 value) => unchecked((short)-value.value);

    public static i16 MultiplicativeIdentity {
        [MethodImpl(Inline)] get => 1;
    }
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => 2;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public static unsafe i16 FromPtr(byte* ptr) => ReadUnaligned<short>(ptr);

    [MethodImpl(Inline)]
    public static unsafe i16 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe i16 FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(short);
        return IsLittleEndian ? ReverseEndianness(ReadUnaligned<short>(start)) : ReadUnaligned<short>(start);
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(short)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }
    
    public static i16 MaxValue { [MethodImpl(Inline)] get => short.MaxValue; }
    
    public static i16 MinValue { [MethodImpl(Inline)] get => short.MinValue; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$i16"u8; }
}


[method: MethodImpl(Inline)]
public readonly record struct i32(int value): 
    ISpanFormattable,
    INumberFormattable,
    ISizedPrimValue<i32>,
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
    IMinMaxValue<i32>
{
    [MethodImpl(Inline)]
    public static implicit operator i32(int val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator int(i32 val) => val.value;
    
    [MethodImpl(Inline)]
    public static i32 operator ~(i32 self) => ~self.value;
    [MethodImpl(Inline)]
    public static i32 operator &(i32 a, i32 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static i32 operator |(i32 a, i32 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static i32 operator ^(i32 a, i32 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static i32 operator <<(i32 a, i32 b) => a.value << b.value;
    [MethodImpl(Inline)]
    public static i32 operator >>(i32 a, i32 b) => a.value >> b.value;
    [MethodImpl(Inline)]
    public static i32 operator >>>(i32 a, i32 b) => a.value >>> b.value;


    public string Bin {
        [MethodImpl(Inline)] get => $"{value:B32}";
    }

    public string Hex {
        [MethodImpl(Inline)] get => $"{value:X8}";
    }

    public string Dec {
        [MethodImpl(Inline)] get => $"{value:D}";
    }


    [MethodImpl(Inline)]
    public int CompareTo(i32 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static i32 operator %(i32 left, i32 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static i32 operator +(i32 value) => value;
    [MethodImpl(Inline)]
    public static explicit operator char(i32 value) => (char) value.value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static i32 operator +(i32 left, i32 right) => unchecked(left.value + right.value);

    public static i32 AdditiveIdentity {
        [MethodImpl(Inline)]
        get => 0;
    }
    [MethodImpl(Inline)]
    public static bool operator >(i32 left, i32 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(i32 left, i32 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(i32 left, i32 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(i32 left, i32 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static i32 operator --(i32 value) => value - 1;

    [MethodImpl(Inline)]
    public static i32 operator /(i32 left, i32 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static i32 operator ++(i32 value) => value + 1;
    [MethodImpl(Inline)]
    public static i32 operator *(i32 left, i32 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static i32 operator -(i32 left, i32 right) => unchecked(left.value - right.value);

    [MethodImpl(Inline)]
    public static i32 operator -(i32 value) => unchecked(-value.value);

    public static i32 MultiplicativeIdentity {
        [MethodImpl(Inline)]
        get => 1;
    }
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => 4;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public static unsafe i32 FromPtr(byte* ptr) => ReadUnaligned<int>(ptr);

    [MethodImpl(Inline)]
    public static unsafe i32 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe i32 FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(int);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<int>(start)) 
            : ReadUnaligned<int>(start);
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(int)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }
    
    public static i32 MaxValue { [MethodImpl(Inline)] get => int.MaxValue; }
    
    public static i32 MinValue { [MethodImpl(Inline)] get => int.MinValue; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$i32"u8; }
}


[method: MethodImpl(Inline)]
public readonly record struct i64(long value): 
    ISpanFormattable,
    INumberFormattable,
    ISizedPrimValue<i64>,
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
    IComparable<i64>,
    IMinMaxValue<i64> {
    
    [MethodImpl(Inline)]
    public static implicit operator i64(long val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator long(i64 val) => val.value;
    
    [MethodImpl(Inline)]
    public static i64 operator ~(i64 self) => ~self.value;
    [MethodImpl(Inline)]
    public static i64 operator &(i64 a, i64 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static i64 operator |(i64 a, i64 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static i64 operator ^(i64 a, i64 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static i64 operator <<(i64 a, i64 b) => unchecked(a.value << (int) b.value);
    [MethodImpl(Inline)]
    public static i64 operator >>(i64 a, i64 b) => unchecked(a.value >> (int) b.value);
    [MethodImpl(Inline)]
    public static i64 operator >>>(i64 a, i64 b) => unchecked(a.value >>> (int) b.value);


    public string Bin {
        [MethodImpl(Inline)] get => $"{value:B64}";
    }

    public string Hex {
        [MethodImpl(Inline)] get => $"{value:X16}";
    }

    public string Dec {
        [MethodImpl(Inline)] get => $"{value:D}";
    }

    [MethodImpl(Inline)]
    public int CompareTo(i64 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static i64 operator %(i64 left, i64 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static i64 operator +(i64 value) => value;
    
    [MethodImpl(Inline)]
    public static explicit operator char(i64 value) => (char) value.value;
    
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(Inline)]
    public static i64 operator +(i64 left, i64 right) => unchecked(left.value + right.value);
    
    public static i64 AdditiveIdentity { [MethodImpl(Inline)] get => 0; }
    [MethodImpl(Inline)]
    public static bool operator >(i64 left, i64 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(i64 left, i64 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(i64 left, i64 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(i64 left, i64 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static i64 operator --(i64 value) => value - 1;

    [MethodImpl(Inline)]
    public static i64 operator /(i64 left, i64 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static i64 operator ++(i64 value) => value + 1;
    
    [MethodImpl(Inline)]
    public static i64 operator *(i64 left, i64 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static i64 operator -(i64 left, i64 right) => unchecked(left.value - right.value);
    
    [MethodImpl(Inline)]
    public static i64 operator -(i64 value) => unchecked(-value.value);

    public static i64 MultiplicativeIdentity {
        [MethodImpl(Inline)] get => 1;
    }
    
    public static usize ByteCount {
        [MethodImpl(Inline)] get => 8;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public static unsafe i64 FromPtr(byte* ptr) => ReadUnaligned<long>(ptr);

    [MethodImpl(Inline)]
    public static unsafe i64 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe i64 FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(long);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<long>(start)) 
            : ReadUnaligned<long>(start);
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(long)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }
    
    
    public static i64 MaxValue { [MethodImpl(Inline)] get => long.MaxValue; }
    
    public static i64 MinValue { [MethodImpl(Inline)] get => long.MinValue; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$i64"u8; }
}

[method: MethodImpl(Inline)]
public readonly record struct i128(Int128 value): 
    ISpanFormattable,
    INumberFormattable,
    ISizedPrimValue<i128>,
    IBitwiseOperators<i128, i128, i128>,
    IAdditionOperators<i128, i128, i128>,
    ISubtractionOperators<i128, i128, i128>,
    IMultiplyOperators<i128, i128, i128>,
    IDivisionOperators<i128, i128, i128>,
    IModulusOperators<i128, i128, i128>,
    IShiftOperators<i128, i128, i128>,
    IComparisonOperators<i128, i128, bool>,
    IUnaryPlusOperators<i128, i128>,
    IUnaryNegationOperators<i128, i128>,
    IAdditiveIdentity<i128, i128>,
    IMultiplicativeIdentity<i128, i128>,
    IComparable<i128>,
    IMinMaxValue<i128> {
    
    [MethodImpl(Inline)]
    public static implicit operator i128(Int128 val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator Int128(i128 val) => val.value;
    
    [MethodImpl(Inline)]
    public static i128 operator ~(i128 self) => ~self.value;
    [MethodImpl(Inline)]
    public static i128 operator &(i128 a, i128 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static i128 operator |(i128 a, i128 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static i128 operator ^(i128 a, i128 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static i128 operator <<(i128 a, i128 b) => unchecked(a.value << (int) b.value);
    [MethodImpl(Inline)]
    public static i128 operator >>(i128 a, i128 b) => unchecked(a.value >> (int) b.value);
    [MethodImpl(Inline)]
    public static i128 operator >>>(i128 a, i128 b) => unchecked(a.value >>> (int) b.value);


    public string Bin {
        [MethodImpl(Inline)] get => $"{value:B64}";
    }

    public string Hex {
        [MethodImpl(Inline)] get => $"{value:X16}";
    }

    public string Dec {
        [MethodImpl(Inline)] get => $"{value:D}";
    }

    [MethodImpl(Inline)]
    public int CompareTo(i128 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static i128 operator %(i128 left, i128 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static i128 operator +(i128 value) => value;
    
    [MethodImpl(Inline)]
    public static explicit operator char(i128 value) => (char) value.value;
    
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(Inline)]
    public static i128 operator +(i128 left, i128 right) => unchecked(left.value + right.value);
    
    public static i128 AdditiveIdentity {
        [MethodImpl(Inline)] get => Int128.Zero;
    }
    [MethodImpl(Inline)]
    public static bool operator >(i128 left, i128 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(i128 left, i128 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(i128 left, i128 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(i128 left, i128 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static i128 operator --(i128 value) => (Int128) value - 1;

    [MethodImpl(Inline)]
    public static i128 operator /(i128 left, i128 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static i128 operator ++(i128 value) => (Int128) value + 1;
    
    [MethodImpl(Inline)]
    public static i128 operator *(i128 left, i128 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static i128 operator -(i128 left, i128 right) => unchecked(left.value - right.value);
    
    [MethodImpl(Inline)]
    public static i128 operator -(i128 value) => unchecked(-value.value);

    public static i128 MultiplicativeIdentity {
        [MethodImpl(Inline)] get => (Int128) 1;
    }
    
    public static usize ByteCount {
        [MethodImpl(Inline)] get => 8;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public static unsafe i128 FromPtr(byte* ptr) => ReadUnaligned<Int128>(ptr);

    [MethodImpl(Inline)]
    public static unsafe i128 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe i128 FromBytecode(byte* start, scoped ref nuint pc) {
        pc += (nuint)sizeof(Int128);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<Int128>(start)) 
            : ReadUnaligned<Int128>(start);
    }
    
    [MethodImpl(Inline)]
    public unsafe byte[] ToBytecode() {
        var arr = new byte[sizeof(Int128)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }
    
    public static i128 MaxValue { [MethodImpl(Inline)] get => Int128.MaxValue; }
    
    public static i128 MinValue { [MethodImpl(Inline)] get => Int128.MinValue; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$i128"u8; }
}