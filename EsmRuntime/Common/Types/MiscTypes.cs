// ReSharper disable InconsistentNaming
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
namespace EsmRuntime.Common.Types;

// internally called bool
public readonly record struct boolean(bool value) : ISizedValue<boolean> {
    
    public static implicit operator boolean(bool val) => new(val);
    public static implicit operator bool(boolean val) => val.value;

    public void ToSpan(Span<byte> span) {
        span[0] = (byte) (value ? 1 : 0);
    }
    public unsafe void ToPtr(byte* ptr) {
        *ptr = (byte) (value ? 1 : 0);
    }

    public static boolean FromSpan(ReadOnlySpan<byte> bytes) 
        => bytes[0] != 0;

    public static unsafe boolean FromPtr(byte* ptr) 
        => *ptr != 0;

    public static int ByteCount => 1;
}
    
