

using System.Runtime.CompilerServices;

namespace EsmRuntime.Common.Types;

public interface IByteSerializable<out T> where T: struct, IByteSerializable<T>, allows ref struct {
    unsafe void ToPtr(byte* ptr);
    public static abstract unsafe T FromFatPtr(byte* ptr, usize size);
    public nuint InstanceSize { get; }
}

public interface IBytecodeSerializable<out T> where T : struct, IByteSerializable<T>, allows ref struct {
    public static abstract unsafe T FromBytecode(byte* start, int* pc);
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator);
    
}

public interface ISizedValue<out T> : IByteSerializable<T>, IBytecodeSerializable<T> where T : struct, ISizedValue<T>, allows ref struct {
    public static abstract usize ByteCount { get; }
    nuint IByteSerializable<T>.InstanceSize => T.ByteCount;

    static abstract unsafe T FromPtr(byte* ptr);

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
        usize size = value.InstanceSize;
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
    public nuint InstanceSize => ByteCount;
    public FatPtr ToBytecode(delegate*<nuint, byte*> generator) => throw new InvalidOperationException();

    public static Reference<T> FromBytecode(byte* start, int* pc) => throw new InvalidOperationException();
}

// TODO: Move to compiler
public unsafe ref struct Embed<T>(T value): IBytecodeSerializable<Embed<T>>, IByteSerializable<Embed<T>>
    where T : struct, IByteSerializable<T>, allows ref struct {
    T _value = value;
    
    public T Value => _value;

    usize ValSize => _value.InstanceSize;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ToPtr(byte* ptr) {
        ValSize.ToPtr(ptr);
        _value.ToPtr(ptr + usize.ByteCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Embed<T> FromFatPtr(byte* ptr, usize size) 
        => new(T.FromFatPtr(ptr + usize.ByteCount, size));
    public nuint InstanceSize => usize.ByteCount + ValSize;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Embed<T> IBytecodeSerializable<Embed<T>>.FromBytecode(byte* start, int* pc) {
        usize size = usize.FromPtr(start);
        Embed<T> self = new(T.FromFatPtr(start + usize.ByteCount, size));
        *pc += (int) self.InstanceSize;
        return self;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        nuint size = InstanceSize;
        byte* ptr = generator(size);
        ToPtr(ptr);
        return new(ptr, size);
    }
}