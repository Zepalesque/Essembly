// ReSharper disable InconsistentNaming

using System.Numerics;
using System.Runtime.CompilerServices;
using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives;
using static System.Runtime.CompilerServices.Unsafe;
using static EsmRuntime.Constants;

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
    ISubtractionOperators<u8, u8, u8>,
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

    [MethodImpl(Inline)]
    public static implicit operator u8(byte val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator byte(u8 val) => val.value;
    
    [MethodImpl(Inline)]
    public static u8 operator ~(u8 self) => unchecked((byte)~self.value);
    [MethodImpl(Inline)]
    public static u8 operator &(u8 a, u8 b) => unchecked((byte)(a.value & b.value));
    [MethodImpl(Inline)]
    public static u8 operator |(u8 a, u8 b) => unchecked((byte)(a.value | b.value));
    [MethodImpl(Inline)]
    public static u8 operator ^(u8 a, u8 b) => unchecked((byte)(a.value ^ b.value));
    [MethodImpl(Inline)]
    public static u8 operator <<(u8 a, u8 b) => unchecked((byte)(a.value << b.value));
    [MethodImpl(Inline)]
    public static u8 operator >>(u8 a, u8 b) => unchecked((byte)(a.value >> b.value));
    [MethodImpl(Inline)]
    public static u8 operator >>>(u8 a, u8 b) => unchecked((byte)(a.value >>> b.value));

    [MethodImpl(Inline)]
    public static explicit operator char(u8 val) => (char) val.value;
    [MethodImpl(Inline)]
    public static explicit operator u8(char val) => new(unchecked((byte)val));
    
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
    public int CompareTo(u8 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static u8 operator %(u8 left, u8 right) => unchecked((byte) (left.value % right.value));

    [MethodImpl(Inline)]
    public static u8 operator +(u8 value) => value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static u8 operator +(u8 left, u8 right) => unchecked((byte) (left.value + right.value));

    public static u8 AdditiveIdentity {
        [MethodImpl(Inline)]
        get;
    } = 0;

    [MethodImpl(Inline)]
    public static bool operator >(u8 left, u8 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(u8 left, u8 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(u8 left, u8 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(u8 left, u8 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static u8 operator --(u8 value) => value - 1;

    [MethodImpl(Inline)]
    public static u8 operator /(u8 left, u8 right) => unchecked((byte) (left.value / right.value));

    [MethodImpl(Inline)]
    public static u8 operator ++(u8 value) => value + 1;
    public static u8 MultiplicativeIdentity { get; } = 1;
    [MethodImpl(Inline)]
    public static u8 operator *(u8 left, u8 right) => unchecked((byte) (left.value * right.value));

    [MethodImpl(Inline)]
    public static u8 operator -(u8 left, u8 right) => unchecked((byte) (left.value - right.value));

    [MethodImpl(Inline)]
    public static u8 operator -(u8 value) => unchecked((byte)-value.value);

    public static usize ByteCount {
        [MethodImpl(Inline)]
        get;
    } = 1;

    [MethodImpl(Inline)]
    public static unsafe u8 FromPtr(byte* ptr) => ReadUnaligned<byte>(ptr);

    [MethodImpl(Inline)]
    public static unsafe u8 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe u8 FromBytecode(byte* start, int* pc) {
        *pc += sizeof(byte);
        return IsLittleEndian ? ReverseEndianness(ReadUnaligned<byte>(start)) : ReadUnaligned<byte>(start);
    }

    [MethodImpl(Inline)]
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator(sizeof(byte));
        WriteUnaligned(ptr, IsLittleEndian ? ReverseEndianness(value) : value);
        return new(ptr, sizeof(byte));
    }
}


public readonly record struct u16(ushort value): 
    ISpanFormattable,
    INumberFormattable,
    IUtf16Formattable<u16>,
    ISizedValue<u16>,
    IBitwiseOperators<u16, u16, u16>,
    IAdditionOperators<u16, u16, u16>,
    ISubtractionOperators<u16, u16, u16>,
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
    [MethodImpl(Inline)]
    public static implicit operator u16(ushort val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator ushort(u16 val) => val.value;
    
    [MethodImpl(Inline)]
    public static u16 operator ~(u16 self) => unchecked((ushort)~self.value);
    [MethodImpl(Inline)]
    public static u16 operator &(u16 a, u16 b) => unchecked((ushort)(a.value & b.value));
    [MethodImpl(Inline)]
    public static u16 operator |(u16 a, u16 b) => unchecked((ushort)(a.value | b.value));
    [MethodImpl(Inline)]
    public static u16 operator ^(u16 a, u16 b) => unchecked((ushort)(a.value ^ b.value));
    [MethodImpl(Inline)]
    public static u16 operator <<(u16 a, u16 b) => unchecked((ushort)(a.value << b.value));
    [MethodImpl(Inline)]
    public static u16 operator >>(u16 a, u16 b) => unchecked((ushort)(a.value >> b.value));
    [MethodImpl(Inline)]
    public static u16 operator >>>(u16 a, u16 b) => unchecked((ushort)(a.value >>> b.value));


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
    public int CompareTo(u16 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static u16 operator %(u16 left, u16 right) => unchecked((ushort) (left.value % right.value));

    [MethodImpl(Inline)]
    public static u16 operator +(u16 value) => value;
    [MethodImpl(Inline)]
    public static explicit operator char(u16 value) => (char) value.value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static u16 operator +(u16 left, u16 right) => unchecked((ushort) (left.value + right.value));
    
    public static u16 AdditiveIdentity { [MethodImpl(Inline)] get; } = 0;
   
    [MethodImpl(Inline)]
    public static bool operator >(u16 left, u16 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(u16 left, u16 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(u16 left, u16 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(u16 left, u16 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static u16 operator --(u16 value) => value - 1;

    [MethodImpl(Inline)]
    public static u16 operator /(u16 left, u16 right) => unchecked((ushort) (left.value / right.value));

    [MethodImpl(Inline)]
    public static u16 operator ++(u16 value) => value + 1;
    
    [MethodImpl(Inline)]
    public static u16 operator *(u16 left, u16 right) => unchecked((ushort) (left.value * right.value));

    [MethodImpl(Inline)]
    public static u16 operator -(u16 left, u16 right) => unchecked((ushort) (left.value - right.value));

    [MethodImpl(Inline)]
    public static u16 operator -(u16 value) => unchecked((ushort)-value.value);

    public static u16 MultiplicativeIdentity {
        [MethodImpl(Inline)] get;
    } = 1;
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get;
    } = 2;

    [MethodImpl(Inline)]
    public static unsafe u16 FromPtr(byte* ptr) => ReadUnaligned<ushort>(ptr);

    [MethodImpl(Inline)]
    public static unsafe u16 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe u16 FromBytecode(byte* start, int* pc) {
        *pc += sizeof(ushort);
        return IsLittleEndian ? ReverseEndianness(ReadUnaligned<ushort>(start)) : ReadUnaligned<ushort>(start);
    }
    
    [MethodImpl(Inline)]
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator(sizeof(ushort));
        WriteUnaligned(ptr, IsLittleEndian ? ReverseEndianness(value) : value);
        return new(ptr, sizeof(ushort));
    }

    public static u16 MaxValue {
        [MethodImpl(Inline)]
        get;
    } = ushort.MaxValue;

    public static u16 MinValue {
        [MethodImpl(Inline)]
        get;
    } = ushort.MinValue;
}


public readonly record struct u32(uint value): 
    ISpanFormattable,
    INumberFormattable,
    ISizedValue<u32>,
    IBitwiseOperators<u32, u32, u32>,
    IAdditionOperators<u32, u32, u32>,
    ISubtractionOperators<u32, u32, u32>,
    IMultiplyOperators<u32, u32, u32>,
    IDivisionOperators<u32, u32, u32>,
    IModulusOperators<u32, u32, u32>,
    IShiftOperators<u32, u32, u32>,
    IComparisonOperators<u32, u32, bool>,
    IUnaryPlusOperators<u32, u32>,
    IUnaryNegationOperators<u32, u32>,
    IAdditiveIdentity<u32, u32>,
    IMultiplicativeIdentity<u32, u32>,
    IComparable<u32>,
    IMinMaxValue<u32>
{
    [MethodImpl(Inline)]
    public static implicit operator u32(uint val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator uint(u32 val) => val.value;
    
    [MethodImpl(Inline)]
    public static u32 operator ~(u32 self) => ~self.value;
    [MethodImpl(Inline)]
    public static u32 operator &(u32 a, u32 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static u32 operator |(u32 a, u32 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static u32 operator ^(u32 a, u32 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static u32 operator <<(u32 a, u32 b) => unchecked(a.value << (int) b.value);
    [MethodImpl(Inline)]
    public static u32 operator >>(u32 a, u32 b) => unchecked(a.value >> (int) b.value);
    [MethodImpl(Inline)]
    public static u32 operator >>>(u32 a, u32 b) => unchecked(a.value >>> (int) b.value);


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
    public int CompareTo(u32 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static u32 operator %(u32 left, u32 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static u32 operator +(u32 value) => value;
    [MethodImpl(Inline)]
    public static explicit operator char(u32 value) => (char) value.value;
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);
    
    [MethodImpl(Inline)]
    public static u32 operator +(u32 left, u32 right) => unchecked(left.value + right.value);

    public static u32 AdditiveIdentity {
        [MethodImpl(Inline)]
        get;
    } = 0;
    [MethodImpl(Inline)]
    public static bool operator >(u32 left, u32 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(u32 left, u32 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(u32 left, u32 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(u32 left, u32 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static u32 operator --(u32 value) => value - 1;

    [MethodImpl(Inline)]
    public static u32 operator /(u32 left, u32 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static u32 operator ++(u32 value) => value + 1;
    [MethodImpl(Inline)]
    public static u32 operator *(u32 left, u32 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static u32 operator -(u32 left, u32 right) => unchecked(left.value - right.value);

    [MethodImpl(Inline)]
    public static u32 operator -(u32 value) => unchecked((uint)-value.value);

    public static u32 MultiplicativeIdentity {
        [MethodImpl(Inline)]
        get;
    } = 1;
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get;
    } = 4;

    [MethodImpl(Inline)]
    public static unsafe u32 FromPtr(byte* ptr) => ReadUnaligned<uint>(ptr);

    [MethodImpl(Inline)]
    public static unsafe u32 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe u32 FromBytecode(byte* start, int* pc) {
        *pc += sizeof(uint);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<uint>(start)) 
            : ReadUnaligned<uint>(start);
    }
    
    [MethodImpl(Inline)]
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator(sizeof(uint));
        WriteUnaligned(ptr, IsLittleEndian ? ReverseEndianness(value) : value);
        return new(ptr, sizeof(uint));
    }
    
    public static u32 MaxValue {
        [MethodImpl(Inline)]
        get;
    } = uint.MaxValue;

    public static u32 MinValue {
        [MethodImpl(Inline)]
        get;
    } = uint.MinValue;
}


public readonly record struct u64(ulong value): 
    ISpanFormattable,
    INumberFormattable,
    ISizedValue<u64>,
    IBitwiseOperators<u64, u64, u64>,
    IAdditionOperators<u64, u64, u64>,
    ISubtractionOperators<u64, u64, u64>,
    IMultiplyOperators<u64, u64, u64>,
    IDivisionOperators<u64, u64, u64>,
    IModulusOperators<u64, u64, u64>,
    IShiftOperators<u64, u64, u64>,
    IComparisonOperators<u64, u64, bool>,
    IUnaryPlusOperators<u64, u64>,
    IUnaryNegationOperators<u64, u64>,
    IAdditiveIdentity<u64, u64>,
    IMultiplicativeIdentity<u64, u64>,
    IComparable<u64>,
    IMinMaxValue<u64> {
    
    [MethodImpl(Inline)]
    public static implicit operator u64(ulong val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator ulong(u64 val) => val.value;
    
    [MethodImpl(Inline)]
    public static u64 operator ~(u64 self) => ~self.value;
    [MethodImpl(Inline)]
    public static u64 operator &(u64 a, u64 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static u64 operator |(u64 a, u64 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static u64 operator ^(u64 a, u64 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static u64 operator <<(u64 a, u64 b) => unchecked(a.value << (int) b.value);
    [MethodImpl(Inline)]
    public static u64 operator >>(u64 a, u64 b) => unchecked(a.value >> (int) b.value);
    [MethodImpl(Inline)]
    public static u64 operator >>>(u64 a, u64 b) => unchecked(a.value >>> (int) b.value);


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
    public int CompareTo(u64 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static u64 operator %(u64 left, u64 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static u64 operator +(u64 value) => value;
    
    [MethodImpl(Inline)]
    public static explicit operator char(u64 value) => (char) value.value;
    
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(Inline)]
    public static u64 operator +(u64 left, u64 right) => unchecked(left.value + right.value);
    
    public static u64 AdditiveIdentity { [MethodImpl(Inline)] get; } = 0;
    [MethodImpl(Inline)]
    public static bool operator >(u64 left, u64 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(u64 left, u64 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(u64 left, u64 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(u64 left, u64 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static u64 operator --(u64 value) => value - 1;

    [MethodImpl(Inline)]
    public static u64 operator /(u64 left, u64 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static u64 operator ++(u64 value) => value + 1;
    
    [MethodImpl(Inline)]
    public static u64 operator *(u64 left, u64 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static u64 operator -(u64 left, u64 right) => unchecked(left.value - right.value);
    
    [MethodImpl(Inline)]
    public static u64 operator -(u64 value) => unchecked(~value.value + 1);

    public static u64 MultiplicativeIdentity {
        [MethodImpl(Inline)]
        get;
    } = 1;
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get;
    } = 8;

    [MethodImpl(Inline)]
    public static unsafe u64 FromPtr(byte* ptr) => ReadUnaligned<ulong>(ptr);

    [MethodImpl(Inline)]
    public static unsafe u64 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe u64 FromBytecode(byte* start, int* pc) {
        *pc += sizeof(ulong);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<ulong>(start)) 
            : ReadUnaligned<ulong>(start);
    }
    
    [MethodImpl(Inline)]
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator(sizeof(ulong));
        WriteUnaligned(ptr, IsLittleEndian ? ReverseEndianness(value) : value);
        return new(ptr, sizeof(ulong));
    }


    public static u64 MaxValue {
        [MethodImpl(Inline)]
        get;
    } = ulong.MaxValue;

    public static u64 MinValue {
        [MethodImpl(Inline)]
        get;
    } = ulong.MinValue;
}


/*public readonly record struct u128(UInt128 value):
    ISpanFormattable,
    INumberFormattable,
    ISizedValue<u128>,
    IBitwiseOperators<u128, u128, u128>,
    IAdditionOperators<u128, u128, u128>,
    ISubtractionOperators<u128, u128, u128>,
    IMultiplyOperators<u128, u128, u128>,
    IDivisionOperators<u128, u128, u128>,
    IModulusOperators<u128, u128, u128>,
    IShiftOperators<u128, u128, u128>,
    IComparisonOperators<u128, u128, bool>,
    IUnaryPlusOperators<u128, u128>,
    IUnaryNegationOperators<u128, u128>,
    IAdditiveIdentity<u128, u128>,
    IMultiplicativeIdentity<u128, u128>,
    IComparable<u128>,
    IMinMaxValue<u128> {
    
    [MethodImpl(Inline)]
    public static implicit operator u128(UInt128 val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator UInt128(u128 val) => val.value;
    
    [MethodImpl(Inline)]
    public static u128 operator ~(u128 self) => ~self.value;
    [MethodImpl(Inline)]
    public static u128 operator &(u128 a, u128 b) => a.value & b.value;
    [MethodImpl(Inline)]
    public static u128 operator |(u128 a, u128 b) => a.value | b.value;
    [MethodImpl(Inline)]
    public static u128 operator ^(u128 a, u128 b) => a.value ^ b.value;
    [MethodImpl(Inline)]
    public static u128 operator <<(u128 a, u128 b) => unchecked(a.value << (int) b.value);
    [MethodImpl(Inline)]
    public static u128 operator >>(u128 a, u128 b) => unchecked(a.value >> (int) b.value);
    [MethodImpl(Inline)]
    public static u128 operator >>>(u128 a, u128 b) => unchecked(a.value >>> (int) b.value);


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
    public int CompareTo(u128 other) => value.CompareTo(other.value);

    [MethodImpl(Inline)]
    public static u128 operator %(u128 left, u128 right) => left.value % right.value;

    [MethodImpl(Inline)]
    public static u128 operator +(u128 value) => value;
    
    [MethodImpl(Inline)]
    public override string ToString() => value.ToString();
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) 
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) 
        => value.TryFormat(destination, out charsWritten, format, provider);

    [MethodImpl(Inline)]
    public static u128 operator +(u128 left, u128 right) => unchecked(left.value + right.value);
    
    public static u128 AdditiveIdentity { [MethodImpl(Inline)] get; } = (UInt128)0;
    [MethodImpl(Inline)]
    public static bool operator >(u128 left, u128 right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(u128 left, u128 right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(u128 left, u128 right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(u128 left, u128 right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static u128 operator --(u128 value) => (UInt128)value - 1;

    [MethodImpl(Inline)]
    public static u128 operator /(u128 left, u128 right) => left.value / right.value;

    [MethodImpl(Inline)]
    public static u128 operator ++(u128 value) => (UInt128)value + 1;
    
    [MethodImpl(Inline)]
    public static u128 operator *(u128 left, u128 right) => unchecked(left.value * right.value);

    [MethodImpl(Inline)]
    public static u128 operator -(u128 left, u128 right) => unchecked(left.value - right.value);
    
    [MethodImpl(Inline)]
    public static u128 operator -(u128 value) => unchecked(~value.value + 1);

    public static u128 MultiplicativeIdentity {
        [MethodImpl(Inline)]
        get;
    } = (UInt128) 1;
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get;
    } = 8;

    [MethodImpl(Inline)]
    public static unsafe u128 FromPtr(byte* ptr) => ReadUnaligned<UInt128>(ptr);

    [MethodImpl(Inline)]
    public static unsafe u128 FromFatPtr(byte* ptr, usize size) 
        => FromPtr(ptr);

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => WriteUnaligned(ptr, value);

    [MethodImpl(Inline)]
    public static unsafe u128 FromBytecode(byte* start, int* pc) {
        *pc += sizeof(UInt128);
        return IsLittleEndian 
            ? ReverseEndianness(ReadUnaligned<UInt128>(start)) 
            : ReadUnaligned<UInt128>(start);
    }
    
    [MethodImpl(Inline)]
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator((nuint) sizeof(UInt128));
        WriteUnaligned(ptr, IsLittleEndian ? ReverseEndianness(value) : value);
        return new(ptr, (nuint) sizeof(UInt128));
    }


    public static u128 MaxValue {
        [MethodImpl(Inline)]
        get;
    } = UInt128.MaxValue;

    public static u128 MinValue {
        [MethodImpl(Inline)]
        get;
    } = UInt128.MinValue;
}*/