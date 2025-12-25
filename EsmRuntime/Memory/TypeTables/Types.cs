using System.IO.Hashing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Memory.TypeTables;



// OLD ---


public interface IType<out TSelf> : IByteReadable<TSelf>, IDisposable
    where TSelf : struct, IType<TSelf>, allows ref struct {
    public static abstract TypeFlags Flags { get; }
    u128 SigHash { get; }
    
    Signature Sig { get; }
}

public readonly ref struct PrimitiveDynamicType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveDynamicType<T>>, ISizedValue<PrimitiveDynamicType<T>>
    where T : struct, IPrimValue<T>, allows ref struct {
    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveDynamicType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveDynamicType<T> FromPtr(byte* ptr) => throw new NotImplementedException();
    // ReSharper disable once StaticMemberInGenericType
    public static usize ByteCount { [MethodImpl(Utils.Inline)] get; } = usize.ByteCount + sizeof(TypeFlags) + u128.ByteCount;
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    public bool IsConstSize { [MethodImpl(Utils.Inline)] get => true; }
    
    public static TypeFlags Flags { [MethodImpl(Utils.Inline)] get => TypeFlags.DynSize; }
    public u128 SigHash { [MethodImpl(Utils.Inline)] get; } = signature.Hash();
    public Signature Sig { [MethodImpl(Utils.Inline)] get; } = Signature.AllocCopy(signature);
    
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveDynamicType<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new NotImplementedException();
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
    
    public void Dispose() {
        throw new NotImplementedException();
    }
}

public readonly ref struct PrimitiveSizedType<T>(ReadOnlySpan<byte> signature) : IType<PrimitiveSizedType<T>>, ISizedValue<PrimitiveSizedType<T>>
    where T : struct, IPrimValue<T>, allows ref struct {
    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveSizedType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveSizedType<T> FromPtr(byte* ptr) => throw new NotImplementedException();
    // ReSharper disable once StaticMemberInGenericType
    public static usize ByteCount { [MethodImpl(Utils.Inline)] get; } = usize.ByteCount + sizeof(TypeFlags) + u128.ByteCount;
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    public bool IsConstSize { [MethodImpl(Utils.Inline)] get => true; }
    public static TypeFlags Flags { [MethodImpl(Utils.Inline)] get => TypeFlags.ConstSize; }
    public u128 SigHash { [MethodImpl(Utils.Inline)] get; } = signature.Hash();
    public Signature Sig { [MethodImpl(Utils.Inline)] get; } = Signature.AllocCopy(signature);
    
    [MethodImpl(Utils.Inline)]
    public static unsafe PrimitiveSizedType<T> FromBytecode(byte* start, scoped ref nuint pc) => throw new NotImplementedException();
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => throw new NotImplementedException();
    
    public void Dispose() { }
}

[Flags]
public enum TypeFlags : byte {
    DynSize = 0b0001,
    Builtin = 0b0010,
    Generic = 0b0100,
}

public readonly ref struct PtrType<T>(T valType, bool mut) : IType<PtrType<T>>
    where T : struct, IType<T>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe PtrType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    public bool Mut { get; } = mut;
    T ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Utils.Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    public static TypeFlags Flags { [MethodImpl(Utils.Inline)] get => TypeFlags.ConstSize | TypeFlags.Pointer; }
    
    public Signature Sig {
        [MethodImpl(Utils.Inline)]
        get {
            using Signature prefix = Signature.AllocCopy(Mut ? "*mut "u8 : "*"u8);
            using Signature inner = ValType.Sig;
            
            return prefix + inner;
        }
    }
    
    
    public u128 SigHash {
        [MethodImpl(Utils.Inline)]
        get {
            u128 hash = ValType.SigHash;
            
            if (Mut) {
                Buf21<byte> buf = new();
                Span<byte> span = buf;
                "*mut "u8.CopyTo(span[..5]);
                MemoryMarshal.Write(span[5..], hash.value);
                return span.Hash();
            } else {
                Buf21<byte> buf = new();
                Span<byte> span = buf;
                "*"u8.CopyTo(span[..1]);
                MemoryMarshal.Write(span[1..], hash.value);
                return span.Hash();
            }
            
        }
    }
    
    public void Dispose() { }
}

// Not to be confused with objects
public readonly ref struct RefType<T>(T valType, bool mut) : IType<RefType<T>>
    where T : struct, IType<T>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe RefType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    public bool Mut { get; } = mut;
    T ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Utils.Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    public static TypeFlags Flags { [MethodImpl(Utils.Inline)] get => TypeFlags.ConstSize | TypeFlags.Pointer; }
    
    public Signature Sig {
        [MethodImpl(Utils.Inline)]
        get {
            using Signature prefix = Signature.AllocCopy(Mut ? "&mut "u8 : "&"u8);
            using Signature inner = ValType.Sig;
            
            return prefix + inner;
        }
    }
    
    
    public u128 SigHash {
        [MethodImpl(Utils.Inline)]
        get {
            u128 hash = ValType.SigHash;
            
            if (Mut) {
                Buf21<byte> buf = new();
                Span<byte> span = buf;
                "&mut "u8.CopyTo(span[..5]);
                MemoryMarshal.Write(span[5..], hash.value);
                return span.Hash();
            } else {
                Buf21<byte> buf = new();
                Span<byte> span = buf;
                "&"u8.CopyTo(span[..1]);
                MemoryMarshal.Write(span[1..], hash.value);
                return span.Hash();
            }
            
        }
    }
    
    public void Dispose() { }
}

