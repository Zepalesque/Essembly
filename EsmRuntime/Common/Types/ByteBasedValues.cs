

namespace EsmRuntime.Common.Types;

public interface IByteSerializable<out T> where T: struct, IByteSerializable<T>, allows ref struct {
    void ToSpan(Span<byte> span);
    unsafe void ToPtr(byte* ptr);
    public static abstract T FromSpan(ReadOnlySpan<byte> bytes);
    public int InstanceSize { get; }
}

public interface IBytecodeSerializable<out T> where T : struct, IByteSerializable<T>, allows ref struct {
    public static abstract unsafe T FromBytecode(byte* start, int* pc);
    public Span<byte> ToBytecode(Func<int, nuint> generator);
    
}

public interface ISizedValue<out T> : IByteSerializable<T>, IBytecodeSerializable<T> where T : struct, ISizedValue<T>, allows ref struct {
    public static abstract int ByteCount { get; }
    int IByteSerializable<T>.InstanceSize => T.ByteCount;

    static abstract unsafe T FromPtr(byte* ptr);

    static unsafe T IBytecodeSerializable<T>.FromBytecode(byte* start, int* pc) {
        var self = T.FromPtr(start);
        pc += T.ByteCount;
        return self;
    }

    public new static unsafe T FromBytecode(byte* start, int* pc) 
        => T.FromBytecode(start, pc);

    unsafe Span<byte> IBytecodeSerializable<T>.ToBytecode(Func<int, nuint> generator) {
        int size = T.ByteCount;
        
        nuint addr = generator(size);
        
        var ptr = (byte*) addr;
        
        ToPtr(ptr);
        
        return new(ptr, size);
    }
}


public readonly unsafe ref struct Reference<T>(usize address): ISizedValue<Reference<T>> where T : struct, IByteSerializable<T>, allows ref struct {
    public usize Address { get; } = address;
    public usize DerefSize => usize.FromPtr((byte*) Address);
    
    public void ToSpan(Span<byte> span) {
        Address.ToSpan(span);
    }
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }

    public T Dereference() {
        if (Address == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to dereference the null pointer!");
        
        var ptr = (byte*) (Address + usize.ByteCount);
        return T.FromSpan(new(ptr, DerefSize));
    }

    public static Reference<T> CreateAt(byte* ptr, T value) {
        usize size = value.InstanceSize;
        size.ToPtr(ptr);
        value.ToPtr(ptr + usize.ByteCount);
        var addr = (usize) ptr;
        return new(addr);
    }

    public static Reference<T> FromSpan(ReadOnlySpan<byte> bytes)
        => new(usize.FromSpan(bytes));
    
    public static Reference<T> FromPtr(byte* ptr) 
        => new(usize.FromPtr(ptr));
    
    public static int ByteCount => usize.ByteCount;
    public int InstanceSize => ByteCount;
    public Span<byte> ToBytecode(Func<int, nuint> generator) => throw new InvalidOperationException();

    public static Reference<T> FromBytecode(byte* start) => throw new InvalidOperationException();
}

// TODO: Move to compiler
public unsafe ref struct Embed<T>(T value): IBytecodeSerializable<Embed<T>>, IByteSerializable<Embed<T>>
    where T : struct, IByteSerializable<T>, allows ref struct {
    T _value = value;
    public usize ValSize => _value.InstanceSize;


    public void ToSpan(Span<byte> span) {
        fixed (byte* start = &span[0]) {
            ToPtr(start);
        }
    }
    
    public void ToPtr(byte* ptr) {
        ValSize.ToPtr(ptr);
        _value.ToPtr(ptr + usize.ByteCount);
    }

    public static Embed<T> FromSpan(ReadOnlySpan<byte> bytes) 
        => new(T.FromSpan(bytes[usize.ByteCount..]));
    public int InstanceSize => usize.ByteCount + ValSize;
    
    static Embed<T> IBytecodeSerializable<Embed<T>>.FromBytecode(byte* start, int* pc) {
        usize size = usize.FromPtr(start);
        Embed<T> self = new(T.FromSpan(new(start + usize.ByteCount, size)));
        *pc += self.InstanceSize;
        return self;
    }

    public Span<byte> ToBytecode(Func<int, nuint> generator) {
        int size = InstanceSize;
        nuint addr = generator(size);
        byte* ptr = (byte*) addr;
        ToPtr(ptr);
        return new(ptr, size);
    }
}