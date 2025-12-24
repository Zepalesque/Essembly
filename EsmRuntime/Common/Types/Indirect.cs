using System.Runtime.CompilerServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types;

[method: MethodImpl(Inline)]
public readonly unsafe ref struct Ptr<T>(isize address): ISizedTypeValue<Ptr<T>> where T : struct, ITypedValue<T>, allows ref struct {
    public isize Address { [MethodImpl(Inline)] get; } = address;
    public usize DerefSize {
        [MethodImpl(Inline)]
        get => usize.FromPtr((byte*)Address);
    }
    
    [MethodImpl(Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Inline)]
    public T Dereference() {
        if (Address == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to dereference the null pointer!");
        
        var ptr = (byte*) (Address + (isize) usize.ByteCount);
        return T.FromFatPtr(ptr, DerefSize);
    }
    
    [MethodImpl(Inline)]
    public Ptr Raw() {
        return new(Address);
    }
    
    [MethodImpl(Inline)]
    public static Ptr<T> CreateAt(byte* ptr, scoped ref T value) {
        usize size = value.InstSize;
        size.ToPtr(ptr);
        value.ToPtr(ptr + usize.ByteCount);
        var addr = (isize) ptr;
        return new(addr);
    }
    
    [MethodImpl(Inline)]
    public static Ptr<T> FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Inline)]
    public static Ptr<T> FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Inline)]
        get => true;
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Inline)]
    public static Ptr<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
}

// Raw Pointer
[method: MethodImpl(Inline)]
public readonly unsafe ref struct Ptr(isize address): IPrimValue<Ptr> {
    public isize Address { [MethodImpl(Inline)] get; } = address;
    
    [MethodImpl(Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Inline)]
    public Ptr<T> Coerce<T>() where T : struct, ITypedValue<T>, allows ref struct {
        return new(Address);
    }
    
    [MethodImpl(Inline)]
    public static Ptr CreateAt<T>(byte* ptr, scoped ref T value) where T : struct, ITypedValue<T>, allows ref struct {
        usize size = value.InstSize;
        size.ToPtr(ptr);
        value.ToPtr(ptr + usize.ByteCount);
        var addr = (isize) ptr;
        return new(addr);
    }
    
    [MethodImpl(Inline)]
    public static Ptr FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Inline)]
    public static Ptr FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Inline)]
        get => true;
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Inline)]
    public static Ptr FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "$raw*"u8; }
}
}