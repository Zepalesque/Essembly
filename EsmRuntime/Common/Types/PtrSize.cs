using System.Numerics;
using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;
using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public readonly record struct usize(nuint value):
    ISpanFormattable,
    ISizedPrimValue<usize>,
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
    [MethodImpl(Utils.Inline)]
    public static implicit operator nuint(usize self) => EsmVM.MemAddrUSize + self.value;
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(nuint self) => new(self - EsmVM.MemAddrUSize);
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator nint(usize self) => (isize) self;
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(usize self) => new(unchecked((nint) self.value));
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(nint self) => (isize) self;
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(int self) => new((nuint)self);
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(long self) => new((nuint)unchecked((ulong)self));    
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(uint self) => new(self);  
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(ulong self) => new((nuint) self);  
    
    [MethodImpl(Utils.Inline)]
    public static unsafe explicit operator byte*(usize self) => (byte*) (nuint) self;
    [MethodImpl(Utils.Inline)]
    public static unsafe explicit operator usize(byte* self) => (nuint) self;

    [MethodImpl(Utils.Inline)]
    public static unsafe usize FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(ulong);
        return IsLittleEndian
            ? ReverseEndianness(ReadUnaligned<ulong>(start))
            : ReadUnaligned<ulong>(start);
    }

    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(ulong)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }

    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr) {
        WriteUnaligned(ptr, value);
    }

    [MethodImpl(Utils.Inline)]
    public static unsafe usize FromFatPtr(byte* ptr, usize size) => FromPtr(ptr);

    [MethodImpl(Utils.Inline)]
    public static unsafe usize FromPtr(byte* ptr) => ReadUnaligned<nuint>(ptr);
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => nuint.Size;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => value.ToString(format, formatProvider);

    [MethodImpl(Utils.Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => value.TryFormat(destination, out charsWritten, format, provider);

    public string Bin {
        [MethodImpl(Utils.Inline)]
        get => nuint.Size switch {
            4 => $"{value:B32}",
            8 => $"{value:B64}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Hex {
        [MethodImpl(Utils.Inline)]
        get => nuint.Size switch {
            4 => $"{value:X8}",
            8 => $"{value:X16}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Dec {
        [MethodImpl(Utils.Inline)]
        get => $"{value:D}";
    }


    [MethodImpl(Utils.Inline)]
    public static usize operator &(usize left, usize right) => new(left.value & right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator |(usize left, usize right) => new(left.value | right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator ^(usize left, usize right) => new(left.value ^ right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator ~(usize value) => new(~value.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator +(usize left, usize right) => new(left.value + right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator -(usize left, usize right) => new(left.value - right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator *(usize left, usize right) => new(left.value * right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator /(usize left, usize right) => new(left.value / right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator %(usize left, usize right) => new(left.value % right.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator <<(usize value, usize shiftAmount) => new(value.value << (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator >> (usize value, usize shiftAmount) => new(value.value >> (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static usize operator >>> (usize value, usize shiftAmount) => new(value.value >>> (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static bool operator >(usize left, usize right) => left.value > right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator >=(usize left, usize right) => left.value >= right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator <(usize left, usize right) => left.value < right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator <=(usize left, usize right) => left.value <= right.value;

    [MethodImpl(Utils.Inline)]
    public static usize operator +(usize value) => value;
    
    [MethodImpl(Utils.Inline)]
    public static usize operator -(usize value) => new(~value.value + 1);
    
    public static usize AdditiveIdentity { [MethodImpl(Utils.Inline)] get => 0; }
    public static usize MultiplicativeIdentity { [MethodImpl(Utils.Inline)] get => 1; }
    [MethodImpl(Utils.Inline)]
    public int CompareTo(usize other) => value.CompareTo(other.value);
    
    public static usize MaxValue { [MethodImpl(Utils.Inline)] get => nuint.MaxValue; }
    
    public static usize MinValue { [MethodImpl(Utils.Inline)] get => nuint.MinValue; }
    
}

public readonly record struct isize(nint value):
    ISpanFormattable,
    ISizedPrimValue<isize>,
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
    [MethodImpl(Utils.Inline)]
    public static implicit operator nint(isize self) => EsmVM.MemAddrISize + self.value;
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(nint self) => new(self - EsmVM.MemAddrISize);
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator usize(isize self) => new(unchecked((nuint) self.value));
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator nuint(isize self) => (usize) self;
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(nuint self) => (usize) self;
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(int self) => new(self);
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(uint self) => new(unchecked((int) self));
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(long self) => new((nint) self);
    
    [MethodImpl(Utils.Inline)]
    public static implicit operator isize(ulong self) => new((nint) unchecked((long) self));  
    
    [MethodImpl(Utils.Inline)]
    public static unsafe explicit operator byte*(isize self) => (byte*) (nint) self;
    [MethodImpl(Utils.Inline)]
    public static unsafe explicit operator isize(byte* self) => (nint) self;

    [MethodImpl(Utils.Inline)]
    public static unsafe isize FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(long);
        return IsLittleEndian
            ? ReverseEndianness(ReadUnaligned<long>(start))
            : ReadUnaligned<long>(start);
    }

    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(long)];
        Write(arr, IsLittleEndian ? ReverseEndianness(value) : value);
        return arr;
    }

    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr) {
        WriteUnaligned(ptr, value);
    }

    [MethodImpl(Utils.Inline)]
    public static unsafe isize FromFatPtr(byte* ptr, usize size) => FromPtr(ptr);

    [MethodImpl(Utils.Inline)]
    public static unsafe isize FromPtr(byte* ptr) => ReadUnaligned<nint>(ptr);
    
    public static unsafe usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => nint.Size;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => value.ToString(format, formatProvider);

    [MethodImpl(Utils.Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => value.TryFormat(destination, out charsWritten, format, provider);

    public string Bin {
        [MethodImpl(Utils.Inline)]
        get => nint.Size switch {
            4 => $"{value:B32}",
            8 => $"{value:B64}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Hex {
        [MethodImpl(Utils.Inline)]
        get => nint.Size switch {
            4 => $"{value:X8}",
            8 => $"{value:X16}",
            _ => throw new InvalidOperationException()
        };
    }

    public string Dec {
        [MethodImpl(Utils.Inline)]
        get => $"{value:D}";
    }


    [MethodImpl(Utils.Inline)]
    public static isize operator &(isize left, isize right) => new(left.value & right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator |(isize left, isize right) => new(left.value | right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator ^(isize left, isize right) => new(left.value ^ right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator ~(isize value) => new(~value.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator +(isize left, isize right) => new(left.value + right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator -(isize left, isize right) => new(left.value - right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator *(isize left, isize right) => new(left.value * right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator /(isize left, isize right) => new(left.value / right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator %(isize left, isize right) => new(left.value % right.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator <<(isize value, isize shiftAmount) => new(value.value << (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator >> (isize value, isize shiftAmount) => new(value.value >> (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static isize operator >>> (isize value, isize shiftAmount) => new(value.value >>> (int) shiftAmount.value);

    [MethodImpl(Utils.Inline)]
    public static bool operator >(isize left, isize right) => left.value > right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator >=(isize left, isize right) => left.value >= right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator <(isize left, isize right) => left.value < right.value;

    [MethodImpl(Utils.Inline)]
    public static bool operator <=(isize left, isize right) => left.value <= right.value;

    [MethodImpl(Utils.Inline)]
    public static isize operator +(isize value) => value;
    
    [MethodImpl(Utils.Inline)]
    public static isize operator -(isize value) => new(~value.value + 1);

    public static isize AdditiveIdentity { [MethodImpl(Utils.Inline)] get => 0; }
    public static isize MultiplicativeIdentity { [MethodImpl(Utils.Inline)] get => 1; }
    [MethodImpl(Utils.Inline)]
    public int CompareTo(isize other) => value.CompareTo(other.value);
    
    public static isize MaxValue { [MethodImpl(Utils.Inline)] get => nint.MaxValue; }
    
    public static isize MinValue { [MethodImpl(Utils.Inline)] get => nint.MinValue; }
    
}