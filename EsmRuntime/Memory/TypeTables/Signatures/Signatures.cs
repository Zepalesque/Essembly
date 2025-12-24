using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;
using static System.Runtime.InteropServices.NativeMemory;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.TypeTables.Signatures;

public readonly unsafe ref struct Signature : IDisposable {
    byte* Start { [MethodImpl(Inline)] get; }
    int Size { [MethodImpl(Inline)] get; }
    
    [MethodImpl(Inline)]
    Signature(byte* start, int size) {
        Start = start;
        Size = size;
    }
    
    [MethodImpl(Inline)]
    internal ReadOnlySpan<byte> AsSpan() => new(Start, Size);
    
    [MethodImpl(Inline)]
    internal static Signature AllocCopy(scoped ReadOnlySpan<byte> data) {
        
        int length = data.Length;
        
        var start = (byte*)AlignedAlloc((nuint)length, 16);
        data.CopyTo(new(start, length));
        Signature value = new(start, length);
        
        return value;
    }
    
    /*public static explicit operator Signature(scoped ReadOnlySpan<byte> data)
        => AllocCopy(data);   
    
    public static explicit operator Signature(scoped Span<byte> data)
        => AllocCopy(data);*/
    
    public static Signature operator +(Signature a, Signature b) {
        Span<byte> concat = stackalloc byte[a.Length + b.Length];
        int split = a.Length;
        a.CopyTo(concat[..split]);
        b.CopyTo(concat[split..]);
        
        
        return AllocCopy(concat);
    }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        AlignedFree(Start);
    }
    
    public int Length {
        [MethodImpl(Inline)] get => Size;
    }
    
    [MethodImpl(Inline)]
    public void CopyTo(Span<byte> span) {
        AsSpan().CopyTo(span);
    }
}