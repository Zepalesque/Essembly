// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types;
[method: MethodImpl(Inline)]
public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IPrimValue<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;
    
    public Slice<u8> Utf8 {
        [MethodImpl(Inline)]
        get => _utf8;
    }
    
    [MethodImpl(Inline)]
    public static StringSlice FromBytecode(byte* start, scoped ref nuint pc) {
        return new(Slice<u8>.FromBytecode(start, ref pc));
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        return _utf8.ToBytecode();
    }
    
    [MethodImpl(Inline)]
    public override string ToString() => Encoding.UTF8.GetString(_utf8.Bytes);
    
    [MethodImpl(Inline)]
    public void ToPtr(byte* ptr) {
        _utf8.ToPtr(ptr);
    }
    
    [MethodImpl(Inline)]
    public static StringSlice FromFatPtr(byte* ptr, usize size)
        => new(Slice<u8>.FromFatPtr(ptr, size));
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => _utf8.InstSize;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Inline)]
        get => false;
    }
    
}

[method: MethodImpl(Inline)]
public readonly record struct @bool(bool value) : ISizedValue<@bool> {
    [MethodImpl(Inline)]
    public static implicit operator @bool(bool val) => new(val);
    [MethodImpl(Inline)]
    public static implicit operator bool(@bool val) => val.value;
    
    public static usize ByteCount {
        [MethodImpl(Inline)]
        get => 1;
    }
    
    public nuint InstSize {
        [MethodImpl(Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Inline)]
        get => true;
    }
    
    [MethodImpl(Inline)]
    public static unsafe @bool FromPtr(byte* ptr)
        => ReadUnaligned<bool>(ptr);
    
    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr)
        => WriteUnaligned(ptr, value);
    
    [MethodImpl(Inline)]
    public static unsafe @bool FromFatPtr(byte* ptr, usize size)
        => FromPtr(ptr);
    
    [MethodImpl(Inline)]
    public static unsafe @bool FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(bool);
        return ReadUnaligned<bool>(start);
    }
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(bool)];
        Write(arr, value);
        return arr;
    }
}