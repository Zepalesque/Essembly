namespace EsmRuntime.Common.Types;

public interface INumberFormattable {
    public string Bin { get; }
    public string Hex { get; }
    public string Dec { get; }
}

public interface IUtf8Formattable<in T> where T : struct, IUtf8Formattable<T>, allows ref struct {
    public static abstract explicit operator char(T value);
}
public interface IUtf16Formattable<in T> where T : struct, IUtf16Formattable<T>, allows ref struct {
    public static abstract explicit operator char(T value);
}