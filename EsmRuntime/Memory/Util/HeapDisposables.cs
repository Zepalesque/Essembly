using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.Util;

[method: MethodImpl(Inline)]
public readonly unsafe ref struct RecursiveBox<T>(T* ptr) : IDisposable
    where T : unmanaged, IDisposable, allows ref struct {
    
    public static implicit operator RecursiveBox<T>(T* self) => new(self);
    
    public ref T Value { [MethodImpl(Inline)] get => ref *ptr; }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        if (ptr == null) return;
        
        ptr->Dispose();
        
        NativeMemory.AlignedFree(ptr);
    }
}

[method: MethodImpl(Inline)]
public readonly unsafe ref struct Box<T>(T* ptr) : IDisposable
    where T : unmanaged, allows ref struct {
    
    public static implicit operator Box<T>(T* self) => new(self);
    
    public ref T Value { [MethodImpl(Inline)] get => ref *ptr; }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        if (ptr == null) return;
        NativeMemory.AlignedFree(ptr);
    }
}