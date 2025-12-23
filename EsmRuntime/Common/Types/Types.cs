using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types;

public static class TypeUtils {
    [MethodImpl(Inline)]
    public static u128 Hash(this StringSlice str) {
        return XxHash128.HashToUInt128(str.Utf8.Bytes);
    }
    
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
    
    // primitive signatures
    public static ReadOnlySpan<byte> U8Sig { [MethodImpl(Inline)] get => "^u8"u8; }
    public static ReadOnlySpan<byte> U16Sig { [MethodImpl(Inline)] get => "^u16"u8; }
    public static ReadOnlySpan<byte> U32Sig { [MethodImpl(Inline)] get => "^u32"u8; }
    public static ReadOnlySpan<byte> U64Sig { [MethodImpl(Inline)] get => "^u64"u8; }
    public static ReadOnlySpan<byte> U128Sig { [MethodImpl(Inline)] get => "^u128"u8; }
    public static ReadOnlySpan<byte> UsizeSig { [MethodImpl(Inline)] get => "^usize"u8; }
    public static ReadOnlySpan<byte> I8Sig { [MethodImpl(Inline)] get => "^i8"u8; }
    public static ReadOnlySpan<byte> I16Sig { [MethodImpl(Inline)] get => "^i16"u8; }
    public static ReadOnlySpan<byte> I32Sig { [MethodImpl(Inline)] get => "^i32"u8; }
    public static ReadOnlySpan<byte> I64Sig { [MethodImpl(Inline)] get => "^i64"u8; }
    public static ReadOnlySpan<byte> I128Sig { [MethodImpl(Inline)] get => "^i128"u8; }
    public static ReadOnlySpan<byte> IsizeSig { [MethodImpl(Inline)] get => "^isize"u8; }
    public static ReadOnlySpan<byte> StrSig { [MethodImpl(Inline)] get => "^str"u8; }
    
    // Primitive hashes
    public static readonly u128
          U8Hash = XxHash128.HashToUInt128(U8Sig)
        , U16Hash = XxHash128.HashToUInt128(U16Sig)
        , U32Hash = XxHash128.HashToUInt128(U32Sig)
        , U64Hash = XxHash128.HashToUInt128(U64Sig)
        , U128Hash = XxHash128.HashToUInt128(U128Sig)
        , UsizeHash = XxHash128.HashToUInt128(UsizeSig)
        , I8Hash = XxHash128.HashToUInt128(I8Sig)
        , I16Hash = XxHash128.HashToUInt128(I16Sig)
        , I32Hash = XxHash128.HashToUInt128(I32Sig)
        , I64Hash = XxHash128.HashToUInt128(I64Sig)
        , I128Hash = XxHash128.HashToUInt128(I128Sig)
        , IsizeHash = XxHash128.HashToUInt128(IsizeSig)
        , StrHash = XxHash128.HashToUInt128(StrSig)
          
    ;
    
}

public interface IType<out T> : IByteSerializable<T> where T : struct, IType<T>, allows ref struct {
    TypeFlags Flags { get; }
    u128 SigHash { get; }
    

}


public ref struct PrimitiveType<TValue> : IType<PrimitiveType<TValue>>, ISizedValue<PrimitiveType<TValue>> where TValue: struct, ITypedValue<TValue>, allows ref struct {
    public PrimitiveType() { }
    
    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    [MethodImpl(Inline)]
    public static unsafe PrimitiveType<TValue> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();

    [MethodImpl(Inline)]
    public static unsafe PrimitiveType<TValue> FromPtr(byte* ptr) => throw new NotImplementedException();
    // ReSharper disable once StaticMemberInGenericType
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize; }
    public u128 SigHash { [MethodImpl(Inline)] get => TValue.Signature.Hash(); }
    [MethodImpl(Inline)] 
    public static unsafe PrimitiveType<TValue> FromBytecode(byte* start, int* pc) => throw new NotImplementedException();
    
    [MethodImpl(Inline)] 
    public byte[] ToBytecode() => throw new NotImplementedException();
}

public enum TypeFlags : byte {
    // Kind
    ConstSize = 0b0001, // value type
    DynamSize = 0b0010, // reference type
    Kind = ConstSize | DynamSize,
    
    // Pointer, slice etc
    Pointer  = 0b0100,
    Slice    = 0b1000,
}


public readonly ref struct PointerType<TType>(TType valType) : IType<PointerType<TType>>
    where TType : struct, IType<TType> {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe PointerType<TType> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    TType ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize; }
    public unsafe u128 SigHash {
        [MethodImpl(Inline)]
        get {
            Bytes17 arr = new();
            
            Span<byte> span = arr;
            arr[0] = (byte) '&';
            
            u128 inner = ValType.SigHash;
            inner.ToPtr(span.StartPtr() + 1);
            
            return span.Hash();
        }
    }
}

public readonly ref struct SliceType<TType>(TType valType) : IType<SliceType<TType>>
    where TType : struct, IType<TType> {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe SliceType<TType> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    TType ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Inline)] get => ByteCount; }
    public TypeFlags Flags { [MethodImpl(Inline)] get => TypeFlags.ConstSize; }
    public unsafe u128 SigHash {
        [MethodImpl(Inline)]
        get {
            Bytes18 arr = new();
            
            Span<byte> span = arr;
  
            
            u128 inner = ValType.SigHash;
            inner.ToPtr(span.StartPtr());
            arr[^2] = (byte) '[';
            arr[^1] = (byte) ']';
            
            return span.Hash();
        }
    }
}

[InlineArray(17)]
struct Bytes17 {
    byte _value;
}
[InlineArray(18)]
struct Bytes18 {
    byte _value;
}