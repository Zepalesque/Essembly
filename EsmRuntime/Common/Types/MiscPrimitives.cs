// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
namespace EsmRuntime.Common.Types;

#pragma warning restore CS8981
public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IPrimValue<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;
    
    public Slice<u8> Utf8 => _utf8;
    
    public static StringSlice FromBytecode(byte* start, scoped ref nuint pc) {
        return new(Slice<u8>.FromBytecode(start, ref pc));
    }
    
    public byte[] ToBytecode() {
        return _utf8.ToBytecode();
    }
    
    
    public override string ToString() => Encoding.UTF8.GetString(_utf8.Bytes);
    
    public void ToPtr(byte* ptr) {
        _utf8.ToPtr(ptr);
    }
    public static StringSlice FromFatPtr(byte* ptr, usize size)
        => new(Slice<u8>.FromFatPtr(ptr, size));
    
    public nuint InstSize => _utf8.InstSize;
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Constants.Inline)] get => "^str"u8; }
}
#pragma warning disable CS8981

public readonly record struct @bool(bool value) : ISizedValue<@bool> {
    public static implicit operator @bool(bool val) => new(val);
    public static implicit operator bool(@bool val) => val.value;
    
    public static usize ByteCount => 1;
    public nuint InstSize { get; } = ByteCount;
    
    public static unsafe @bool FromPtr(byte* ptr)
        => ReadUnaligned<bool>(ptr);
    
    public unsafe void ToPtr(byte* ptr)
        => WriteUnaligned(ptr, value);
    
    public static unsafe @bool FromFatPtr(byte* ptr, usize size)
        => FromPtr(ptr);
    
    public static unsafe @bool FromBytecode(byte* start, scoped ref nuint pc) {
        pc += sizeof(bool);
        return ReadUnaligned<bool>(start);
    }
    
    public byte[] ToBytecode() {
        var arr = new byte[sizeof(bool)];
        Write(arr, value);
        return arr;
    }
}