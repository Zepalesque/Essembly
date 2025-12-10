// ReSharper disable InconsistentNaming

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

public interface IByteSerializable<out T> where T: struct, IByteSerializable<T>, allows ref struct {
    void ToSpan(Span<byte> span);
    unsafe void ToPtr(byte* ptr);
    public static abstract T FromSpan(ReadOnlySpan<byte> bytes);
    public static abstract unsafe T FromPtr(byte* ptr);
    public int InstanceSize { get; }
}

public interface IBytecodeSerializable<out T> where T : struct, IByteSerializable<T>, allows ref struct {
    public static abstract unsafe ReadOnlySpan<byte> DataToSerialize(byte* start);
}
    
public interface ISizedValue<out T> : IByteSerializable<T> where T : struct, ISizedValue<T>, allows ref struct {
    public static abstract int ByteCount { get; }
    int IByteSerializable<T>.InstanceSize => T.ByteCount;
    
}

public unsafe interface IReference<out T>: ISizedValue<T> where T : struct, IReference<T>, allows ref struct {
    public byte* Start { get; }

    static T IByteSerializable<T>.FromSpan(ReadOnlySpan<byte> span) {
        usize addr = usize.FromSpan(span);
        return T.FromRefPtr((byte*)addr);
    }

    static T IByteSerializable<T>.FromPtr(byte* ptr) => T.FromSpan(new(ptr, ByteCount));

    static int ISizedValue<T>.ByteCount => ByteCount;
    
    public new static int ByteCount => usize.ByteCount;

    int ByteCountInMemory { get; }

    static abstract T FromRefPtr(byte* ptr);

    void IByteSerializable<T>.ToSpan(Span<byte> span) {
        var addr = (usize)Start;
        addr.ToSpan(span);
    }

    void IByteSerializable<T>.ToPtr(byte* ptr) => ToSpan(new(ptr, ByteCount));
}

public readonly unsafe ref struct Reference<T>(usize address): ISizedValue<Reference<T>> where T : struct, IByteSerializable<T>, allows ref struct {
    public void ToSpan(Span<byte> span) {
        address.ToSpan(span);
    }
    public void ToPtr(byte* ptr) {
        address.ToPtr(ptr);
    }

    public T Dereference() {
        var ptr = (byte*) address;
        return T.FromPtr(ptr);
    }

    public static Reference<T> CreateAt(byte* ptr, T value) {
        value.ToPtr(ptr);
        var addr = (usize) ptr;
        return new(addr);
    }

    public static Reference<T> FromSpan(ReadOnlySpan<byte> bytes)
        => new(usize.FromSpan(bytes));

    public static Reference<T> FromPtr(byte* ptr) 
        => new(usize.FromPtr(ptr));
    public static int ByteCount => usize.ByteCount;
    public int InstanceSize => ByteCount;
}