using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using EsmCompiler.Util;
using EsmRuntime.Common;

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
    public byte[] Bytes { get; set; } = new[]{(byte) EsmRuntime.Common.OpCode.AllocStr}.Concat(Ascii).ToArray();
    public override ReadOnlySpan<byte> OpCode => Bytes;
    public byte[] Ascii { get; set; } = Ascii;
}

public record AllocMem(string VarName, FixedSizeType Type,  FilePos Pos) : UnfinalizedStmt(0, Pos) {
    public string VarName { get; set; } = VarName;
}

public record PushLocal(string VarName, FixedSizeType Type, FilePos Pos) : UnfinalizedStmt(0, Pos) {
    public string VarName { get; set; } = VarName;
}

public record PushMem(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record PushHeap(byte Index, FilePos Pos) : OneOpStmt(EsmRuntime.Common.OpCode.PushGlobalAddr, Index, Pos) {
    public byte Index { get; set; } = Index;
}

public record StoreIdentifier(string VarName, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public string VarName { get; set; } = VarName;
}

public record StoreMem(ushort Index, FixedSizeType Type, FilePos Pos) : FinalizedStmt(Pos) {
    public ushort Index { get; set; } = Index;
    readonly byte[] _op = [(byte) Type.MemStorage];
    public override ReadOnlySpan<byte> OpCode => _op;
    public FixedSizeType Type { get; set; } = Type;
}


public record Exit(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.Exit, Pos);

public record LabeledStmt(Label Label, SizedStmt Statement, FilePos Pos) : UnfinalizedStmt(Statement.Size, Pos) {
    public Label Label { get; set; } = Label;
    public SizedStmt Statement { get; set; } = Statement;
}


public record Goto(Label Label, bool? IfZero, FilePos Pos) : UnfinalizedStmt(2, Pos) {
    public Label Label { get; set; } = Label;
    public bool? IfZero { get; set; } = IfZero;
}

public record Jump(sbyte Offset, bool? IfZero, FilePos Pos) 
    : OneOpStmt(EsmRuntime.Common.OpCode.get_Item(IfZero), unchecked((byte) Offset), Pos) {
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
            OpCode code = EsmRuntime.Common.OpCode.X64ToX32 + (byte) (start - 3) + i;
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
            OpCode code = (signExtend ? EsmRuntime.Common.OpCode.X8ToI16 : EsmRuntime.Common.OpCode.X8ToU16) + start + i;
            codes[i] = (byte) code;
        }

        return new(codes);
    } 
}



public record Label(string Id, FilePos Pos) : BaseNode(Pos) {
    public string Id { get; set; } = Id;
}

public record BitAnd(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.And8, Pos);
public record BitOr(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.Or8, Pos);
public record BitXor(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.Xor8, Pos);
public record BitNot(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.Not8, Pos);
public record BitLShift(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.Left8, Pos);
public record BitRShift(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.RightI8, Pos);
public record BitUrShift(FilePos Pos) : NoOpStmt(EsmRuntime.Common.OpCode.RightU8, Pos);

public record Print(IoMode Mode, FilePos Pos) : NoOpStmt(Mode.PrintCode, Pos) {
    public IoMode Mode { get; set; } = Mode;
}

public record Input(IoMode Mode, FilePos Pos) : NoOpStmt(Mode.InputCode, Pos) {
    public IoMode Mode { get; set; } = Mode;
}

public enum IoMode {
    Ascii,
    Utf16,
    U8, U16, U32, U64,
    I8, I16, I32, I64,
    Str,
}

public static class PrintModeExt {
    extension(IoMode self) {
        public static IoMode FromToken(int token) => token switch {
            EsmLexer.Ascii => IoMode.Ascii,
            EsmLexer.Utf16 => IoMode.Utf16,
            EsmLexer.U8 => IoMode.U8,
            EsmLexer.U16 => IoMode.U16,
            EsmLexer.U32 => IoMode.U32,
            EsmLexer.U64 => IoMode.U64,
            EsmLexer.I8 => IoMode.I8,
            EsmLexer.I16 => IoMode.I16,
            EsmLexer.I32 => IoMode.I32,
            EsmLexer.I64 => IoMode.I64,
            EsmLexer.Str => IoMode.Str,
            _ => throw new InvalidOperationException()
        };

        public OpCode PrintCode
            => self switch {
                IoMode.Ascii => OpCode.PrintAscii,
                IoMode.Utf16 => OpCode.PrintUtf16,
                IoMode.U8 => OpCode.PrintU8,
                IoMode.U16 => OpCode.PrintU16,
                IoMode.U32 => OpCode.PrintU32,
                IoMode.U64 => OpCode.PrintU64,
                IoMode.I8 => OpCode.PrintI8,
                IoMode.I16 => OpCode.PrintI16,
                IoMode.I32 => OpCode.PrintI32,
                IoMode.I64 => OpCode.PrintI64,
                IoMode.Str => OpCode.PrintStr,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode InputCode
            => self switch {
                IoMode.Ascii => OpCode.InputAscii,
                IoMode.Utf16 => OpCode.InputUtf16,
                IoMode.U8 => OpCode.InputU8,
                IoMode.U16 => OpCode.InputU16,
                IoMode.U32 => OpCode.InputU32,
                IoMode.U64 => OpCode.InputU64,
                IoMode.I8 => OpCode.InputI8,
                IoMode.I16 => OpCode.InputI16,
                IoMode.I32 => OpCode.InputI32,
                IoMode.I64 => OpCode.InputI64,
                IoMode.Str => OpCode.InputStr,
                _ => throw new InvalidOperationException()
            };
    }
}



