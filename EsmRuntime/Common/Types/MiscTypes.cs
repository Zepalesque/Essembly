// ReSharper disable InconsistentNaming

using static System.Runtime.CompilerServices.Unsafe;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
namespace EsmRuntime.Common.Types;

// internally called bool
public readonly record struct @bool(bool value) : ISizedValue<@bool> {
    
    public static implicit operator @bool(bool val) => new(val);
    public static implicit operator bool(@bool val) => val.value;

    public static usize ByteCount => 1;
    public static unsafe @bool FromPtr(byte* ptr)
        => ReadUnaligned<bool>(ptr);

    public unsafe void ToPtr(byte* ptr) 
        => WriteUnaligned(ptr, value);

    public static unsafe @bool FromFatPtr(byte* ptr, usize size)
        => FromPtr(ptr);

    public static unsafe @bool FromBytecode(byte* start, int* pc) {
        *pc += sizeof(bool);
        return ReadUnaligned<bool>(start);
    }

    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        byte* ptr = generator(sizeof(bool));
        WriteUnaligned(ptr, value);
        return new(ptr, sizeof(bool));
    }
}
    
