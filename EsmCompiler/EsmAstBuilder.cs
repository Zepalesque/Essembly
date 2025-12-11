using Antlr4.Runtime;
using EsmCompiler.Util;
using EsmRuntime;

namespace EsmCompiler;

public class EsmAstBuilder(CompilationLogger logger, FileData file) : EsmParserBaseVisitor<BaseNode> {
    
    internal CompilationLogger Logger { get; } = logger;
    FileData File { get; } = file;

    
    
    public override StoreIdentifier VisitStoreToPointer(EsmParser.StoreToPointerContext context) 
        => new(context.Identifier().GetText(), context.InFile(File));

    public override Goto VisitGoto(EsmParser.GotoContext context)
        => context.condition == null
            ? new(Label(context.label), null, context.InFile(File))
            : new(Label(context.label), context.condition.value.Type == EsmLexer.True,
                context.InFile(File));

    public override Exit VisitExit(EsmParser.ExitContext context) 
        => new(context.InFile(File));
    
    

    public override FinalizedStmt VisitPushConst(EsmParser.PushConstContext context) {
        EsmParser.SizedLiteralContext lit = context.lit;
        FilePos pos = lit.InFile(File);
        FixedSizeType type = FixedSizeType.ByIndex(context.type.type.Type);
        
        /*if (lit.str != null) {
            var inspecs = LiteralUtil.RegularEscape(pos, lit.str.Text, out var result);
            foreach (var err in inspecs) logger.LogInspection(err);
            return new LoadStr(Encoding.ASCII.GetBytes(result + "\0"), pos);
        }
        */


        ConstantNode node;
        if (lit.@int != null) {
            var inspecs = LiteralUtil.ParseInt(pos, lit.@int.Text, type, out node);
            foreach (var err in inspecs) Logger.LogInspection(err);
        } else if (lit.@char != null) {
            var inspec = LiteralUtil.RegularEscapeChar(pos, lit.@char.Text, out char result);
            if (inspec != null) {
                Logger.LogInspection(inspec.Value);
            }


            if (type == FixedSizeType.U8 || type == FixedSizeType.I8 || result > byte.MaxValue) {
                Logger.LogInspection(new InvalidLiteralErr(pos, lit.@char.Text, "8-bit character", null));
            }

            node = LiteralUtil.CreateIntLiteral(pos, result, type);
        } else throw new InvalidOperationException();
        
        return node;
    }

    // public override Input VisitPushInput(EsmParser.PushInputContext context)
        // => new(IoMode.FromToken(context.io.io.Type), context.InFile(File));
    
    public override Print VisitPrint(EsmParser.PrintContext context)
        => new(IoMode.FromToken(context.io.io.Type), context.InFile(File));
    
    public override LoadMem VisitPushMem(EsmParser.PushMemContext context)
        => new(context.loc.Text, context.InFile(File));

    public override AllocMem VisitAllocMem(EsmParser.AllocMemContext context)
        => new(context.id.Text, FixedSizeType.ByIndex(context.type.type.Type), context.InFile(File));

    public override SizedStmt VisitToStack(EsmParser.ToStackContext context)
        => (SizedStmt) context.push.Accept(this);

    public override SizedStmt VisitOperationPerform(EsmParser.OperationPerformContext context) {
        FilePos pos = context.InFile(File);
        return context.op.op.Type switch {
            EsmLexer.BwAnd => new BitAnd(pos),
            EsmLexer.BwOr => new BitOr(pos),
            EsmLexer.BwXor => new BitXor(pos),
            EsmLexer.BwNot => new BitNot(pos),
            EsmLexer.BwLShift => new BitLShift(pos),
            EsmLexer.BwRShift => new BitRShift(pos),
            _ => throw new InvalidOperationException()
        };
    }

    public override SizedStmt VisitStatement(EsmParser.StatementContext context) { 
        SizedStmt stmt = (SizedStmt) context.stmt.Accept(this);
        return context.label != null ? new LabeledStmt(Label(context.label), stmt, context.InFile(File)) : stmt;
    }

    // public Span<SizedStmt> Build(EsmParser.BaseContext context) 
        // => context.statement().Map(VisitStatement).AsSpan();

    Label Label(IToken token) => new(token.Text, token.InFile(File));
}