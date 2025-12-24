using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.Util;


public unsafe struct Box<T> : IDisposable
    where T : unmanaged, allows ref struct {
    T* _ptr;
    
    public ref T Value { [MethodImpl(Inline)] get => ref *_ptr; }
    
    [MethodImpl(Inline)]
    internal Box(T* ptr) {
        _ptr = ptr;
    }
    
    [MethodImpl(Inline)]
    public Box(T value) {
        var ptr = (T*)NativeMemory.AlignedAlloc((nuint)sizeof(T), 16);
        *ptr = value;
        
        _ptr = ptr;
    }
    
    [MethodImpl(Inline)]
    public Box<T> Move() {
        var box = new Box<T>(_ptr);
        _ptr = null;
        
        return box;
    }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        if (_ptr == null) return;
        
        TryDispose(_ptr);
        NativeMemory.AlignedFree(_ptr);
        
        _ptr = null;
    }
    
    // ReSharper disable once StaticMemberInGenericType
    static readonly delegate*<T*, void> FuncPtr;
    
    [MethodImpl(Inline)]
    static Box() {
        if (!typeof(IDisposable).IsAssignableFrom(typeof(T))) return;
        var func = (void*) typeof(Disposer<>).MakeGenericType(typeof(T)).GetMethod(nameof(Disposer<>.Dispose))!.MethodHandle.GetFunctionPointer();
        
        FuncPtr = (delegate*<T*, void>) func;
    }
    
    [MethodImpl(Inline)]
    static void TryDispose(T* ptr) {
        if (FuncPtr == null) return;
        FuncPtr(ptr);
    }
}

static class Disposer<T> where T : unmanaged, IDisposable, allows ref struct {
    [MethodImpl(Inline)]
    internal static unsafe void Dispose(T* value) {
        value->Dispose();
    }
}