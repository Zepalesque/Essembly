using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.Util;

[method: MethodImpl(Inline)]
public readonly unsafe ref struct Box<T>(T* ptr) : IDisposable
    where T : unmanaged, allows ref struct {
    
    public ref T Value { [MethodImpl(Inline)] get => ref *ptr; }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        if (ptr == null) return;
        
        BoxDispose<T>.TryDispose(ptr);
        
        NativeMemory.AlignedFree(ptr);
    }
}

static class BoxDispose<T> where T : unmanaged, allows ref struct {
    // ReSharper disable once StaticMemberInGenericType
    static readonly unsafe delegate*<T*, void> FuncPtr;
    
    [MethodImpl(Inline)]
    static unsafe BoxDispose() {
        if (!typeof(IDisposable).IsAssignableFrom(typeof(T))) return;
        var func = (void*) typeof(Disposer<>).MakeGenericType(typeof(T)).GetMethod(nameof(Disposer<>.Dispose))!.MethodHandle.GetFunctionPointer();
        
        FuncPtr = (delegate*<T*, void>) func;
    }
    
    [MethodImpl(Inline)]
    public static unsafe void TryDispose(T* ptr) {
        if (FuncPtr == null) return;
        FuncPtr(ptr);
    }
}

static class Disposer<T> where T : unmanaged, IDisposable, allows ref struct {
    [MethodImpl(Inline)]
    public static unsafe void Dispose(T* value) {
        value->Dispose();
    }
}