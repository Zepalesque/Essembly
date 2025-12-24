using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;
using static System.Runtime.InteropServices.NativeMemory;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.TypeTables.Signatures;

public readonly ref struct LegacySignature : IDisposable {
    
    [MethodImpl(Inline)]
    unsafe LegacySignature(bool disc, void* data) {
        IsUnion = disc;
        _data = data;
    }
    
    readonly unsafe void* _data;
    
    // ReSharper disable MemberCanBePrivate.Global
    public bool IsPiece { [MethodImpl(Inline)] get => !IsUnion; }
    public bool IsUnion { [MethodImpl(Inline)] get; }
    // ReSharper restore MemberCanBePrivate.Global
    
    
    public unsafe ref Signature AsPiece {
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
    
    unsafe Signature* PiecePtr {
        [MethodImpl(Inline)]
        get => (Signature*)_data;
    }
    
    unsafe SigUnion* UnionPtr {
        [MethodImpl(Inline)]
        get => (SigUnion*)_data;
    }
    
    [MethodImpl(Inline)]
    public static unsafe Box<LegacySignature> SigPiece(ReadOnlySpan<byte> span) {
        var self = (LegacySignature*)AlignedAlloc((nuint) sizeof(LegacySignature), 16);
        
        Signature* data = Signatures.Signature.AllocCopy(span);
        const bool disc = false;
        
        LegacySignature value = new(disc, data);
        *self = value;
        
        return new(self);
    }
    
    [MethodImpl(Inline)]
    public static unsafe Box<LegacySignature> SigUnion(Box<LegacySignature> first, Box<LegacySignature> second) {
        var self = (LegacySignature*)AlignedAlloc((nuint) sizeof(LegacySignature), 16);
        
        SigUnion* data = Signatures.SigUnion.Unite(first, second);
        const bool disc = true;
        
        LegacySignature value = new(disc, data);
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

public readonly unsafe ref struct Signature : IDisposable {
    byte* Start { [MethodImpl(Inline)] get; }
    int Size { [MethodImpl(Inline)] get; }
    
    [MethodImpl(Inline)]
    Signature(byte* start, int size) {
        Start = start;
        Size = size;
    }
    
    [MethodImpl(Inline)]
    ReadOnlySpan<byte> AsSpan() => new(Start, Size);
    
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

public readonly unsafe ref struct SigUnion : IDisposable {
    [MethodImpl(Inline)]
    SigUnion(Box<LegacySignature> first, Box<LegacySignature> second) {
        _first = first;
        _second = second;
        Length = First.Length + Second.Length;
    }
    
    readonly Box<LegacySignature> _first;
    readonly Box<LegacySignature> _second;
    
    public int Length { [MethodImpl(Inline)] get; }
   
    [MethodImpl(Inline)]
    public void CopyTo(Span<byte> span) {
        int split = First.Length;
        First.CopyTo(span[..split]);
        Second.CopyTo(span[split..]);
    }
    
    ref LegacySignature First { [MethodImpl(Inline)] get => ref _first.Value; }
    
    ref LegacySignature Second { [MethodImpl(Inline)] get => ref _second.Value; }
    
    [MethodImpl(Inline)]
    internal static SigUnion* Unite(Box<LegacySignature> first, Box<LegacySignature> second) {
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