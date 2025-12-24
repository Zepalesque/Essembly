// ReSharper disable InconsistentNaming

using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.MemoryMarshal;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
namespace EsmRuntime.Common.Types;

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
    
