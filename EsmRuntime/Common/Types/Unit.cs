using System.Runtime.CompilerServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Common.Types;

public struct Unit: ISpanFormattable, ISizedPrimValue<Unit> {
    static readonly Unit Self = default;

    [MethodImpl(Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) {
        FormattableString formattable = $"()";
        return formattable.ToString(formatProvider);
    }

    [MethodImpl(Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => destination.TryWrite(provider, $"()", out charsWritten);

    [MethodImpl(Inline)]
    public override string ToString() => "()";


    [MethodImpl(Inline)]
    public unsafe void ToPtr(byte* ptr) { }

    [MethodImpl(Inline)]
    public static unsafe Unit FromFatPtr(byte* ptr, usize size) => Self;
    
    [MethodImpl(Inline)]
    public static unsafe Unit FromPtr(byte* ptr) => Self;

    [MethodImpl(Inline)]
    public static unsafe Unit FromBytecode(byte* start, scoped ref int pc) => Self;
    
    [MethodImpl(Inline)]
    public byte[] ToBytecode() => [];

    public static usize ByteCount {
        [MethodImpl(Inline)] get => 0;
    }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Inline)] get => "()"u8; }
}