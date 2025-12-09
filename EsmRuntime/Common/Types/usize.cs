using System.Numerics;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

// #if TARGET_64BIT
// using uaddr__impl = u64;
// #else
// using uaddr__impl = u32;
// #endif
using usize__impl = u16;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
public readonly record struct usize(usize__impl val):
    ISpanFormattable,
    ISizedValue<usize>,
    INumberFormattable,
    IBitwiseOperators<usize, usize, usize>,
    IAdditionOperators<usize, usize, usize>,
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
    public static explicit operator nuint(usize self) => EsmVM.MemAddr + self.val;
    public static explicit operator usize(nuint self) => new((usize__impl) (self - EsmVM.MemAddr));    

    public static implicit operator int(usize self) => self.val;
    public static implicit operator usize(int self) => new((usize__impl)self);    

    public static explicit operator usize__impl(usize self) => self.val;
    public static explicit operator usize(usize__impl self) => new(self);    
    
    public static unsafe explicit operator byte*(usize self) => (byte*) (nuint) self;
    public static unsafe explicit operator usize(byte* self) => (usize) (nuint) self;
    
    public void ToSpan(Span<byte> span) {
        val.ToSpan(span);
    }

    public unsafe void ToPtr(byte* ptr) => val.ToPtr(ptr);

    public static usize FromSpan(ReadOnlySpan<byte> bytes) => new(usize__impl.FromSpan(bytes));
    public static unsafe usize FromPtr(byte* ptr) => FromSpan(new(ptr, ByteCount));

    public static usize FromProgram(ReadOnlySpan<byte> bytes, ref int pc) {
        var subspan = bytes[pc..(pc += ByteCount)];
        return FromSpan(subspan);
    }
    
    
    public static int ByteCount => usize__impl.ByteCount;
    public string ToString(string? format, IFormatProvider? formatProvider)
        => val.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => val.TryFormat(destination, out charsWritten, format, provider);

    public string Bin => val.Bin;

    public string Hex => val.Hex;

    public string Dec => val.Dec;
    

    public static usize operator &(usize left, usize right) => new(left.val & right.val);

    public static usize operator |(usize left, usize right) => new(left.val | right.val);

    public static usize operator ^(usize left, usize right) => new(left.val ^ right.val);

    public static usize operator ~(usize value) => new(~value.val);

    public static usize operator +(usize left, usize right) => new(left.val + right.val);

    public static usize operator *(usize left, usize right) => new(left.val * right.val);

    public static usize operator /(usize left, usize right) => new(left.val / right.val);

    public static usize operator %(usize left, usize right) => new(left.val % right.val);

    public static usize operator <<(usize value, usize shiftAmount) => new(value.val << shiftAmount.val);

    public static usize operator >> (usize value, usize shiftAmount) => new(value.val >> shiftAmount.val);

    public static usize operator >>> (usize value, usize shiftAmount) => new(value.val >>> shiftAmount.val);

    public static bool operator >(usize left, usize right) => left.val > right.val;

    public static bool operator >=(usize left, usize right) => left.val >= right.val;

    public static bool operator <(usize left, usize right) => left.val < right.val;

    public static bool operator <=(usize left, usize right) => left.val <= right.val;

    public static usize operator +(usize value) => value;
    
    public static usize operator -(usize value) => new(-value.val);
    public static usize AdditiveIdentity { get; } = 0;
    public static usize MultiplicativeIdentity { get; } = 1;
    public int CompareTo(usize other) => throw new NotImplementedException();
    public static usize MaxValue => (usize) usize__impl.MaxValue;
    public static usize MinValue => (usize) usize__impl.MinValue;
}