using System.Runtime.CompilerServices;
using System.Text;
using static EsmRuntime.Constants;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IPrimValue<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;

    public Slice<u8> Utf8 => _utf8;

    public static StringSlice FromBytecode(byte* start, scoped ref int pc) {
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
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "^str"u8; }
}