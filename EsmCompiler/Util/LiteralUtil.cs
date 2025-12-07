using System.Globalization;
using System.Text.RegularExpressions;

namespace EsmCompiler.Util;

public static partial class LiteralUtil {
    
    [GeneratedRegex("0[_0-7]*?[0-7][iu](?:8|(?:16)|(?:32)|(?:64)|(?:128))")]
    private static partial Regex InvalidOctalLiteral();  
    
    [GeneratedRegex(@"\\(?:o(?<octal>[0-7]{3}))|(?:x(?<ascii>[0-9a-fA-f]{2}))|(?:[ux](?<single>[0-9a-fA-f]{4}))|(?:(?<double>U[0-9a-fA-f]{6}))")]
    private static partial Regex CharRegex();
    
    [GeneratedRegex(@"\\~[\\]")]
    private static partial Regex EscapeCatch();

    public static InvalidEscapeSeqError[] RegularEscape(FilePos tokenPos, string input, out string result) {
        var unicode = CharRegex();

        input = input[1..^1];

        var list = new List<InvalidEscapeSeqError>();
        
        FilePos slice = tokenPos[1..^2];

        string unicodeEsc = unicode.Replace(input, match => {
            if (match.Groups["octal"].Success && ushort.TryParse(OctDigitsToBinary(match.Groups["single"].ValueSpan),
                    NumberStyles.BinaryNumber, null, out var uso)) {
                return char.ToString((char) uso);
            }

            if (match.Groups["ascii"].Success && byte.TryParse(match.Groups["ascii"].ValueSpan,
                    NumberStyles.HexNumber, null, out var b)) {
                return char.ToString((char) b);
            }

            if (match.Groups["single"].Success && ushort.TryParse(match.Groups["single"].ValueSpan,
                    NumberStyles.HexNumber, null, out var us)) {
                return char.ToString((char) us);
            }

            if (match.Groups["double"].Success && uint.TryParse(match.Groups["double"].ValueSpan,
                    NumberStyles.HexNumber, null, out var ui)) {
                return char.ConvertFromUtf32(unchecked((int) ui));
            }

            return match.Value;
        });
        
        var others = unicodeEsc
            .Replace("\\0",  "\0")
            .Replace("\\a",  "\a")
            .Replace("\\b",  "\b")
            .Replace("\\e",  "\e")
            .Replace("\\f",  "\f")
            .Replace("\\n",  "\n")
            .Replace("\\r",  "\r")
            .Replace("\\t",  "\t")
            .Replace("\\v",  "\v")
            .Replace("\\'",  "\'")
            .Replace("\\\"", "\"")
        ;

        Regex inv = EscapeCatch();

        foreach (ValueMatch match in inv.EnumerateMatches(others)) {
            var index = match.Index;
            var length = match.Index;
                    
            FilePos invPos = slice[index..(index + length)];
            var invStr = input[index..(index + length)];


            list.Add(new(invPos, invStr, null));
        }

        result = others.Replace(@"\\", "\\");
        return list.ToArray();
    }

    static ReadOnlySpan<char> OctDigitsToBinary(ReadOnlySpan<char> input) {
        return new string(input).Aggregate("0b", (current, c) => current + c switch {
            '0' => "000",
            '1' => "001",
            '2' => "010",
            '3' => "011",
            '4' => "100",
            '5' => "101",
            '6' => "110",
            '7' => "111",
            _ => c
        });
    }
    
    public static InvalidEscapeSeqError? RegularEscapeChar(FilePos tokenPos, string input, out char result) {
        var unicode = CharRegex();

        input = input[1..^1];

        
        FilePos slice = tokenPos[1..^1];

        string unicodeEsc = unicode.Replace(input, match => {
            if (match.Groups["octal"].Success && ushort.TryParse(OctDigitsToBinary(match.Groups["single"].ValueSpan),
                    NumberStyles.BinaryNumber, null, out var uso)) {
                return char.ToString((char) uso);
            }

            if (match.Groups["ascii"].Success && byte.TryParse(match.Groups["ascii"].ValueSpan,
                    NumberStyles.HexNumber, null, out var b)) {
                return char.ToString((char) b);
            }

            if (match.Groups["single"].Success && ushort.TryParse(match.Groups["single"].ValueSpan,
                    NumberStyles.HexNumber, null, out var us)) {
                return char.ToString((char) us);
            }

            if (match.Groups["double"].Success && uint.TryParse(match.Groups["double"].ValueSpan,
                    NumberStyles.HexNumber, null, out var ui)) {
                return char.ConvertFromUtf32(unchecked((int) ui));
            }

            return match.Value;
        });
        
        var others = unicodeEsc
            .Replace("\\0",  "\0")
            .Replace("\\a",  "\a")
            .Replace("\\b",  "\b")
            .Replace("\\e",  "\e")
            .Replace("\\f",  "\f")
            .Replace("\\n",  "\n")
            .Replace("\\r",  "\r")
            .Replace("\\t",  "\t")
            .Replace("\\v",  "\v")
            .Replace("\\'",  "\'")
            .Replace("\\\"", "\"")
        ;

        InvalidEscapeSeqError? err = null;
        Regex inv = EscapeCatch();

        // Hopefully there will be no more than one?
        foreach (var match in inv.EnumerateMatches(others)) {
            int index = match.Index;
            int length = match.Index;
                    
            FilePos invPos = slice[(index)..(index + length)];
            string invStr = input[(index)..(index + length)];
            
            err = new(invPos, invStr, null);
        }

        result = others.Replace(@"\\", "\\")[0];
        return err;
    }

}