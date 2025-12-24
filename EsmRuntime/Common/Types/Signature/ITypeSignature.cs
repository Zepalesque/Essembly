using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;
using static System.Runtime.InteropServices.NativeMemory;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types.Signature;

public readonly ref struct TypeSig : IHeapDispose {
    
    [MethodImpl(Inline)]
    unsafe TypeSig(bool disc, void* data) {
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
    public static unsafe RecursiveBox<TypeSig> SigPiece(ReadOnlySpan<byte> span) {
        var self = (TypeSig*)AlignedAlloc((nuint) sizeof(TypeSig), 16);
        
        SigPiece* data = Signature.SigPiece.AllocCopy(span);
        const bool disc = false;
        
        TypeSig value = new(disc, data);
        *self = value;
        
        return new(self);
    }
    
    [MethodImpl(Inline)]
    public static unsafe RecursiveBox<TypeSig> SigUnion(RecursiveBox<TypeSig> first, RecursiveBox<TypeSig> second) {
        var self = (TypeSig*)AlignedAlloc((nuint) sizeof(TypeSig), 16);
        
        SigUnion* data = Signature.SigUnion.Unite(first, second);
        const bool disc = true;
        
        TypeSig value = new(disc, data);
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

public readonly unsafe ref struct SigPiece : IHeapDispose {
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

public readonly unsafe ref struct SigUnion : IHeapDispose {
    [MethodImpl(Inline)]
    SigUnion(RecursiveBox<TypeSig> first, RecursiveBox<TypeSig> second) {
        _first = first;
        _second = second;
        Length = First.Length + Second.Length;
    }
    
    readonly RecursiveBox<TypeSig> _first;
    readonly RecursiveBox<TypeSig> _second;
    
    public int Length { [MethodImpl(Inline)] get; }
   
    [MethodImpl(Inline)]
    public void CopyTo(Span<byte> span) {
        int split = First.Length;
        First.CopyTo(span[..split]);
        Second.CopyTo(span[split..]);
    }
    
    ref TypeSig First { [MethodImpl(Inline)] get => ref _first.Value; }
    
    ref TypeSig Second { [MethodImpl(Inline)] get => ref _second.Value; }
    
    [MethodImpl(Inline)]
    internal static SigUnion* Unite(RecursiveBox<TypeSig> first, RecursiveBox<TypeSig> second) {
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