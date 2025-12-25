using System.Runtime.CompilerServices;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Common.Types;

public struct Unit: ISpanFormattable, ISizedPrimValue<Unit> {
    static readonly Unit Self = default;

    [MethodImpl(Utils.Inline)]
    public string ToString(string? format, IFormatProvider? formatProvider) {
        FormattableString formattable = $"$()";
        return formattable.ToString(formatProvider);
    }

    [MethodImpl(Utils.Inline)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => destination.TryWrite(provider, $"$()", out charsWritten);

    [MethodImpl(Utils.Inline)]
    public override string ToString() => "$()";


    [MethodImpl(Utils.Inline)]
    public unsafe void ToPtr(byte* ptr) { }

    [MethodImpl(Utils.Inline)]
    public static unsafe Unit FromFatPtr(byte* ptr, usize size) => Self;
    
    [MethodImpl(Utils.Inline)]
    public static unsafe Unit FromPtr(byte* ptr) => Self;

    [MethodImpl(Utils.Inline)]
    public static unsafe Unit FromBytecode(byte* start, scoped ref nuint pc) => Self;
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() => [];

    public static usize ByteCount {
        [MethodImpl(Utils.Inline)] get => 0;
    }
    
    public nuint InstSize { [MethodImpl(Utils.Inline)] get => ByteCount; }
    
    public static ReadOnlySpan<byte> Signature { [MethodImpl(Utils.Inline)] get => "$unit"u8; }
}