public readonly ref struct SliceType<T>(T valType) : IType<SliceType<T>>
    where T : struct, IType<T>, allows ref struct {
    public unsafe void ToPtr(byte* ptr) => throw new NotImplementedException();
    public static unsafe SliceType<T> FromFatPtr(byte* ptr, usize size) => throw new NotImplementedException();
    
    T ValType { get; } = valType;
    
    public static usize ByteCount { [MethodImpl(Utils.Inline)] get => usize.ByteCount + sizeof(byte) + u128.ByteCount; }
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    
    public static TypeFlags Flags { [MethodImpl(Utils.Inline)] get => TypeFlags.DynamSize | TypeFlags.Slice; }
    
    public Signature Sig {
        [MethodImpl(Utils.Inline)]
        get {
            using Signature inner = ValType.Sig;
            using Signature postfix = Signature.AllocCopy("[]"u8);
            
            return inner + postfix;
        }
    }
    
    public u128 SigHash {
        [MethodImpl(Utils.Inline)]
        get {
            u128 hash = ValType.SigHash;
            
            Buf18<byte> buf = new();
            Span<byte> span = buf;
            MemoryMarshal.Write(span[..^2], hash.value);
            buf[^2] = (byte)'[';
            buf[^1] = (byte)']';
            
            return span.Hash();
        }
    }
    
    public void Dispose() { }
}

public static class TypeUtils {
    
    extension(ReadOnlySpan<byte> span) {
        [MethodImpl(Utils.Inline)]
        public u128 Hash() {
            return XxHash128.HashToUInt128(span);
        }
        
        [MethodImpl(Utils.Inline)]
        public unsafe byte* StartPtr() {
            return (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
        }
    }
    
    extension(Span<byte> span) {
        [MethodImpl(Utils.Inline)]
        public u128 Hash() {
            return XxHash128.HashToUInt128(span);
        }
        
        [MethodImpl(Utils.Inline)]
        public unsafe byte* StartPtr() {
            return (byte*)Unsafe.AsPointer(in MemoryMarshal.GetReference(span));
        }
    }
    
    extension(Signature self) {
        [MethodImpl(Utils.Inline)]
        public u128 Hash() {
            return XxHash128.HashToUInt128(self.AsSpan());
        }
    }
    
    extension<T>(T self)
        where T : struct, IType<T>, allows ref struct {
        
        public SliceType<T> Slice() {
            return new(self);
        }
        
        public RefType<T> Reference() {
            return new(self, false);
        }
    }
    
    extension<TSelf>(TSelf self)
        where TSelf : struct, IType<TSelf>, allows ref struct {

    }
    
    // primitive types
    
    public static PrimitiveSizedType<u8> U8Type { [MethodImpl(Utils.Inline)] get => new("eva/core.u8"u8); }
    public static PrimitiveSizedType<u16> U16Type { [MethodImpl(Utils.Inline)] get => new("eva/core.u16"u8); }
    public static PrimitiveSizedType<u32> U32Type { [MethodImpl(Utils.Inline)] get => new("eva/core.u32"u8); }
    public static PrimitiveSizedType<u64> U64Type { [MethodImpl(Utils.Inline)] get => new("eva/core.u64"u8); }
    public static PrimitiveSizedType<u128> U128Type { [MethodImpl(Utils.Inline)] get => new("eva/core.u128"u8); }
    public static PrimitiveSizedType<usize> UsizeType { [MethodImpl(Utils.Inline)] get => new("eva/core.usize"u8); }
    public static PrimitiveSizedType<i8> I8Type { [MethodImpl(Utils.Inline)] get => new("eva/core.i8"u8); }
    public static PrimitiveSizedType<i16> I16Type { [MethodImpl(Utils.Inline)] get => new("eva/core.i16"u8); }
    public static PrimitiveSizedType<i32> I32Type { [MethodImpl(Utils.Inline)] get => new("eva/core.i32"u8); }
    public static PrimitiveSizedType<i64> I64Type { [MethodImpl(Utils.Inline)] get => new("eva/core.i64"u8); }
    public static PrimitiveSizedType<i128> I128Type { [MethodImpl(Utils.Inline)] get => new("eva/core.i128"u8); }
    public static PrimitiveSizedType<isize> IsizeType { [MethodImpl(Utils.Inline)] get => new("eva/core.isize"u8); }
    public static PrimitiveSizedType<Ptr> RawPtrType { [MethodImpl(Utils.Inline)] get => new("eva/core.isize"u8); }
    public static PrimitiveDynamicType<StringSlice> StrType { [MethodImpl(Utils.Inline)] get => new("eva/core.str"u8); }
}

