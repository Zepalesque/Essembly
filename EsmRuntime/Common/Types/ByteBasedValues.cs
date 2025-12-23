

namespace EsmRuntime.Common.Types;

public interface IByteSerializable<out T> where T: struct, IByteSerializable<T>, allows ref struct {
    unsafe void ToPtr(byte* ptr);
    public static abstract unsafe T FromFatPtr(byte* ptr, usize size);
    public nuint InstSize { get; }
}


public interface IBytecodeSerializable<out T>: IByteSerializable<T> where T : struct, IBytecodeSerializable<T>, allows ref struct {
    public static abstract unsafe T FromBytecode(byte* start, int* pc);
    public byte[] ToBytecode();
}

public interface ISizedValue<out T> : IBytecodeSerializable<T> where T : struct, ISizedValue<T>, allows ref struct {
    public static abstract usize ByteCount { get; }
    nuint IByteSerializable<T>.InstSize => T.ByteCount;

    static abstract unsafe T FromPtr(byte* ptr);
}

public interface ITypedValue<out T> : IByteSerializable<T> where T : struct, ITypedValue<T>, allows ref struct {
    public static abstract ReadOnlySpan<byte> Signature { get; }
}


public readonly unsafe ref struct Reference<T>(usize address): ISizedValue<Reference<T>> where T : struct, IByteSerializable<T>, allows ref struct {
    public usize Address { get; } = address;
    public usize DerefSize => usize.FromPtr((byte*) Address);
    
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }

    public T Dereference() {
        if (Address == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to dereference the null pointer!");
        
        var ptr = (byte*) (Address + usize.ByteCount);
        return T.FromFatPtr(ptr, DerefSize);
    }

    public static Reference<T> CreateAt(byte* ptr, T value) {
        usize size = value.InstSize;
        size.ToPtr(ptr);
        value.ToPtr(ptr + usize.ByteCount);
        var addr = (usize) ptr;
        return new(addr);
    }

    public static Reference<T> FromFatPtr(byte* ptr, usize size)
        => new(usize.FromPtr(ptr));
    
    public static Reference<T> FromPtr(byte* ptr) 
        => new(usize.FromPtr(ptr));
    
    public static usize ByteCount => usize.ByteCount;
    public nuint InstSize => ByteCount;
    public byte[] ToBytecode() => throw new InvalidOperationException();

    public static Reference<T> FromBytecode(byte* start, int* pc) => throw new InvalidOperationException();
}