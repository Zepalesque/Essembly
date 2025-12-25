using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Heap;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Common.Types;

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct Ptr<T>(isize address): ISizedTypeValue<Ptr<T>> where T : struct, ITypedValue<T>, allows ref struct {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    public usize DerefSize {
        [MethodImpl(Utils.Inline)]
        get => T.ConstSize ?? usize.FromPtr((byte*)Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public T Dereference() {
        if (Address == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to dereference the null pointer!");
        
        if (T.ConstSize != null) {
            var ptr = (byte*) Address;
            return T.FromFatPtr(ptr, T.ConstSize.Value);
        } else {
            var ptr = (byte*)(Address + (isize)usize.ByteCount);
            return T.FromFatPtr(ptr, DerefSize);
        }
    }
    
    [MethodImpl(Utils.Inline)]
    public Ptr Raw() {
        return new(Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ptr<T> CreateAt(byte* ptr, scoped ref T value) {
        var addr = (isize) ptr;
        if (addr == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to set the null pointer!");
        
        if (T.ConstSize != null) {
            value.ToPtr(ptr);
        } else {
            usize size = value.InstSize;
            size.ToPtr(ptr);
            value.ToPtr(ptr + usize.ByteCount);
        }
        
        return new(addr);
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ptr<T> FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static Ptr<T> FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static Ptr<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
}

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct MutPtr<T>(isize address): ISizedTypeValue<MutPtr<T>> where T : struct, ITypedValue<T>, allows ref struct {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    public usize DerefSize {
        [MethodImpl(Utils.Inline)]
        get => T.ConstSize ?? usize.FromPtr((byte*)Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public T Dereference() {
        if (Address == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to dereference the null pointer!");
        
        if (T.ConstSize != null) {
            var ptr = (byte*) Address;
            return T.FromFatPtr(ptr, T.ConstSize.Value);
        } else {
            var ptr = (byte*)(Address + (isize)usize.ByteCount);
            return T.FromFatPtr(ptr, DerefSize);
        }
    }
    
    public bool TryReplace(scoped ref T replacement, out T original) {
        if (DerefSize != replacement.InstSize) {
            original = default;
            return false;
        }
        
        original = Dereference();
        CreateAt((byte*)Address, ref replacement);
        return true;
    }
    
    [MethodImpl(Utils.Inline)]
    public MutPtr Raw() {
        return new(Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr<T> CreateAt(byte* ptr, scoped ref T value) {
        return TryCreateAt(ptr, ref value, out MutPtr<T> res) 
            ? res
            : throw new NullAccessError("Attempted to set the null pointer!");
    }
    
    public static bool TryCreateAt(byte* ptr, scoped ref T value, out MutPtr<T> result) {
        var addr = (isize) ptr;
        if (addr == EsmVM.NullAddr) {
            result = default;
            return false;
        }
        
        if (T.ConstSize != null) {
            value.ToPtr(ptr);
        } else {
            usize size = value.InstSize;
            size.ToPtr(ptr);
            value.ToPtr(ptr + usize.ByteCount);
        }
        
        result = new(addr);
        return true;
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr<T> FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr<T> FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
}

// Raw Pointer
[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct Ptr(isize address): IPrimValue<Ptr> {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public Ptr<T> Coerce<T>() where T : struct, ITypedValue<T>, allows ref struct {
        return new(Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ptr CreateAt<T>(byte* ptr, scoped ref T value) where T : struct, ITypedValue<T>, allows ref struct {
        Ptr<T> typed = Ptr<T>.CreateAt(ptr, ref value);
        return typed.Raw();
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ptr CreateAt(byte* ptr){
        var addr = (isize) ptr;
        return addr == EsmVM.NullAddr ? throw new NullAccessError("Attempted to set the null pointer!") : new(addr);
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ptr FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static Ptr FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static Ptr FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Utils.Inline)] get => "$raw*"u8; }
}

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct MutPtr(isize address): IPrimValue<MutPtr> {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public MutPtr<T> Coerce<T>() where T : struct, ITypedValue<T>, allows ref struct {
        return new(Address);
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr CreateAt<T>(byte* ptr, scoped ref T value) where T : struct, ITypedValue<T>, allows ref struct {
        MutPtr<T> typed = MutPtr<T>.CreateAt(ptr, ref value);
        return typed.Raw();
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr CreateAt(byte* ptr){
        var addr = (isize) ptr;
        return addr == EsmVM.NullAddr ? throw new NullAccessError("Attempted to set the null pointer!") : new(addr);
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static MutPtr FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Utils.Inline)] get => "$raw*"u8; }
}

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct Ref<T>(isize address): ISizedTypeValue<Ref<T>> where T : struct, ITypedValue<T>, allows ref struct {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public T Dereference() {
        return AsTypedPtr().Dereference().Dereference();
    }
    
    public Ptr<Ptr> AsPtr() {
        return new(Address);
    }
    
    public Ptr<Ptr<T>> AsTypedPtr() {
        return new(Address);
    }
    
    public static Ref<T> FromPtr(Ptr<Ptr> ptr) {
        return new(ptr.Address);
    }
    
    public static Ref<T> FromTypedPtr(Ptr<Ptr<T>> ptr) {
        return new(ptr.Address);
    }
    
    
    [MethodImpl(Utils.Inline)]
    public static Ref<T> CreateAt(byte* valPtr, byte* addrPtr, scoped ref T value) {
        
        Ptr<T> valPtrImpl = Ptr<T>.CreateAt(valPtr, ref value);
        
        Ptr<Ptr<T>> addrPtrImpl = Ptr<Ptr<T>>.CreateAt(valPtr, ref valPtrImpl);
        
        return FromTypedPtr(addrPtrImpl);
    }
    
    [MethodImpl(Utils.Inline)]
    public static Ref<T> FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static Ref<T> FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static Ref<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
}

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct MutRef<T>(isize address): ISizedTypeValue<MutRef<T>> where T : struct, ITypedValue<T>, allows ref struct {
    public isize Address { [MethodImpl(Utils.Inline)] get; } = address;
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        Address.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public T Dereference() {
        return AsTypedPtr().Dereference().Dereference();
    }
    
    public Ptr<MutPtr> AsPtr() {
        return new(Address);
    }
    
    public MutPtr<Ptr<T>> AsTypedPtr() {
        return new(Address);
    }
    
    public static MutRef<T> FromPtr(Ptr<Ptr> ptr) {
        return new(ptr.Address);
    }
    
    public static MutRef<T> FromTypedPtr(Ptr<Ptr<T>> ptr) {
        return new(ptr.Address);
    }
    
    public bool Realloc(scoped ref T replacement, scoped ref Heap heap) {
        MutPtr<Ptr<T>> ptr = AsTypedPtr();
        Ptr<T> repPtr = heap.AllocatePtr(ref replacement);
        
        if (ptr.TryReplace(ref repPtr, out Ptr<T> inner)) {
            heap.Free(inner);
            return true;
        } else {
            heap.Free(repPtr);
            return false;
        }
    }
    
    
    [MethodImpl(Utils.Inline)]
    public static MutRef<T> CreateAt(byte* valPtr, byte* addrPtr, scoped ref T value) {
        
        Ptr<T> valPtrImpl = Ptr<T>.CreateAt(valPtr, ref value);
        
        Ptr<Ptr<T>> addrPtrImpl = Ptr<Ptr<T>>.CreateAt(valPtr, ref valPtrImpl);
        
        return FromTypedPtr(addrPtrImpl);
    }
    
    [MethodImpl(Utils.Inline)]
    public static MutRef<T> FromFatPtr(byte* ptr, usize size)
        => new(isize.FromPtr(ptr));
    
    [MethodImpl(Utils.Inline)]
    public static MutRef<T> FromPtr(byte* ptr)
        => new(isize.FromPtr(ptr));
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => isize.ByteCount;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new InvalidOperationException();
    
    [MethodImpl(Utils.Inline)]
    public static MutRef<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new InvalidOperationException();
}