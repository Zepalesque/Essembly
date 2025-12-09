namespace EsmRuntime.Debug;

public static class StringTransformation {

    public static string Escape(string s) 
        => s.Replace("\\0", "\0")
            .Replace("\a", "\\a")
            .Replace("\b", "\\b")
            .Replace("\e", "\\e")
            .Replace("\f", "\\f")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\v", "\\v")
            .Replace("\'", "\\'")
            .Replace("\"", "\\\"")
            .Replace("\\", @"\\");
}