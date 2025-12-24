using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types.Signature;
using EsmRuntime.Memory.Util;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types;

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
    
    extension<TSelf, TValue>(TSelf self)
        where TValue : struct, ISizedTypeValue<TValue>, allows ref struct
        where TSelf : struct, IType<TSelf, TValue>, allows ref struct {
        
        public SliceType<TSelf, TValue> Slice() {
            return new(self);
        }
    }
    
    extension<TSelf, TValue>(TSelf self)
        where TValue : struct, ITypedValue<TValue>, allows ref struct
        where TSelf : struct, IType<TSelf, TValue>, allows ref struct {
        public ReferenceType<TSelf, TValue> Reference() {
            return new(self);
        }
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
    public static PrimitiveDynamicType<StringSlice> StrType { [MethodImpl(Inline)] get => new(StringSlice.Signature); }
}

public interface IType<out TSelf, TValue> : IByteSerializable<TSelf> 
    where TSelf : struct, IType<TSelf, TValue>, allows ref struct 
    where TValue : struct, ITypedValue<TValue>, allows ref struct {
    TypeFlags Flags { get; }
    u128 SigHash { get; }
    
    RecursiveBox<TypeSig> Signature { get; }
    
}

public readonly ref struct PrimitiveDynamicType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveDynamicType<T>, T>, ISizedValue<PrimitiveDynamicType<T>>
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
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.DynamSize; }
    public u128 SigHash { [MethodImpl(Inline)] get; } = signature.Hash();
    public RecursiveBox<TypeSig> Signature { [MethodImpl(Inline)] get; } = TypeSig.SigPiece(signature);
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveDynamicType<T> FromBytecode(byte* start, scoped ref int pc) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
}

public readonly ref struct PrimitiveSizedType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveSizedType<T>, T>, ISizedValue<PrimitiveSizedType<T>>
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
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize; }
    public u128 SigHash { [MethodImpl(Inline)] get; } = signature.Hash();
    public RecursiveBox<TypeSig> Signature { [MethodImpl(Inline)] get; } = TypeSig.SigPiece(signature);
    
    [MethodImpl(Inline)]
    public static unsafe PrimitiveSizedType<T> FromBytecode(byte* start, scoped ref int pc) => throw new NotImplementedException();
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
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
public readonly ref struct ReferenceType<TType, TValue>(TType valType) : IType<ReferenceType<TType, TValue>, Reference<TValue>>
    where TValue : struct, ITypedValue<TValue>, allows ref struct
    where TType : struct, IType<TType, TValue>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe ReferenceType<TType, TValue> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    TType ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize | TypeFlags.Pointer; }
    
    public RecursiveBox<TypeSig> Signature => TypeSig.SigUnion(TypeSig.SigPiece("&"u8), ValType.Signature);
    
    
    public unsafe u128 SigHash {
        [MethodImpl(Inline)] // useless thanks to stackalloc :/
        get {
            using RecursiveBox<TypeSig> sig = Signature;
            
            int length = sig.Value.Length;
            Span<byte> span = stackalloc byte[length];
            sig.Value.CopyTo(span);
            
            return span.Hash();
        }
    }
}

public readonly ref struct SliceType<TType, TValue>(TType valType) : IType<SliceType<TType, TValue>, Slice<TValue>>
    where TValue : struct, ISizedTypeValue<TValue>, allows ref struct
    where TType : struct, IType<TType, TValue>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe SliceType<TType, TValue> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    TType ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.DynamSize | TypeFlags.Slice; }
    
    public RecursiveBox<TypeSig> Signature {
        [MethodImpl(Inline)]
        get => TypeSig.SigUnion(ValType.Signature, TypeSig.SigPiece("[]"u8));
    }
    
    public unsafe u128 SigHash {
        [MethodImpl(Inline)] // useless thanks to stackalloc :/
        get {
            using RecursiveBox<TypeSig> sig = Signature;
            
            int length = sig.Value.Length;
            Span<byte> span = stackalloc byte[length];
            sig.Value.CopyTo(span);
            
            return span.Hash();
        }
    }
}