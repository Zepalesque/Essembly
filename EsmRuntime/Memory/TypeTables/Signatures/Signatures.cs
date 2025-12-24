using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;
using static System.Runtime.InteropServices.NativeMemory;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.TypeTables.Signatures;

public readonly ref struct Signature : IDisposable {
    
    [MethodImpl(Inline)]
    unsafe Signature(bool disc, void* data) {
        IsUnion = disc;
        _data = data;
    }
    
    readonly unsafe void* _data;
    
    // ReSharper disable MemberCanBePrivate.Global
    public bool IsPiece { [MethodImpl(Inline)] get => !IsUnion; }
    public bool IsUnion { [MethodImpl(Inline)] get; }
    // ReSharper restore MemberCanBePrivate.Global
    
    
    public unsafe ref SigPiece AsPiece {
        [MethodImpl(Inline)]
        get {
            if (IsPiece) return ref *PiecePtr;
            throw new InvalidOperationException();
        }
    }
    
    public unsafe ref SigUnion AsUnion {
        [MethodImpl(Inline)]
        get {
            if (IsUnion) return ref *UnionPtr;
            throw new InvalidOperationException();
        }
    }
    
    unsafe SigPiece* PiecePtr {
        [MethodImpl(Inline)]
        get => (SigPiece*)_data;
    }
    
    unsafe SigUnion* UnionPtr {
        [MethodImpl(Inline)]
        get => (SigUnion*)_data;
    }
    
    [MethodImpl(Inline)]
    public static unsafe Box<Signature> SigPiece(ReadOnlySpan<byte> span) {
        var self = (Signature*)AlignedAlloc((nuint) sizeof(Signature), 16);
        
        SigPiece* data = Signatures.SigPiece.AllocCopy(span);
        const bool disc = false;
        
        Signature value = new(disc, data);
        *self = value;
        
        return new(self);
    }
    
    [MethodImpl(Inline)]
    public static unsafe Box<Signature> SigUnion(Box<Signature> first, Box<Signature> second) {
        var self = (Signature*)AlignedAlloc((nuint) sizeof(Signature), 16);
        
        SigUnion* data = Signatures.SigUnion.Unite(first, second);
        const bool disc = true;
        
        Signature value = new(disc, data);
        *self = value;
        
        return new(self);
    }
    
    [MethodImpl(Inline)]
    public unsafe void Dispose() {
        if (IsPiece) PiecePtr->Dispose();
        else UnionPtr->Dispose();
        
        AlignedFree(_data);
    }
    
    public unsafe int Length {
        [MethodImpl(Inline)]
        get => IsPiece ? PiecePtr->Length : UnionPtr->Length;
    }
    
    [MethodImpl(Inline)]
    public unsafe void CopyTo(Span<byte> span) {
        if (IsPiece) PiecePtr->CopyTo(span);
        else UnionPtr->CopyTo(span);
    }
}

public readonly unsafe ref struct SigPiece : IDisposable {
    byte* Start { [MethodImpl(Inline)] get; }
    int Size { [MethodImpl(Inline)] get; }
    
    [MethodImpl(Inline)]
    SigPiece(byte* start, int size) {
        Start = start;
        Size = size;
    }
    
    [MethodImpl(Inline)]
    ReadOnlySpan<byte> AsSpan() => new(Start, Size);
    
    [MethodImpl(Inline)]
    internal static SigPiece* AllocCopy(ReadOnlySpan<byte> data) {
        var self = (SigPiece*)AlignedAlloc((nuint) sizeof(SigPiece), 16);
        
        var start = (byte*)AlignedAlloc((nuint)data.Length, 16);
        int size = data.Length;
        
        data.CopyTo(new(start, size));
        
        SigPiece value = new(start, size);
        *self = value;
        
        return self;
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

public readonly unsafe ref struct SigUnion : IDisposable {
    [MethodImpl(Inline)]
    SigUnion(Box<Signature> first, Box<Signature> second) {
        _first = first;
        _second = second;
        Length = First.Length + Second.Length;
    }
    
    readonly Box<Signature> _first;
    readonly Box<Signature> _second;
    
    public int Length { [MethodImpl(Inline)] get; }
   
    [MethodImpl(Inline)]
    public void CopyTo(Span<byte> span) {
        int split = First.Length;
        First.CopyTo(span[..split]);
        Second.CopyTo(span[split..]);
    }
    
    ref Signature First { [MethodImpl(Inline)] get => ref _first.Value; }
    
    ref Signature Second { [MethodImpl(Inline)] get => ref _second.Value; }
    
    [MethodImpl(Inline)]
    internal static SigUnion* Unite(Box<Signature> first, Box<Signature> second) {
        var self = (SigUnion*)AlignedAlloc((nuint) sizeof(SigUnion), 16);
        
        SigUnion value = new(first, second);
        *self = value;
        
        return self;
    }
    
    [MethodImpl(Inline)]
    public void Dispose() {
        _first.Dispose();
        _second.Dispose();
    }
}