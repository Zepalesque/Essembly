using EsmCompiler.Util;
using EsmRuntime;
using Quickenshtein;

namespace EsmCompiler;

public static partial class EsmCompiler {
    static ReadOnlySpan<StmtInfo> Analyze(Span<SizedStmt> stmts, CompilationLogger logger) {
        Dictionary<string, VariableInfo> locals = new();
        Dictionary<string, VariableInfo> memory = new();
        Dictionary<string, int> labels = new();
        List<StmtInfo> stmtInfo = [];
        
        var index = 0;
        for (var i = 0; i < stmts.Length; i++) {
            ref SizedStmt stmt = ref stmts[i];
            FilePos pos = stmt.Pos;
            if (stmt is LabeledStmt(var (label, _), var other, _)) {
                if (!labels.TryAdd(label, index)) {
                    logger.LogInspection(new LabelAlreadyDec(pos, label));
                }

                if (other is Goto(var lbl, _, _) && lbl.Id == label) {
                    logger.LogInspection(new UnbreakableLoop(stmt.Pos, label));
                }
                
                stmt = other;
            }
            
            
            if (stmt is AllocMem(var name, var type, _)) {
                var id = (byte) locals.Count;
                if (!locals.TryAdd(name, new(index, type, id))) {
                    logger.LogInspection(new VarAlreadyDec(pos, name));
                }
            } else if (stmt is FinalizedStmt f) {
                stmtInfo.Add(new(index, f));
            }
            
            index += stmt.Size;
        }

        index = 0;
        for (var i = 0; i < stmts.Length; i++) {
            ref SizedStmt stmt = ref stmts[i];
            FilePos pos = stmt.Pos;
            
            if (stmt is StoreIdentifier(var name, _)) {
                if (!locals.TryGetValue(name, out VariableInfo info)) {
                    string[] typos = locals.Keys.Filter(s => TypoPossible(name, s));
                    string? hint = typos.Length == 0
                        ? null
                        : $"Did you mean any of: [{typos.Aggregate((s, s1) => $"{s}, {s1}")}]?";
                    
                    logger.LogInspection(new UndefVar(pos, name, hint));
                } else {
                    var mem = new StoreMem(info.Id, info.Type, pos);
                    stmtInfo.Add(new(index, mem));
                    stmt = mem;
                }
            } else if (stmt is LoadMem(var name1, _)) {
                if (!locals.TryGetValue(name1, out VariableInfo info)) {
                    string[] typos = locals.Keys.Filter(s => TypoPossible(name1, s));
                    string? hint = typos.Length == 0
                        ? null
                        : $"Did you mean any of: [{typos.Aggregate((s, s1) => $"{s}, {s1}")}]?";
                    
                    logger.LogInspection(new UndefVar(pos, name1, hint));
                } else {
                    var mem = new LoadHeap(info.Id, pos);
                    stmtInfo.Add(new(index, mem));
                    stmt = mem;
                }
            } else if (stmt is Goto(var (label, _), var cond, _)) {
                if (!labels.TryGetValue(label, out int labelPos)) {
                    string[] typos = labels.Keys.Filter(s => TypoPossible(label, s));
                    string? hint = typos.Length == 0
                        ? null
                        : $"Did you mean any of: [{typos.Aggregate((s, s1) => $"{s}, {s1}")}]?";
                    
                    logger.LogInspection(new UndefLabel(pos, label, hint));
                } else {
                    int offset = labelPos - index;

                    var vars = locals.ToArray();
                    int aux = index;
                    var invalid = vars.Filter(idx => idx.Value.BitIndex > int.Min(labelPos, aux) && idx.Value.BitIndex <= int.Max(labelPos, aux));

                    foreach (var dec in invalid) {
                        logger.LogInspection(new JumpOverVarDec(pos, dec.Key, label));
                    }

                    var clamped = unchecked((sbyte) offset);

                    if (clamped != offset) {
                        logger.LogInspection(new TooLargeJump(pos, offset, label));
                    }
                    
                    var jump = new Jump(clamped, cond, pos);
                    stmtInfo.Add(new(index, jump));
                    stmt = jump;
                }
            }
            index += stmt.Size;
        }

        var arr = stmtInfo.ToArray();
        
        Array.Sort(arr);
        
        return arr;
    }

    static bool TypoPossible(string word, string candidate) {
        int length = word.Length;
        if (length < 3) return false;
        int dist = Levenshtein.GetDistance(word, candidate);

        int thresh = int.Min(3, length / 3);
        
        return dist <= thresh;
    }

    readonly record struct VariableInfo(int BitIndex, FixedSizeType Type, byte Id);
    readonly record struct StmtInfo(int BitIndex, FinalizedStmt Stmt): IComparable<StmtInfo> {
        public int CompareTo(StmtInfo other) => BitIndex.CompareTo(other.BitIndex);
    }
}