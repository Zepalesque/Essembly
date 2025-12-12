using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

// #if TARGET_64BIT
// using uaddr__impl = u64;
// #else
// using uaddr__impl = u32;
// #endif
using usize__impl = u16;
using isize__impl = i16;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public readonly record struct usize(usize__impl val):
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator nuint(usize self) => EsmVM.MemAddr + self.val;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator usize(nuint self) => new((usize__impl) (self - EsmVM.MemAddr));    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(usize self) => self.val;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator usize(int self) => new((usize__impl)self);    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator usize__impl(usize self) => self.val;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator usize(usize__impl self) => new(self);    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe explicit operator byte*(usize self) => (byte*) (nuint) self;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe explicit operator usize(byte* self) => (usize) (nuint) self;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToSpan(Span<byte> span) {
        val.ToSpan(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ToPtr(byte* ptr) => val.ToPtr(ptr);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize FromSpan(ReadOnlySpan<byte> bytes) => new(usize__impl.FromSpan(bytes));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe usize FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize FromProgram(ReadOnlySpan<byte> bytes, ref int pc) {
        var subspan = bytes[pc..(pc += ByteCount)];
        return FromSpan(subspan);
    }
    
    
    public static int ByteCount => usize__impl.ByteCount;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => val.ToString(format, formatProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => val.TryFormat(destination, out charsWritten, format, provider);

    public string Bin => val.Bin;

    public string Hex => val.Hex;

    public string Dec => val.Dec;
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator &(usize left, usize right) => new(left.val & right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator |(usize left, usize right) => new(left.val | right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator ^(usize left, usize right) => new(left.val ^ right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator ~(usize value) => new(~value.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator +(usize left, usize right) => new(left.val + right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator -(usize left, usize right) => new(left.val - right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator *(usize left, usize right) => new(left.val * right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator /(usize left, usize right) => new(left.val / right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator %(usize left, usize right) => new(left.val % right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator <<(usize value, usize shiftAmount) => new(value.val << shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator >> (usize value, usize shiftAmount) => new(value.val >> shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator >>> (usize value, usize shiftAmount) => new(value.val >>> shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(usize left, usize right) => left.val > right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(usize left, usize right) => left.val >= right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(usize left, usize right) => left.val < right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(usize left, usize right) => left.val <= right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator +(usize value) => value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static usize operator -(usize value) => new(-value.val);
    public static usize AdditiveIdentity { get; } = 0;
    public static usize MultiplicativeIdentity { get; } = 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(usize other) => throw new NotImplementedException();
    public static usize MaxValue => (usize) usize__impl.MaxValue;
    public static usize MinValue => (usize) usize__impl.MinValue;

    public static unsafe usize FromBytecode(byte* start, int* pc) => throw new NotImplementedException();
}

// TODO: figure this out
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public readonly record struct isize(isize__impl val):
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator int(isize self) => self.val;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator isize(int self) => new((isize__impl)self);    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator isize__impl(isize self) => self.val;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator isize(isize__impl self) => new(self);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToSpan(Span<byte> span) {
        val.ToSpan(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void ToPtr(byte* ptr) => val.ToPtr(ptr);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize FromSpan(ReadOnlySpan<byte> bytes) => new(isize__impl.FromSpan(bytes));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe isize FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize FromProgram(ReadOnlySpan<byte> bytes, ref int pc) {
        var subspan = bytes[pc..(pc += ByteCount)];
        return FromSpan(subspan);
    }
    
    
    public static int ByteCount => isize__impl.ByteCount;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => val.ToString(format, formatProvider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => val.TryFormat(destination, out charsWritten, format, provider);

    public string Bin => val.Bin;

    public string Hex => val.Hex;

    public string Dec => val.Dec;
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator &(isize left, isize right) => new(left.val & right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator |(isize left, isize right) => new(left.val | right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator ^(isize left, isize right) => new(left.val ^ right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator ~(isize value) => new(~value.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator +(isize left, isize right) => new(left.val + right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator -(isize left, isize right) => new(left.val - right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator *(isize left, isize right) => new(left.val * right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator /(isize left, isize right) => new(left.val / right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator %(isize left, isize right) => new(left.val % right.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator <<(isize value, isize shiftAmount) => new(value.val << shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator >> (isize value, isize shiftAmount) => new(value.val >> shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator >>> (isize value, isize shiftAmount) => new(value.val >>> shiftAmount.val);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(isize left, isize right) => left.val > right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(isize left, isize right) => left.val >= right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(isize left, isize right) => left.val < right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(isize left, isize right) => left.val <= right.val;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator +(isize value) => value;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static isize operator -(isize value) => new(-value.val);
    public static isize AdditiveIdentity { get; } = 0;
    public static isize MultiplicativeIdentity { get; } = 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(isize other) => throw new NotImplementedException();
    public static isize MaxValue => (isize) isize__impl.MaxValue;
    public static isize MinValue => (isize) isize__impl.MinValue;
}