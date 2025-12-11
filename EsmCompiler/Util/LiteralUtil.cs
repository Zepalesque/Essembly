using System.Globalization;
using System.Numerics;
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

    
    public static List<ICompilerInspection> ParseInt(FilePos pos, string text, FixedSizeType type, out ConstantNode node) {
        bool success;
        List<ICompilerInspection> list = [];

        if (InvalidOctalLiteral().IsMatch(text)) {
            list.Add(new PossibleOctal(pos, text));
        }

        if (text.StartsWith("0o")) {
            var sub = text[2..];
            var bin = sub.Aggregate("0b", (current, c) => current + c switch {
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

            text = bin;
        }
        

        switch (type) {
            case FixedSizeType.U8: {
                success = byte.TryParse(text, null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.U16: {
                success = ushort.TryParse(text, null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.U32: {
                success = uint.TryParse(text, null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.U64: {
                success = ulong.TryParse(text, null, out var result);
                node = CreateIntLiteral(pos, result);
                /* else if (text.EndsWith("integer" = "u128")) {
            success = UInt128.TryParse(text[..^4], null, out var result);
            node = new U128Literal(result, pos);
            if (!success) list.Add(new InvalidLiteralErr(pos, text, "integer", null));
        }*/
                break;
            }
            case FixedSizeType.I8: {
                success = sbyte.TryParse(text[..^2], null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.I16: {
                success = short.TryParse(text[..^3], null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.I32: {
                success = int.TryParse(text[..^3], null, out var result);
                node = CreateIntLiteral(pos, result);
                break;
            }
            case FixedSizeType.I64: {
                success = long.TryParse(text[..^3], null, out var result);
                node = CreateIntLiteral(pos, result);
                /* else if (text.EndsWith("integer" = "i128")) {
            success = Int128.TryParse(text[..^4], null, out var result);
            node = new I128Literal(result, pos);
            if (!success) list.Add(new InvalidLiteralErr(pos, text, "integer", null));
        } */
                break;
            }
            case FixedSizeType.StrPointer:
            default: throw new InvalidOperationException();

                /*
            success = Int128.TryParse(text, null, out var result);
            if (success) {
                if (result <= long.MaxValue) node = new I64Literal((long) result, pos);
                else if (result <= int.MaxValue) node = new I32Literal((int) result, pos);
                else node = new I128Literal(result, pos);
            } else {
                node = new I32Literal(0, pos);
                if (!success) list.Add(new InvalidLiteralErr(pos, text, "integer", null));
            }*/
        }

        if (!success) list.Add(new InvalidLiteralErr(pos, text, "integer", null));

        return list;
    }

    internal static ConstantNode CreateIntLiteral<T>(FilePos pos, T result) where T: unmanaged, IBinaryInteger<T>, IConvertible {
        return result switch {
            byte val => CreateIntLiteral(pos, val, FixedSizeType.U8),
            ushort val => CreateIntLiteral(pos, val, FixedSizeType.U16),
            uint val => CreateIntLiteral(pos, val, FixedSizeType.U32),
            ulong val => CreateIntLiteral(pos, val, FixedSizeType.U64),
            sbyte val => CreateIntLiteral(pos, val, FixedSizeType.I8),
            short val => CreateIntLiteral(pos, val, FixedSizeType.I16),
            int val => CreateIntLiteral(pos, val, FixedSizeType.I32),
            long val => CreateIntLiteral(pos, val, FixedSizeType.I64),
            _ => throw new InvalidOperationException()
        };
    }
    
    internal static ConstantNode CreateIntLiteral<T>(FilePos pos, T result, FixedSizeType type) where T: unmanaged, IBinaryInteger<T>, IConvertible {
        return type switch {
            FixedSizeType.U8 => new U8Constant(result.ToByte(null), pos),
            FixedSizeType.U16 => new U16Constant(result.ToUInt16(null), pos),
            FixedSizeType.U32 => new U32Constant(result.ToUInt32(null), pos),
            FixedSizeType.U64 => new U64Constant(result.ToUInt64(null), pos),
            FixedSizeType.I8 => new I8Constant(result.ToSByte(null), pos),
            FixedSizeType.I16 => new I16Constant(result.ToInt16(null), pos),
            FixedSizeType.I32 => new I32Constant(result.ToInt32(null), pos),
            FixedSizeType.I64 => new I64Constant(result.ToInt64(null), pos),
            _ => throw new InvalidOperationException()
        };
    }
}