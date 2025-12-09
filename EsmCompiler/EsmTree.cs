using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using EsmCompiler.Util;
using EsmCore;
using EsmRuntime;

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

public record AllocStr(byte[] Ascii, FilePos Pos) : FinalizedStmt(Pos) {
    public byte[] Bytes { get; set; } = new[]{(byte) EsmRuntime.OpCode.AllocStr}.Concat(Ascii).ToArray();
    public override ReadOnlySpan<byte> OpCode => Bytes;
    public byte[] Ascii { get; set; } = Ascii;
}

public record AllocMem(string VarName, FixedSizeType Type,  FilePos Pos) : UnfinalizedStmt(0, Pos) {
    public string VarName { get; set; } = VarName;
}

public record LocalVar(string VarName, FixedSizeType Type, FilePos Pos) : UnfinalizedStmt(0, Pos) {
    public string VarName { get; set; } = VarName;
}

public record LoadMem(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record LoadHeap(byte Index, FilePos Pos) : OneOpStmt(EsmRuntime.OpCode.LoadMem, Index, Pos) {
    public byte Index { get; set; } = Index;
}

public record StoreIdentifier(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record StoreMem(ushort Index, FixedSizeType Type, FilePos Pos) : FinalizedStmt {
    public ushort Index { get; set; } = Index;
    public override ReadOnlySpan<byte> OpCode => FixedTypeExtIII.
}
public record StoreMem16(int Index, FilePos Pos) : OneOpStmt(EsmRuntime.OpCode.StoreMem16, Index, Pos) {
    public int Index { get; set; } = Index;
}

public record StoreMem32(int Index, FilePos Pos) : OneOpStmt(EsmRuntime.OpCode.StoreMem32, Index, Pos) {
    public int Index { get; set; } = Index;
}

public record StoreMem64(int Index, FilePos Pos) : OneOpStmt(EsmRuntime.OpCode.StoreMem64, Index, Pos) {
    public int Index { get; set; } = Index;
}

public record Exit(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Exit, Pos);

public record LabeledStmt(Label Label, SizedStmt Statement, FilePos Pos) : UnfinalizedStmt(Statement.Size, Pos) {
    public Label Label { get; set; } = Label;
    public SizedStmt Statement { get; set; } = Statement;
}


public record Goto(Label Label, bool? IfZero, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public Label Label { get; set; } = Label;
    public bool? IfZero { get; set; } = IfZero;
}

public record Jump(sbyte Offset, bool? IfZero, FilePos Pos) 
    : OneOpStmt(EsmRuntime.OpCode.get_Item(IfZero), unchecked((byte) Offset), Pos) {
    public bool? IfZero { get; set; } = IfZero;
    public sbyte Offset { get; set; } = Offset;
}

public record IntDemotion(byte Start, byte End, FilePos Pos) 
    : FinalizedStmt(Pos) {
    public override ReadOnlySpan<byte> OpCode => CalculateOpcodes(Start, End);
    
    static ReadOnlySpan<byte> CalculateOpcodes(byte start, byte end) {
        
        int length = end - start;
        var codes = new byte[length];
        for (byte i = 0; i < length; i++) {
            OpCode code = EsmRuntime.OpCode.X64ToX32 + (byte) (start - 3) + i;
            codes[i] = (byte) code;
        }

        return new(codes);
    } 
}

public record IntPromotion(byte Start, byte End, bool SignExtend, FilePos Pos) 
    : FinalizedStmt(Pos) {
    public override ReadOnlySpan<byte> OpCode => CalculateOpcodes(Start, End, SignExtend);
    
    static ReadOnlySpan<byte> CalculateOpcodes(byte start, byte end, bool signExtend) {
        
        int length = end - start;
        var codes = new byte[length];
        for (byte i = 0; i < length; i++) {
            OpCode code = (signExtend ? EsmRuntime.OpCode.X8ToI16 : EsmRuntime.OpCode.X8ToU16) + start + i;
            codes[i] = (byte) code;
        }

        return new(codes);
    } 
}



public record Label(string Id, FilePos Pos) : BaseNode(Pos) {
    public string Id { get; set; } = Id;
}

public record BitAnd(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.And8, Pos);
public record BitOr(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Or8, Pos);
public record BitXor(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Xor8, Pos);
public record BitNot(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Not8, Pos);
public record BitLShift(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Left8, Pos);
public record BitRShift(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.Right8, Pos);
public record BitURShift(FilePos Pos) : NoOpStmt(EsmRuntime.OpCode.RightU8, Pos);

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
    Str,
}

public static class PrintModeExt {
    extension(PrintMode self) {
        public static PrintMode Of(int token) => token switch {
            EsmLexer.Ascii => PrintMode.Ascii,
            EsmLexer.Bin => PrintMode.Binary,
            EsmLexer.Hex => PrintMode.Hex,
            EsmLexer.Dec => PrintMode.Decimal,
            EsmLexer.Str => PrintMode.Str,
            _ => throw new InvalidOperationException()
        };

        public OpCode Code
            => self switch {
                PrintMode.Ascii => OpCode.PrintAscii,
                PrintMode.Binary => OpCode.PrintBin,
                PrintMode.Decimal => OpCode.PrintDec,
                PrintMode.Hex => OpCode.PrintHex,
                PrintMode.Str => OpCode.PrintStr,
                _ => throw new InvalidOperationException()
            };
    }
}


public enum InputMode {
    Ascii,
    Utf16,
    DecimalI8,
    DecimalU8,
    DecimalI16,
    DecimalU16,
    Str,
}

public static class InputModeExt {
    extension(InputMode self) {
        public static InputMode Of(int token) => token switch {
            EsmLexer.Ascii => InputMode.Ascii,
            EsmLexer.Bin => InputMode.Binary,
            EsmLexer.Hex => InputMode.Hex,
            EsmLexer.Dec => InputMode.Decimal,
            EsmLexer.Str => InputMode.Str,
            _ => throw new InvalidOperationException()
        };
        
        public OpCode Code
            => self switch {
                InputMode.Ascii => OpCode.InputAscii,
                InputMode.Binary => OpCode.InputBin,
                InputMode.Decimal => OpCode.InputDec,
                InputMode.Hex => OpCode.InputHex,
                InputMode.Str => OpCode.InputStr,
                _ => throw new InvalidOperationException()
            };
    }
}

