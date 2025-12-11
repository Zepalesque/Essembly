// ReSharper disable InconsistentNaming

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common.Types;



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


public readonly unsafe ref struct Reference<T>(usize address): ISizedValue<Reference<T>> where T : struct, IByteSerializable<T>, allows ref struct {
    public usize Address { get; } = address;
    
    public void ToSpan(Span<byte> span) {
        Address.ToSpan(span);
    }
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }

    public T Dereference() {
        var ptr = (byte*) Address;
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