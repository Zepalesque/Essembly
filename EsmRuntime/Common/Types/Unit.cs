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


    public unsafe void ToPtr(byte* ptr) { }

    public static unsafe Unit FromFatPtr(byte* ptr, usize size) => Self;
    public static unsafe Unit FromPtr(byte* ptr) => Self;

    public static unsafe Unit FromBytecode(byte* start, int* pc) => Self;
    
    public unsafe FatPtr ToBytecode(delegate*<nuint, byte*> generator) => new(null, 0);

    public static usize ByteCount => 0;
}