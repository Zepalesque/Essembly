namespace EsmCompiler.Util;



public interface ICompilerInspection {
    public FilePos Pos { get; }
    
    public string Message { get; }
    public string? Hint { get; }

    public bool Fatal => Id.Type == InspecId.Error;
    
    public InspecId Id { get; }

}

public static class InspecIdExtensions {
    extension(InspecId self) {
        public InspecId Type => self & InspecId.InspecType;
        public InspecId Stage => self & InspecId.InspecStage;
        
        public string? AnsiCode => self.Type switch {
            InspecId.Error   => "\e[1;91m",
            InspecId.Warning => "\e[1;93m",
            InspecId.Notice  => "\e[1;96m",
            InspecId.Debug   => "\e[0;37m",
            _ => null
        };
                 
    }
}

[Flags]
public enum InspecId : ushort {
    // Type
    Error        = 0b00100000_00000000,
    Warning      = 0b01000000_00000000,
    Notice       = 0b01100000_00000000,
    Debug        = 0b10000000_00000000,
    InspecType   = Error | Warning | Notice | Debug,
    
    // Stage
    Lexer        = 0b00000100_00000000,
    Syntax       = 0b00001000_00000000,
    Semantic     = 0b00001100_00000000,
    // ReSharper disable once InconsistentNaming
    Compile      = 0b00010100_00000000,
    InspecStage  = Lexer | Syntax | Semantic | Compile,
    
    // Syntax Errors
    ExampleErr =     0x1 | Error | Syntax,
    InvalidEscSeq =  0x2 | Error | Syntax,
    InvalidLiteral = 0x3 | Error | Syntax,
    
    // Syntax Warnings
    PotentialOctal = 0x1 | Warning | Syntax,
    
    // Semantic Errors
    JumpOverVarDec =    0x1 | Error | Semantic,
    VarAlreadyDec =     0x2 | Error | Semantic,
    VarUndef =          0x3 | Error | Semantic,
    LabelAlreadyDec =   0x4 | Error | Semantic,
    LabelUndef =        0x5 | Error | Semantic,
    
    // Semantic Warnings
    UnbreakableLoop =      0x1 | Warning | Semantic,
}



public readonly record struct ExampleError(FilePos Pos) : ICompilerInspection {
    public string Message { get; } = "test :3";
    public string? Hint { get; } = "bottom text";
    public InspecId Id { get; } = InspecId.ExampleErr;
}

public readonly struct InvalidEscapeSeqError(FilePos pos, string escape, string? hint) : ICompilerInspection {
    public string Message { get; } = $"Invalid escape sequence: \'{escape}\'";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = hint;
    public InspecId Id { get; } = InspecId.InvalidEscSeq;
}

public readonly struct InvalidLiteralErr(FilePos pos, string val, string type, string? hint) : ICompilerInspection {
    public string Message { get; } = $"Invalid {type} literal : \"{val}\"";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = hint;
    public InspecId Id { get; } = InspecId.InvalidLiteral;
}

public readonly struct JumpOverVarDec(FilePos pos, string variable, string destination) : ICompilerInspection {
    public string Message { get; } = $"Invalid goto statement: Cannot skip over variable declaration for variable \"{variable}\"";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = $"Move declaration of \"{variable}\" to before the label \"{destination}\"";
    public InspecId Id { get; } = InspecId.JumpOverVarDec;
}

public readonly struct PossibleOctal(FilePos pos, string val) : ICompilerInspection {
    public string Message { get; } = $"Integer literal with traditional octal syntax: \"{val}\" being parsed as decimal";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = $"Replace with \"0o{val[1..]}\" or \"{val[1..]}\"";
    public InspecId Id { get; } = InspecId.PotentialOctal;
}

public readonly struct VarAlreadyDec(FilePos pos, string variable) : ICompilerInspection {
    public string Message { get; } = $"Variable \"{variable}\" has already been defined!";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = $"Remove duplicate declaration of \"{variable}\"";
    public InspecId Id { get; } = InspecId.VarAlreadyDec;
}

public readonly struct LabelAlreadyDec(FilePos pos, string label) : ICompilerInspection {
    public string Message { get; } = $"Label \"{label}\" has already been defined!";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = $"Remove duplicate declaration of \"{label}\"";
    public InspecId Id { get; } = InspecId.LabelAlreadyDec;
}

public readonly struct UndefVar(FilePos pos, string variable, string? hint) : ICompilerInspection {
    public string Message { get; } = $"Variable \"{variable}\" is undefined!";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = hint;
    public InspecId Id { get; } = InspecId.VarUndef;
}

public readonly struct UndefLabel(FilePos pos, string variable, string? hint) : ICompilerInspection {
    public string Message { get; } = $"Label \"{variable}\" is undefined!";
    public FilePos Pos { get; } = pos;
    public string? Hint { get; } = hint;
    public InspecId Id { get; } = InspecId.LabelUndef;
}

public readonly struct TooLargeJump(FilePos pos, int amount, string label) : ICompilerInspection {
    public string Message { get; } = $"Cannot jump {amount} bytecode instructions to \"{label}\", must be within range [{sbyte.MinValue}, {sbyte.MaxValue}]";
    public FilePos Pos { get; } = pos;
    public string? Hint => null;
    public InspecId Id { get; } = InspecId.LabelUndef;
}

public readonly struct UnbreakableLoop(FilePos pos, string label) : ICompilerInspection {
    public string Message { get; } = $"Unbreakable loop back to {label}";
    public FilePos Pos { get; } = pos;
    public string? Hint => null;
    public InspecId Id { get; } = InspecId.UnbreakableLoop;
}
