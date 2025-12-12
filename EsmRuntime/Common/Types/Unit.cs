namespace EsmRuntime.Common.Types;

public struct Unit: ISpanFormattable, ISizedValue<Unit> {
    static readonly Unit Self = default;

    public string ToString(string? format, IFormatProvider? formatProvider) {
        FormattableString formattable = $"()";
        return formattable.ToString(formatProvider);
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
        => destination.TryWrite(provider, $"()", out charsWritten);

    public override string ToString() => "()";

    public void ToSpan(Span<byte> span) { }
    public unsafe void ToPtr(byte* ptr) { }

    public static Unit FromSpan(ReadOnlySpan<byte> bytes) => Self;
    public static int ByteCount => 0;
    public static unsafe Unit FromPtr(byte* ptr) => Self;
}