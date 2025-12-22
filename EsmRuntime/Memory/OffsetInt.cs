namespace EsmRuntime.Memory;

public readonly record struct OffsetInt(nuint? Value) {
    public nuint? Value { get; init; } = Value;
}