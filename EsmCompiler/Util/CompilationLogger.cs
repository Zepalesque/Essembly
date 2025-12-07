using System.Text;
using System.Text.RegularExpressions;

namespace EsmCompiler.Util;

public partial class CompilationLogger {
    internal uint Errors, Warnings, Notices, Debugs;
    

    public void LogInspection<T>(T inspection) where T: ICompilerInspection {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (inspection.Id.Type == InspecId.Error) Errors++;
        else if (inspection.Id.Type == InspecId.Warning) Warnings++;
        else if (inspection.Id.Type == InspecId.Notice) Notices++;
        else if (inspection.Id.Type == InspecId.Debug) Debugs++;
        
        FilePos pos = inspection.Pos;
        
        FileSpan span = inspection.Pos.File.Memory.AsSpan();

        var esc = inspection.Id.AnsiCode ?? "";
        var message = inspection.Message;
        StringBuilder outer = new($"{esc}{inspection.Id.Stage} {inspection.Id.Type} [esmc::0x{inspection.Id:X}]: {message}\e[0m\n");
        var maxLineNumSize = ((pos.End.Line < span.LineCount - 1 ? pos.End.Line + 1 : pos.End.Line) + 1).ToString().Length;
        string numHolder = new('━', maxLineNumSize + 2);
        var line1 = $"┍{numHolder}┯━ Loc: {pos} ━━"; 

        Regex ansiPattern = MyRegex();

        StringBuilder builder = new();
        var maxLength = ansiPattern.Replace(line1, "").Length;

        if (pos.Start.Line > 0) {
            var pre = span[pos.Start.Line - 1];
            var preString = $"│ {pos.Start.Line.ToString().PadLeft(maxLineNumSize)} │ \e[37m{pre}\e[0m\n";
            var prel = ansiPattern.Replace(preString, "").Length - 1;
            maxLength = int.Max(maxLength, prel);
            builder.Append(preString);
        }
        
        if (pos.Start.Line != pos.End.Line) {
            var startLine = span[pos.Start.Line];
            var cs = pos.Start.Column;
            var preInspec = new string(startLine[..cs]);
            var inspecStart = new string(startLine[cs..]);
            var startString = $"│ {esc}{(pos.Start.Line + 1).ToString().PadLeft(maxLineNumSize)}\e[0m │ \e[37m{preInspec}{esc}{inspecStart}\e[0m\n";
            var startl = ansiPattern.Replace(startString, "").Length - 1;
            maxLength = int.Max(maxLength, startl);
            builder.Append(startString);
            
            for (var i = pos.Start.Line + 1; i < pos.End.Line; i++) {
                var midString = $"│ {esc}{(i + 1).ToString().PadLeft(maxLineNumSize)}\e[0m │ {esc}{new(span[i])}\e[0m\n";
                var midl = ansiPattern.Replace(midString, "").Length - 1;
                builder.Append(midString);
                maxLength = int.Max(maxLength, midl);
            }
            
            var endLine = span[pos.End.Line];
            var ce = pos.End.Column;
            string inspecEnd = ce >= endLine.Length - 1 ? new(endLine) : new(endLine[..(ce + 1)]);
            var postInspec = ce >= endLine.Length - 1 ? "" : new(endLine[(ce + 1)..]);
            
            var endString = $"│ {esc}{(pos.End.Line + 1).ToString().PadLeft(maxLineNumSize)}\e[0m │ {esc}{inspecEnd}\e[37m{postInspec}\e[0m\n";
            var endl = ansiPattern.Replace(endString, "").Length - 1;
            builder.Append(endString);
            maxLength = int.Max(maxLength, endl);
        } else {
            var line = span[pos.Start.Line];
            var cs = pos.Start.Column;
            var ce = pos.End.Column;
            string preInspec = new(line[..cs]);
            string inspec = ce >= line.Length - 1 ? new(line[cs..]) : new(line[cs..(ce + 1)]);
            var postInspec = ce >= line.Length - 1 ? "" : new(line[(ce + 1)..]);
            var s = $"│ {esc}{(pos.Start.Line + 1).ToString().PadLeft(maxLineNumSize)}\e[0m │ \e[37m{preInspec}{esc}{inspec}\e[37m{postInspec}\e[0m\n";
            var l = ansiPattern.Replace(s, "").Length - 1;
            maxLength = int.Max(maxLength, l);
            builder.Append(s);
            
        }

        if (pos.End.Line < span.LineCount - 1) {
            var post = span[pos.End.Line + 1];
            var postString = $"│ {(pos.End.Line + 2).ToString().PadLeft(maxLineNumSize)} │ \e[37m{post}\e[0m\n";
            var postl = ansiPattern.Replace(postString, "").Length - 1;
            maxLength = int.Max(maxLength, postl);
            builder.Append(postString);
        }

        var footer = $"┕{numHolder}┷".PadRight(maxLength, '━');
        
        builder.Append(footer);
        builder.Append('\n');
        
        outer.Append(line1.PadRight(maxLength, '━'));
        outer.Append('\n');
        outer.Append(builder);
        
        var hint = inspection.Hint;
        if (hint != null) outer.Append($"{esc}HINT:\e[0m {hint}");
        
        outer.Append('\n');
        Encoding initialEncoding = Console.OutputEncoding;
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine(outer.ToString());
        Console.OutputEncoding = initialEncoding;
    }

    [GeneratedRegex(@"\[([0-9]*?)m")]
    private static partial Regex MyRegex();
}