namespace EsmRuntime;

public enum ErrorType : byte {
    StackOverflow  = 0000,
    StackUnderflow = 0001,
    MemoryAccess   = 0010,
    InvalidFormat  = 0011,
    InvalidIndex   = 0100,
    NullAccess     = 0101,
}

public abstract class RuntimeError : Exception {
    public readonly ErrorType Type;
    protected RuntimeError(ErrorType type) => Type = type;
    protected RuntimeError(string message, ErrorType type) : base(message) => Type = type;
}

public class StackOverflowError: RuntimeError {
    public StackOverflowError(): base(ErrorType.StackOverflow) {}
    public StackOverflowError(string message) : base(message, ErrorType.StackOverflow) {}
}

public class StackUnderflowError: RuntimeError {
    public StackUnderflowError(): base(ErrorType.StackUnderflow) {}
    public StackUnderflowError(string message) : base(message, ErrorType.StackUnderflow) {}
}

public class MemoryAccessError: RuntimeError {
    public MemoryAccessError() : base(ErrorType.MemoryAccess) {}
    public MemoryAccessError(string message) : base(message, ErrorType.MemoryAccess) {}
}

public class InvalidFormatError: RuntimeError {
    public InvalidFormatError(): base(ErrorType.InvalidFormat) {}
    public InvalidFormatError(string message) : base(message, ErrorType.InvalidFormat) {}
}

public class InvalidIndexError: RuntimeError {
    public InvalidIndexError(): base(ErrorType.InvalidIndex) {}
    public InvalidIndexError(string message) : base(message, ErrorType.InvalidIndex) {}
}

public class NullAccessError : RuntimeError {
    public NullAccessError() : base(ErrorType.NullAccess) {}
    public NullAccessError(string message) : base(message, ErrorType.NullAccess) {}

}