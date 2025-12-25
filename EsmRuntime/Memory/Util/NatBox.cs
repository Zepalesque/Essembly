using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EsmRuntime.Memory.Util;

public unsafe struct NatBox<T> : IDisposable
    where T : unmanaged, allows ref struct {
    T* _ptr;
    
    ref T Value { [MethodImpl(Utils.Inline)] get => ref *_ptr; }
    
    bool HasValue { [MethodImpl(Utils.Inline)] get => _ptr != null; }
    
    [MethodImpl(Utils.Inline)]
    NatBox(T* ptr) {
        _ptr = ptr;
    }
    
    [MethodImpl(Utils.Inline)]
    public NatBox(scoped in T value) {
        _ptr = Utils.AllocNat(in value);
    }
    
    [MethodImpl(Utils.Inline)]
    public NatBox<T> Move() {
        var box = new NatBox<T>(_ptr);
        _ptr = null;
        
        return box;
    }
    
    [MethodImpl(Utils.Inline)]
    public bool Move(out T dest) {
        if (!HasValue) {
            dest = default;
            return false;
        }
        
        dest = Value;
        _ptr = null;
        return true;
    }
    
    [MethodImpl(Utils.Inline)]
    public bool Swap(scoped in T value, out T original) {
        if (!HasValue) {
            _ptr = Utils.AllocNat(in value);
            original = default;
            return false;
        }
        
        original = Value;
        *_ptr = value;
        return true;
    }
    
    [MethodImpl(Utils.Inline)]
    public void Dispose() {
        if (_ptr == null) return;
        
        TryDispose(_ptr);
        NativeMemory.AlignedFree(_ptr);
        
        _ptr = null;
    }
    
    // ReSharper disable once StaticMemberInGenericType
    static readonly delegate*<T*, void> FuncPtr;
    
    [MethodImpl(Utils.Inline)]
    static NatBox() {
        if (!typeof(IDisposable).IsAssignableFrom(typeof(T))) return;
        var func = (void*) typeof(Disposer<>).MakeGenericType(typeof(T)).GetMethod(nameof(Disposer<>.Dispose))!.MethodHandle.GetFunctionPointer();
        
        FuncPtr = (delegate*<T*, void>) func;
    }
    
    [MethodImpl(Utils.Inline)]
    static void TryDispose(T* ptr) {
        if (FuncPtr == null) return;
        FuncPtr(ptr);
    }
}

static class Disposer<T> where T : unmanaged, IDisposable, allows ref struct {
    [MethodImpl(Utils.Inline)]
    internal static unsafe void Dispose(T* value) {
        value->Dispose();
    }
}

public static partial class Utils {
    [MethodImpl(Inline)]
    public static unsafe T* AllocNat<T>(scoped in T value, nuint count = 1) where T : unmanaged, allows ref struct {
        var ptr = (T*)NativeMemory.AlignedAlloc((nuint)sizeof(T) * count, BitOperations.RoundUpToPowerOf2((nuint)sizeof(T)));
        *ptr = value;
        return ptr;
    }
}