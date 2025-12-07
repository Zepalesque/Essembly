using System.CommandLine;
using Antlr4.Runtime;
using EsmCompiler.Util;

namespace EsmCompiler;

public static partial class EsmCompiler {
    public static void Main(string[] args) {
        var inputOption = new Option<FileInfo>("--input");
        var outputOption = new Option<FileInfo>("--output");
        var rootCommand = new RootCommand("The Esm compiler.") { inputOption, outputOption };
        var parseResult = rootCommand.Parse(args);

        FileInfo input = parseResult.GetRequiredValue(inputOption);
        FileInfo output = parseResult.GetRequiredValue(outputOption);

        var text = File.ReadAllText(input.FullName);
        
        EsmLexer lexer = new(new AntlrInputStream(text));
        EsmParser parser = new(new CommonTokenStream(lexer));

        CompilationLogger logger = new CompilationLogger();
        FileData file = new(Path.GetRelativePath(input.FullName, input.FullName), new(text));
        EsmAstBuilder builder = new(logger, file);

        Span<SizedStmt> stmts = builder.Build(parser.@base());
        
        var transformed = Analyze(stmts, logger);

        var bytes = Compile(transformed);

        bool success = logger.Errors == 0;
        
        Console.WriteLine("Compilation process finished.");
        Console.WriteLine($"Success: {success}");
        Console.WriteLine($"Errors: {logger.Errors}");
        Console.WriteLine($"Warnings: {logger.Warnings}");
        Console.WriteLine($"Debugs: {logger.Debugs}");
        Console.WriteLine($"Notices: {logger.Notices}");

        if (!success) return;
        if (!output.Directory?.Exists ?? false) output.Directory.Create();
        File.WriteAllBytes(output.FullName, bytes);
        // using var sw = new BinaryWriter(File.Open(output.FullName, FileMode.Create), Encoding.ASCII);
        // sw.Write(bytes);
    }

    static ReadOnlySpan<byte> Compile(ReadOnlySpan<StmtInfo> transformed) {
        List<byte> bytes = new();

        foreach (var stmtInfo in transformed) {
            bytes.AddRange(stmtInfo.Stmt.OpCode);
        }
        
        return bytes.ToArray();
    }
}