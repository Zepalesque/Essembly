using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EsmCompiler.Util;
using EsmCore;

namespace EsmCompiler;

public abstract record BaseNode(FilePos Pos) : IFilePosHolder {
    FilePos IFilePosHolder.Pos => Pos;
    public FilePos Pos { get; set; } = Pos;
    public FilePos To(IFilePosHolder? other) => Pos.To(other?.Pos);
    public FilePos To(IToken? other) => Pos.To(other);

    public FilePos To(ITerminalNode? other) => Pos.To(other);

    public FilePos To(ParserRuleContext? other) => Pos.To(other);
    
}

public abstract record SizedStmt(FilePos Pos) : BaseNode(Pos) {
    public abstract int Size { get; }
}

public abstract record UnfinalizedStmt(int Size, FilePos Pos) : SizedStmt(Pos) {
    public override int Size { get; } = Size;
}

public abstract record FinalizedStmt(FilePos Pos) : SizedStmt(Pos) {
    public abstract ReadOnlySpan<byte> OpCode { get; }
    public override int Size => OpCode.Length;
}

public abstract record NoOpStmt : FinalizedStmt {
    protected NoOpStmt(OpCode code, FilePos pos) : base(pos)
        => _code = (byte) code;

    public override ReadOnlySpan<byte> OpCode => new(in _code);
    readonly byte _code;
    public void Deconstruct(out OpCode code, out FilePos pos) {
        code = (OpCode) this._code;
        pos = Pos;
    }
}

public abstract record OneOpStmt : FinalizedStmt {
    protected OneOpStmt(OpCode code, byte operand, FilePos pos) : base(pos) 
        => _bytes = [(byte) code, operand];

    readonly byte[] _bytes;
    
    public override ReadOnlySpan<byte> OpCode => _bytes;
    public void Deconstruct(out OpCode code, out byte operand, out FilePos pos) {
        code = (OpCode) _bytes[0];
        operand = _bytes[1];
        pos = Pos;
    }
}

public record LoadConst(byte Val, FilePos Pos) : OneOpStmt(EsmCore.OpCode.LoadConst, Val, Pos) {
    public byte Val { get; set; } = Val;
}

public record LoadStr(byte[] Ascii, FilePos Pos) : FinalizedStmt(Pos) {
    public byte[] Bytes { get; set; } = new byte[]{(byte) EsmCore.OpCode.LoadStr}.Concat(Ascii).ToArray();
    public override ReadOnlySpan<byte> OpCode => Bytes;
    public byte[] Ascii { get; set; } = Ascii;
}

public record DecLocal(string VarName, FilePos Pos) : UnfinalizedStmt(0, Pos) {
    public string VarName { get; set; } = VarName;
}

public record LoadLocal(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record LoadMem(byte Index, FilePos Pos) : OneOpStmt(EsmCore.OpCode.LoadMem, Index, Pos) {
    public byte Index { get; set; } = Index;
}

public record StoreLocal(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record StoreMem(byte Index, FilePos Pos) : OneOpStmt(EsmCore.OpCode.Store, Index, Pos) {
    public byte Index { get; set; } = Index;
}

public record Pop(FilePos Pos) : NoOpStmt(EsmCore.OpCode.PopTop, Pos);
public record Exit(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Exit, Pos);

public record LabeledStmt(Label Label, SizedStmt Statement, FilePos Pos) : UnfinalizedStmt(Statement.Size, Pos) {
    public Label Label { get; set; } = Label;
    public SizedStmt Statement { get; set; } = Statement;
}


public record Goto(Label Label, bool? IfZero, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public Label Label { get; set; } = Label;
    public bool? IfZero { get; set; } = IfZero;
}

public record Jump(sbyte Offset, bool? IfZero, FilePos Pos) 
    : OneOpStmt(IfZero switch {
        null => EsmCore.OpCode.Jump,
            true => EsmCore.OpCode.JumpIfZero,
            false => EsmCore.OpCode.JumpIfNZero
    }, unchecked((byte) Offset), Pos) {
    public bool? IfZero { get; set; } = IfZero;
    public sbyte Offset { get; set; } = Offset;
}

public record Label(string Id, FilePos Pos) : BaseNode(Pos) {
    public string Id { get; set; } = Id;
}

public record BitAnd(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do2And, Pos);
public record BitOr(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do2Or, Pos);
public record BitXor(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do2Xor, Pos);
public record BitNot(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do1Not, Pos);
public record BitLShift(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do2Left, Pos);
public record BitRShift(FilePos Pos) : NoOpStmt(EsmCore.OpCode.Do2Right, Pos);

public record Print(PrintMode Mode, FilePos Pos) : NoOpStmt(Mode.Code, Pos) {
    public PrintMode Mode { get; set; } = Mode;
}

public record Input(InputMode Mode, FilePos Pos) : NoOpStmt(Mode.Code, Pos) {
    public InputMode Mode { get; set; } = Mode;
}

public enum PrintMode {
    Ascii,
    Binary,
    Hex,
    Decimal,
}

public static class PrintModeExt {
    extension(PrintMode self) {
        public static PrintMode Of(int token) => token switch {
            EsmLexer.Ascii => PrintMode.Ascii,
            EsmLexer.Bin => PrintMode.Binary,
            EsmLexer.Hex => PrintMode.Hex,
            EsmLexer.Dec => PrintMode.Decimal,
            _ => throw new InvalidOperationException()
        };

        public OpCode Code
            => self switch {
                PrintMode.Ascii => OpCode.PrintAscii,
                PrintMode.Binary => OpCode.PrintBin,
                PrintMode.Decimal => OpCode.PrintDec,
                PrintMode.Hex => OpCode.PrintHex,
                _ => throw new InvalidOperationException()
            };
    }
}


public enum InputMode {
    Ascii,
    Binary,
    Hex,
    Decimal,
    Int,
    Str,
}

public static class InputModeExt {
    extension(InputMode self) {
        public static InputMode Of(int token) => token switch {
            EsmLexer.Ascii => InputMode.Ascii,
            EsmLexer.Bin => InputMode.Binary,
            EsmLexer.Hex => InputMode.Hex,
            EsmLexer.Dec => InputMode.Decimal,
            EsmLexer.Int => InputMode.Int,
            EsmLexer.Str => InputMode.Str,
            _ => throw new InvalidOperationException()
        };
        
        public OpCode Code
            => self switch {
                InputMode.Ascii => OpCode.InputAscii,
                InputMode.Binary => OpCode.InputBin,
                InputMode.Decimal => OpCode.InputDec,
                InputMode.Hex => OpCode.InputHex,
                InputMode.Int => OpCode.InputInt,
                InputMode.Str => OpCode.InputStr,
                _ => throw new InvalidOperationException()
            };
    }
}

