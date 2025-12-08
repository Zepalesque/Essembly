namespace EsmRuntime;

public static class ExitCode {
    public const byte Success      = 0b0000_0000;
    public const byte Failure      = 0b0000_0001;
    public const byte Unterminated = 0b0000_0010;
    public const byte RuntimeError = 0b0001_0000;
    public const byte UserExit     = 0b0001_0000;
}