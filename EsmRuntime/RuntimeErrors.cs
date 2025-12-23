namespace EsmRuntime;

public enum ErrorType : byte {
    StackOverflow  = 0000,
    StackUnderflow = 0001,
    MemoryAccess   = 0010,
    InvalidFormat  = 0011,
    InvalidIndex   = 0100,
    NullAccess     = 0101,
}

public abstract class RuntimeError(string message, ErrorType type) : Exception(message) {
    public readonly ErrorType Type = type;
}

public class StackOverflowError(string message) : RuntimeError(message, ErrorType.StackOverflow);
public class StackUnderflowError(string message) : RuntimeError(message, ErrorType.StackUnderflow);
public class MemoryAccessError(string message) : RuntimeError(message, ErrorType.MemoryAccess);
public class InvalidFormatError(string message) : RuntimeError(message, ErrorType.InvalidFormat);
public class NullAccessError(string message) : RuntimeError(message, ErrorType.NullAccess);
public class InvalidIndexError(string message) : RuntimeError(message, ErrorType.InvalidIndex);