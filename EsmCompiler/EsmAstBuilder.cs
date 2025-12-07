using System.Text;
using Antlr4.Runtime;
using EsmCompiler.Util;

namespace EsmCompiler;

public class EsmAstBuilder(CompilationLogger logger, FileData file) : EsmBaseVisitor<BaseNode> {
    
    internal CompilationLogger Logger { get; } = logger;
    FileData File { get; } = file;

    public override StoreLocal VisitStoreToVar(EsmParser.StoreToVarContext context) 
        => new(context.Identifier().GetText(), context.InFile(File));

    public override Goto VisitGoto(EsmParser.GotoContext context)
        => context.condition == null
            ? new(Label(context.label), null, context.InFile(File))
            : new(Label(context.label), context.condition.not == null,
                context.InFile(File));

    public override Exit VisitExit(EsmParser.ExitContext context) 
        => new(context.InFile(File));

    public override FinalizedStmt VisitLoadConst(EsmParser.LoadConstContext context) {
        EsmParser.LiteralContext lit = context.lit;
        FilePos pos = lit.InFile(File);

        if (lit.str != null) {
            var inspecs = LiteralUtil.RegularEscape(pos, lit.str.Text, out var result);
            foreach (var err in inspecs) logger.LogInspection(err);
            return new LoadStr(Encoding.ASCII.GetBytes(result + "\0"), pos);
        }
        

        byte b = 0;
        if (lit.@int != null && !byte.TryParse(lit.@int.Text, out b)) {
            Logger.LogInspection(new InvalidLiteralErr(pos, lit.@int.Text, "integer", null));
        } else if (lit.@char != null) {
            var inspec = LiteralUtil.RegularEscapeChar(pos, lit.@char.Text, out var result);
            if (inspec != null) {
                logger.LogInspection(inspec.Value);
            }

            
            if (result > byte.MaxValue) {
                Logger.LogInspection(new InvalidLiteralErr(pos, lit.@char.Text, "character", null));
            }
            b = unchecked((byte)result);

        }
        
        return new LoadConst(b, context.InFile(File));
    }

    public override Input VisitLoadInput(EsmParser.LoadInputContext context)
        => new(InputMode.Of(context.io.io.Type), context.InFile(File));
    
    public override Print VisitPrint(EsmParser.PrintContext context)
        => new(PrintMode.Of(context.io.io.Type), context.InFile(File));

    public override Pop VisitDiscard(EsmParser.DiscardContext context) 
        => new(context.InFile(File));

    public override LoadLocal VisitLoadLocal(EsmParser.LoadLocalContext context)
        => new(context.loc.Text, context.InFile(File));

    public override DecLocal VisitVarDec(EsmParser.VarDecContext context)
        => new(context.id.Text, context.InFile(File));

    public override SizedStmt VisitToStack(EsmParser.ToStackContext context)
        => (SizedStmt) context.load.Accept(this);

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

    public Span<SizedStmt> Build(EsmParser.BaseContext context) 
        => context.statement().Map(VisitStatement).AsSpan();

    Label Label(IToken token) => new(token.Text, token.InFile(File));
}