/*using System.Numerics;
using EsmRuntime.Common;
using EsmRuntime.Common.Types;
using JetBrains.Annotations;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Memory.Util;

// Sentinel integer
// ReSharper disable once InconsistentNaming
public readonly record struct usentinel: 
    ISpanFormattable,
    IBitwiseOperators<usentinel, usentinel, usentinel>,
    IAdditionOperators<usentinel, usentinel, usentinel>,
    ISubtractionOperators<usentinel, usentinel, usentinel>,
    IMultiplyOperators<usentinel, usentinel, usentinel>,
    IDivisionOperators<usentinel, usentinel, usentinel>,
    IModulusOperators<usentinel, usentinel, usentinel>,
    IShiftOperators<usentinel, usentinel, usentinel>,
    IComparisonOperators<usentinel, usentinel, bool>,
    IUnaryPlusOperators<usentinel, usentinel>,
    IUnaryNegationOperators<usentinel, usentinel>,
    IAdditiveIdentity<usentinel, usentinel>,
    IMultiplicativeIdentity<usentinel, usentinel>,
    IComparable<usentinel>,
    IMinMaxValue<usentinel> {
    
    // ReSharper disable once InconsistentNaming
    public nuint value { get; }

    public usentinel(nuint value) : this(value, true) { }

    [UsedImplicitly]
    usentinel(nuint value, bool @checked): this() {
        if (@checked && value == nuint.MaxValue) throw new InvalidOperationException();
        this.value = value;
    }
    
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly usentinel nil = new(nuint.MaxValue, false);

    public void Deconstruct(out nuint value) {
        value = this.value;
    }
    
    public static implicit operator usentinel(nuint value) => new(value);
    public static implicit operator nuint(usentinel value) => value.value;

    public static usentinel operator &(usentinel left, usentinel right) => left.value & right.value;

    public static usentinel operator |(usentinel left, usentinel right) => left.value | right.value;

    public static usentinel operator ^(usentinel left, usentinel right) => left.value ^ right.value;

    public static usentinel operator ~(usentinel value) => ~value.value;

    public static usentinel operator +(usentinel left, usentinel right) => value

    public static usentinel operator -(usentinel left, usentinel right) => throw new NotImplementedException();

    public static usentinel operator *(usentinel left, usentinel right) => throw new NotImplementedException();

    public static usentinel operator /(usentinel left, usentinel right) => throw new NotImplementedException();

    public static usentinel operator %(usentinel left, usentinel right) => throw new NotImplementedException();

    public static usentinel operator <<(usentinel value, usentinel shiftAmount) => throw new NotImplementedException();

    public static usentinel operator >> (usentinel value, usentinel shiftAmount) => throw new NotImplementedException();

    public static usentinel operator >>> (usentinel value, usentinel shiftAmount) => throw new NotImplementedException();

    public static bool operator >(usentinel left, usentinel right) => throw new NotImplementedException();

    public static bool operator >=(usentinel left, usentinel right) => throw new NotImplementedException();

    public static bool operator <(usentinel left, usentinel right) => throw new NotImplementedException();

    public static bool operator <=(usentinel left, usentinel right) => throw new NotImplementedException();

    public static usentinel operator +(usentinel value) => throw new NotImplementedException();

    public static usentinel operator -(usentinel value) => throw new NotImplementedException();
    public static usentinel AdditiveIdentity => 0;
    public static usentinel MultiplicativeIdentity => 1;
    public int CompareTo(usentinel other) => throw new NotImplementedException();
    public static usentinel MaxValue => nuint.MaxValue - 1;
    public static usentinel MinValue => 0;
    public string ToString(string? format, IFormatProvider? formatProvider)
        => this == nil ? "nil" : value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => value.TryFormat(destination, out charsWritten, format, provider);
}*/