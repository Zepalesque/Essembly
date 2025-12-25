// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;
using System.Text;
using EsmRuntime.Memory.Util;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;

namespace EsmRuntime.Common.Types;
[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IPrimValue<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;
    
    public Slice<u8> Utf8 {
        [MethodImpl(Utils.Inline)]
        get => _utf8;
    }
    
    [MethodImpl(Utils.Inline)]
    public static StringSlice FromBytecode(byte* start, scoped ref nuint pc) {
        return new(Slice<u8>.FromBytecode(start, ref pc));
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() {
        return _utf8.ToBytecode();
    }
    
    [MethodImpl(Utils.Inline)]
    public override string ToString() => Encoding.UTF8.GetString(_utf8.Bytes);
    
    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        _utf8.ToPtr(ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    public static StringSlice FromFatPtr(byte* ptr, usize size)
        => new(Slice<u8>.FromFatPtr(ptr, size));
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => _utf8.InstSize;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => false;
    }
    
}

[method: MethodImpl(Utils.Inline)]
public readonly record struct @bool(bool value) : ISizedValue<@bool> {
    [MethodImpl(Utils.Inline)]
    public static implicit operator @bool(bool val) => new(val);
    [MethodImpl(Utils.Inline)]
    public static implicit operator bool(@bool val) => val.value;
    
    public static usize ByteCount {
        [MethodImpl(Utils.Inline)]
        get => 1;
    }
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => ByteCount;
    }
    
    public static bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => true;
    }
    
    [MethodImpl(Utils.Inline)]
    public static unsafe @bool FromPtr(byte* ptr)
        => ReadUnaligned<bool>(ptr);
    
    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr)
        => WriteUnaligned(ptr, value);
    
    [MethodImpl(Utils.Inline)]
    public static unsafe @bool FromFatPtr(byte* ptr, usize size)
        => FromPtr(ptr);
    
    [MethodImpl(Utils.Inline)]
    public static unsafe @bool FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(bool);
        return ReadUnaligned<bool>(start);
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(bool)];
        Write(arr, value);
        return arr;
    }
}