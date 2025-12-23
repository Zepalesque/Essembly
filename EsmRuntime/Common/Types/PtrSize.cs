using System.Numerics;
using System.Runtime.CompilerServices;
using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;
using static EsmRuntime.Constants;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public readonly record struct usize(nuint value):
    ISpanFormattable,
    ISizedValue<usize>,
    INumberFormattable,
    IBitwiseOperators<usize, usize, usize>,
    IAdditionOperators<usize, usize, usize>,
    ISubtractionOperators<usize, usize, usize>,
    IMultiplyOperators<usize, usize, usize>,
    IDivisionOperators<usize, usize, usize>,
    IModulusOperators<usize, usize, usize>,
    IShiftOperators<usize, usize, usize>,
    IComparisonOperators<usize, usize, bool>,
    IUnaryPlusOperators<usize, usize>,
    IUnaryNegationOperators<usize, usize>,
    IAdditiveIdentity<usize, usize>,
    IMultiplicativeIdentity<usize, usize>,
    IComparable<usize>,
    IMinMaxValue<usize>
{
    [MethodImpl(Inline)]
    public static implicit operator nuint(usize self) => EsmVM.MemAddrUSize + self.value;
    [MethodImpl(Inline)]
    public static implicit operator usize(nuint self) => new(self - EsmVM.MemAddrUSize);    

    [MethodImpl(Inline)]
    public static implicit operator int(usize self) => (int) self.value;
    [MethodImpl(Inline)]
    public static implicit operator usize(int self) => new((nuint)self);    

    
    [MethodImpl(Inline)]
    public static implicit operator long(usize self) => (long) self.value;
    [MethodImpl(Inline)]
    public static implicit operator usize(long self) => new((nuint)self);    

    
    [MethodImpl(Inline)]
    public static implicit operator uint(usize self) => (uint) self.value;
    [MethodImpl(Inline)]
    public static implicit operator usize(uint self) => new(self);  
    
    [MethodImpl(Inline)]
    public static implicit operator ulong(usize self) => self.value;
    [MethodImpl(Inline)]
    public static implicit operator usize(ulong self) => new((nuint) self);  
    
    [MethodImpl(Inline)]
    public static unsafe explicit operator byte*(usize self) => (byte*) (nuint) self;
    [MethodImpl(Inline)]
    public static unsafe explicit operator usize(byte* self) => (nuint) self;

    [MethodImpl(Inline)]
    public static unsafe usize FromBytecode(byte* start, int* pc) {
        *pc += sizeof(ulong);
        return IsLittleEndian
            ? ReverseEndianness(ReadUnaligned<ulong>(start))
            : ReadUnaligned<ulong>(start);
    }

    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(ulong)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) {
        WriteUnaligned(ptr, value);
    }

    [MethodImpl(Inline)]
    public static unsafe usize FromFatPtr(byte* ptr, usize size) => FromPtr(ptr);

    [MethodImpl(Inline)]
    public static unsafe usize FromPtr(byte* ptr) => ReadUnaligned<nuint>(ptr);
    
    public static unsafe usize ByteCount {
        [MethodImpl(Inline)]
        get => sizeof(nuint);
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => value.TryFormat(destination, out charsWritten, format, provider);

    public string Bin {
        [MethodImpl(Inline)]
        get => nuint.Size switch {
            4 => $"{value:B32}",
            8 => $"{value:B64}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Hex {
        [MethodImpl(Inline)]
        get => nuint.Size switch {
            4 => $"{value:X8}",
            8 => $"{value:X16}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Dec {
        [MethodImpl(Inline)]
        get => $"{value:D}";
    }


    [MethodImpl(Inline)]
    public static usize operator &(usize left, usize right) => new(left.value & right.value);

    [MethodImpl(Inline)]
    public static usize operator |(usize left, usize right) => new(left.value | right.value);

    [MethodImpl(Inline)]
    public static usize operator ^(usize left, usize right) => new(left.value ^ right.value);

    [MethodImpl(Inline)]
    public static usize operator ~(usize value) => new(~value.value);

    [MethodImpl(Inline)]
    public static usize operator +(usize left, usize right) => new(left.value + right.value);

    [MethodImpl(Inline)]
    public static usize operator -(usize left, usize right) => new(left.value - right.value);

    [MethodImpl(Inline)]
    public static usize operator *(usize left, usize right) => new(left.value * right.value);

    [MethodImpl(Inline)]
    public static usize operator /(usize left, usize right) => new(left.value / right.value);

    [MethodImpl(Inline)]
    public static usize operator %(usize left, usize right) => new(left.value % right.value);

    [MethodImpl(Inline)]
    public static usize operator <<(usize value, usize shiftAmount) => new(value.value << (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static usize operator >> (usize value, usize shiftAmount) => new(value.value >> (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static usize operator >>> (usize value, usize shiftAmount) => new(value.value >>> (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static bool operator >(usize left, usize right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(usize left, usize right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(usize left, usize right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(usize left, usize right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static usize operator +(usize value) => value;
    
    [MethodImpl(Inline)]
    public static usize operator -(usize value) => new(~value.value + 1);

    public static usize AdditiveIdentity { get; } = 0;
    public static usize MultiplicativeIdentity { get; } = 1;
    [MethodImpl(Inline)]
    public int CompareTo(usize other) => value.CompareTo(other.value);
    public static usize MaxValue => nuint.MaxValue;
    public static usize MinValue => nuint.MinValue;
}

public readonly record struct isize(nint value):
    ISpanFormattable,
    ISizedValue<isize>,
    INumberFormattable,
    IBitwiseOperators<isize, isize, isize>,
    IAdditionOperators<isize, isize, isize>,
    ISubtractionOperators<isize, isize, isize>,
    IMultiplyOperators<isize, isize, isize>,
    IDivisionOperators<isize, isize, isize>,
    IModulusOperators<isize, isize, isize>,
    IShiftOperators<isize, isize, isize>,
    IComparisonOperators<isize, isize, bool>,
    IUnaryPlusOperators<isize, isize>,
    IUnaryNegationOperators<isize, isize>,
    IAdditiveIdentity<isize, isize>,
    IMultiplicativeIdentity<isize, isize>,
    IComparable<isize>,
    IMinMaxValue<isize>
{
    [MethodImpl(Inline)]
    public static implicit operator nint(isize self) => EsmVM.MemAddrISize + self.value;
    [MethodImpl(Inline)]
    public static implicit operator isize(nint self) => new(self - EsmVM.MemAddrISize);    

    [MethodImpl(Inline)]
    public static implicit operator int(isize self) => (int) self.value;
    [MethodImpl(Inline)]
    public static implicit operator isize(int self) => new(self);
    
    [MethodImpl(Inline)]
    public static implicit operator long(isize self) => self.value;
    [MethodImpl(Inline)]
    public static implicit operator isize(long self) => new((nint) self);  
    
    [MethodImpl(Inline)]
    public static unsafe explicit operator byte*(isize self) => (byte*) (nint) self;
    [MethodImpl(Inline)]
    public static unsafe explicit operator isize(byte* self) => (nint) self;

    [MethodImpl(Inline)]
    public static unsafe isize FromBytecode(byte* start, int* pc) {
        *pc += sizeof(long);
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

    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) {
        WriteUnaligned(ptr, value);
    }

    [MethodImpl(Inline)]
    public static unsafe isize FromFatPtr(byte* ptr, usize size) => FromPtr(ptr);

    [MethodImpl(Inline)]
    public static unsafe isize FromPtr(byte* ptr) => ReadUnaligned<nint>(ptr);
    
    public static unsafe usize ByteCount {
        [MethodImpl(Inline)]
        get => sizeof(nint);
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => value.ToString(format, formatProvider);

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => value.TryFormat(destination, out charsWritten, format, provider);

    public string Bin {
        [MethodImpl(Inline)]
        get => nint.Size switch {
            4 => $"{value:B32}",
            8 => $"{value:B64}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Hex {
        [MethodImpl(Inline)]
        get => nint.Size switch {
            4 => $"{value:X8}",
            8 => $"{value:X16}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Dec {
        [MethodImpl(Inline)]
        get => $"{value:D}";
    }


    [MethodImpl(Inline)]
    public static isize operator &(isize left, isize right) => new(left.value & right.value);

    [MethodImpl(Inline)]
    public static isize operator |(isize left, isize right) => new(left.value | right.value);

    [MethodImpl(Inline)]
    public static isize operator ^(isize left, isize right) => new(left.value ^ right.value);

    [MethodImpl(Inline)]
    public static isize operator ~(isize value) => new(~value.value);

    [MethodImpl(Inline)]
    public static isize operator +(isize left, isize right) => new(left.value + right.value);

    [MethodImpl(Inline)]
    public static isize operator -(isize left, isize right) => new(left.value - right.value);

    [MethodImpl(Inline)]
    public static isize operator *(isize left, isize right) => new(left.value * right.value);

    [MethodImpl(Inline)]
    public static isize operator /(isize left, isize right) => new(left.value / right.value);

    [MethodImpl(Inline)]
    public static isize operator %(isize left, isize right) => new(left.value % right.value);

    [MethodImpl(Inline)]
    public static isize operator <<(isize value, isize shiftAmount) => new(value.value << (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static isize operator >> (isize value, isize shiftAmount) => new(value.value >> (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static isize operator >>> (isize value, isize shiftAmount) => new(value.value >>> (int) shiftAmount.value);

    [MethodImpl(Inline)]
    public static bool operator >(isize left, isize right) => left.value > right.value;

    [MethodImpl(Inline)]
    public static bool operator >=(isize left, isize right) => left.value >= right.value;

    [MethodImpl(Inline)]
    public static bool operator <(isize left, isize right) => left.value < right.value;

    [MethodImpl(Inline)]
    public static bool operator <=(isize left, isize right) => left.value <= right.value;

    [MethodImpl(Inline)]
    public static isize operator +(isize value) => value;
    
    [MethodImpl(Inline)]
    public static isize operator -(isize value) => new(~value.value + 1);

    public static isize AdditiveIdentity { get; } = 0;
    public static isize MultiplicativeIdentity { get; } = 1;
    [MethodImpl(Inline)]
    public int CompareTo(isize other) => value.CompareTo(other.value);
    public static isize MaxValue => nint.MaxValue;
    public static isize MinValue => nint.MinValue;
}