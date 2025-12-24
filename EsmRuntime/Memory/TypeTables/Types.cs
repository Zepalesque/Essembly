using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.TypeTables;

public readonly ref struct TypeHandle {
    
    
}


public interface IType<out TSelf> : IByteReadable<TSelf>, IDisposable
    where TSelf : struct, IType<TSelf>, allows ref struct {
    public static abstract TypeFlags Flags { get; }
    u128 SigHash { get; }
    
    Box<LegacySignature> Signature { get; }
}

public readonly ref struct PrimitiveDynamicType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveDynamicType<T>>, ISizedValue<PrimitiveDynamicType<T>>
    where T : struct, IPrimValue<T>, allows ref struct {
    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    [MethodImpl(Inline)]
    public static unsafe PrimitiveDynamicType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveDynamicType<T> FromPtr(byte* ptr) => throw new NotImplementedException();
    // ReSharper disable once StaticMemberInGenericType
    public static usize ByteCount { [MethodImpl(Inline)] get; } = usize.ByteCount + sizeof(TypeFlags) + u128.ByteCount;
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public bool IsConstSize { [MethodImpl(Inline)] get => true; }
    
    public static TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.DynamSize; }
    public u128 SigHash { [MethodImpl(Inline)] get; } = signature.Hash();
    public Box<LegacySignature> Signature { [MethodImpl(Inline)] get; } = Signatures.LegacySignature.SigPiece(signature);
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveDynamicType<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
    
    public void Dispose() {
        throw new NotImplementedException();
    }
}

public readonly ref struct PrimitiveSizedType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveSizedType<T>>, ISizedValue<PrimitiveSizedType<T>>
    where T : struct, IPrimValue<T>, allows ref struct {
    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    [MethodImpl(Inline)]
    public static unsafe PrimitiveSizedType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveSizedType<T> FromPtr(byte* ptr) => throw new NotImplementedException();
    // ReSharper disable once StaticMemberInGenericType
    public static usize ByteCount { [MethodImpl(Inline)] get; } = usize.ByteCount + sizeof(TypeFlags) + u128.ByteCount;
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public bool IsConstSize { [MethodImpl(Inline)] get => true; }
    public static TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize; }
    public u128 SigHash { [MethodImpl(Inline)] get; } = signature.Hash();
    public Box<LegacySignature> Signature { [MethodImpl(Inline)] get; } = Signatures.LegacySignature.SigPiece(signature);
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveSizedType<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
    
    public void Dispose() { }
}

[Flags]
public enum TypeFlags : byte {
    // Kind
    ConstSize = 0b0001, // value type
    DynamSize = 0b0010, // reference type
    Kind = ConstSize | DynamSize,
    
    // Pointer, slice etc
    Pointer  = 0b0100,
    Slice    = 0b1000,
}

// Not to be confused with objects
public readonly ref struct RefType<T>(T valType) : IType<RefType<T>>
    where T : struct, IType<T>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe RefType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    T ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public static TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize | TypeFlags.Pointer; }
    
    public Box<LegacySignature> Signature => Signatures.LegacySignature.SigUnion(Signatures.LegacySignature.SigPiece("&"u8), ValType.Signature);
    
    
    public unsafe u128 SigHash {
        [MethodImpl(Inline)] // useless thanks to stackalloc :/
        get {
            using Box<LegacySignature> sig = Signature;
            
            int length = sig.Value.Length;
            Span<byte> span = stackalloc byte[length];
            sig.Value.CopyTo(span);
            
            return span.Hash();
        }
    }
    
    public void Dispose() { }
}

public readonly ref struct SliceType<T>(T valType) : IType<SliceType<T>>
    where T : struct, IType<T>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe SliceType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    T ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    
    public static TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.DynamSize | TypeFlags.Slice; }
    
    public Box<LegacySignature> Signature {
        [MethodImpl(Inline)]
        get => Signatures.LegacySignature.SigUnion(ValType.Signature, Signatures.LegacySignature.SigPiece("[]"u8));
    }
    
    public unsafe u128 SigHash {
        [MethodImpl(Inline)] // useless thanks to stackalloc :/
        get {
            using Box<LegacySignature> sig = Signature;
            
            int length = sig.Value.Length;
            Span<byte> span = stackalloc byte[length];
            sig.Value.CopyTo(span);
            
            return span.Hash();
        }
    }
    
    public void Dispose() { }
}

public static class TypeUtils {
    
    extension(ReadOnlySpan<byte> span) {
        [MethodImpl(Inline)]
        public u128 Hash() {
            return XxHash128.HashToUInt128(span);
        }
        
        [MethodImpl(Inline)]
        public unsafe byte* StartPtr() {
            return (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
        }
    }
    
    extension(Span<byte> span) {
        [MethodImpl(Inline)]
        public u128 Hash() {
            return XxHash128.HashToUInt128(span);
        }
        
        [MethodImpl(Inline)]
        public unsafe byte* StartPtr() {
            return (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
        }
    }
    
    extension<T>(T self)
        where T : struct, IType<T>, allows ref struct {
        
        public SliceType<T> Slice() {
            return new(self);
        }
        
        public RefType<T> Reference() {
            return new(self);
        }
    }
    
    extension<TSelf>(TSelf self)
        where TSelf : struct, IType<TSelf>, allows ref struct {

    }
    
    // primitive types
    
    public static PrimitiveSizedType<u8> U8Type { [MethodImpl(Inline)] get => new(u8.Signature); }
    public static PrimitiveSizedType<u16> U16Type { [MethodImpl(Inline)] get => new(u16.Signature); }
    public static PrimitiveSizedType<u32> U32Type { [MethodImpl(Inline)] get => new(u32.Signature); }
    public static PrimitiveSizedType<u64> U64Type { [MethodImpl(Inline)] get => new(u64.Signature); }
    public static PrimitiveSizedType<u128> U128Type { [MethodImpl(Inline)] get => new(u128.Signature); }
    public static PrimitiveSizedType<usize> UsizeType { [MethodImpl(Inline)] get => new(usize.Signature); }
    public static PrimitiveSizedType<i8> I8Type { [MethodImpl(Inline)] get => new(i8.Signature); }
    public static PrimitiveSizedType<i16> I16Type { [MethodImpl(Inline)] get => new(i16.Signature); }
    public static PrimitiveSizedType<i32> I32Type { [MethodImpl(Inline)] get => new(i32.Signature); }
    public static PrimitiveSizedType<i64> I64Type { [MethodImpl(Inline)] get => new(i64.Signature); }
    public static PrimitiveSizedType<i128> I128Type { [MethodImpl(Inline)] get => new(i128.Signature); }
    public static PrimitiveSizedType<isize> IsizeType { [MethodImpl(Inline)] get => new(isize.Signature); }
    public static PrimitiveSizedType<Ptr> RawPtrType { [MethodImpl(Inline)] get => new(isize.Signature); }
    public static PrimitiveDynamicType<StringSlice> StrType { [MethodImpl(Inline)] get => new(StringSlice.Signature); }
}