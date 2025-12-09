// ReSharper disable InconsistentNaming

using System.Text;
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common.Types;

public interface INumberFormattable {
    public string Bin { get; }
    public string Hex { get; }
    public string Dec { get; }
}

public interface IAsciiFormattable<in T> where T : struct, IAsciiFormattable<T>, allows ref struct {
    public static abstract explicit operator char(T value);
}
public interface IUtf16Formattable<in T> where T : struct, IUtf16Formattable<T>, allows ref struct {
    public static abstract explicit operator char(T value);
}

public interface ISizedValue<out T> where T : struct, ISizedValue<T>, allows ref struct {
    public void ToSpan(Span<byte> span);
    public unsafe void ToPtr(byte* ptr);
    public static abstract T FromSpan(ReadOnlySpan<byte> bytes);
    public static abstract unsafe T FromPtr(byte* ptr);

    public static abstract int ByteCount { get; }
}

public unsafe interface IReference<out T>: ISizedValue<T> where T : struct, IReference<T>, allows ref struct {
    public byte* Start { get; }

    static T ISizedValue<T>.FromSpan(ReadOnlySpan<byte> span) {
        usize addr = usize.FromSpan(span);
        return T.FromRefPtr((byte*)addr);
    }

    static T ISizedValue<T>.FromPtr(byte* ptr) => T.FromSpan(new(ptr, ByteCount));

    static int ISizedValue<T>.ByteCount => ByteCount;
    
    public new static int ByteCount => usize.ByteCount;

    int ByteCountInMemory { get; }

    static abstract T FromRefPtr(byte* ptr);

    void ISizedValue<T>.ToSpan(Span<byte> span) {
        var addr = (usize)Start;
        addr.ToSpan(span);
    }

    void ISizedValue<T>.ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));
}

// Null-terminated
public readonly unsafe struct str(byte* start, int length) : IReference<str> {
    public byte* Start { get; } = start;
    int Length { get; } = length;
    
   

    public int ByteCountInMemory => Length;

    public static str FromRefPtr(byte* ptr) {
        var i = 0;
        while (*(ptr + i) != 0x00) i++;

        return new(ptr, i + 1);
    }
    
    public ReadOnlySpan<byte> ToStringSpan() 
        => new(Start, Length);

    public override string ToString() 
        => Encoding.ASCII.GetString(ToStringSpan());
}

public readonly unsafe struct ImmutableSlice<T>(byte* start, usize lengthInElements) : IReference<ImmutableSlice<T>> where T: unmanaged, ISizedValue<T> {
    public byte* Start { get; } = start;
    usize LengthInElements { get; } = lengthInElements;

    public int ByteCountInMemory { get; } = usize.ByteCount + lengthInElements * T.ByteCount;

    public static ImmutableSlice<T> FromRefPtr(byte* ptr) {
        ReadOnlySpan<byte> span = new(ptr, usize.ByteCount);
        
        usize size = usize.FromSpan(span);
        
        return new(ptr, size);
    }

    public T this[int i] {
        get {

            if (i >= LengthInElements)
                throw new InvalidIndexError($"Invalid index {i} for slice with length {LengthInElements}");
            
            int size = T.ByteCount;
            
            int offestBytes = usize.ByteCount + i * size;
            return T.FromSpan(new(Start + offestBytes, size));
        }
    }

}