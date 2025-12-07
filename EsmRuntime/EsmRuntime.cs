using System.CommandLine;
using System.Globalization;
using System.Text;
using EsmCore;
using static EsmCore.OpCode;

namespace EsmRuntime;

public static class EsmRuntime {
    public static int Main(string[] args) {
        var inputOption = new Option<FileInfo>("--input");
        var debugOption = new Option<bool>("--debug");
        var rootCommand = new RootCommand("The Esm runtime.") { inputOption, debugOption };
        var parseResult = rootCommand.Parse(args);
        
        

        
        FileInfo input = parseResult.GetRequiredValue(inputOption);
        bool debug = parseResult.GetValue(debugOption);
        
        Console.WriteLine($"Running program: \"{input.Name}\"");
        Console.WriteLine();

        var bytes = File.ReadAllBytes(input.FullName);

        byte b = Run(bytes, debug);
        
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"COMPLETE! Exit code: 0x{b:X2}");

        return b;
    }


    static byte Run(ReadOnlySpan<byte> program, bool debug) {
        unsafe {
            var size = byte.MaxValue + 1;
            byte* alloc = stackalloc byte[size * 2];
            var opstack = new OpStack(alloc + size, unchecked((byte) (size - 1)));
            for (var i = 0; i < program.Length; i++) {
                var opcode = (OpCode) program[i];
                switch (opcode) {
                    case LoadConst: {
                        u8 val = program[++i];
                        
                        if (debug) Debug($"Loading constant: {val}");
                        opstack += val;
                        break;
                    }
                    case LoadStr: {
                        int start = i + 1;
                        do {
                            i++; 
                        } while (program[i] != '\0');

                        var span = program[start..(i+1)];
                        
                        for (int index = span.Length - 1; index >= 0; index--) {
                        
                            byte b = span[index];
                        
                            opstack += b;
                        
                        }

                        if (debug) Debug($"Loading string: \"{Encoding.ASCII.GetString(span)}\"");
                        break;
                    }
                    case LoadMem: {
                        byte loc = program[++i];
                        u8 val = *(alloc + loc);
                        if (debug) Debug($"Loading from memory: .{loc:X2}, value: {val}");
                        opstack += val;
                        break;
                    }
                    case Store: {
                        byte val = opstack.Pop();
                        byte loc = program[++i];
                        if (debug) Debug($"Storing to memory: .{loc:X2}, value: {val}");
                        *(alloc + loc) = val;
                        break;
                    }
                    case PopTop: {
                        u8 val = opstack.Pop();
                        if (debug) Debug($"Discarding: {val}");
                        break;
                    }
                    
                    // Jump statements
                    case Jump: {
                        int jump = unchecked((sbyte) program[i + 1]) - 1;
                        int res = i + jump;
                        if (debug) Debug($"Jumping from #{i} by {jump}, new will be #{res}");
                        
                        
                        i = res;
                        break;
                    }
                    case JumpIfZero: {
                        u8 val = opstack.Pop();
                        if (val == 0) {
                            int jump = unchecked((sbyte) program[i + 1]) - 1;
                            int res = i + jump;
                            if (debug) Debug($"Top of operand stack was zero, jumping from #{i} by {jump}, new will be {res}");
                        
                        
                            i = res;
                        } else {
                            if (debug) Debug("Top of operand stack was not zero, not jumping");
                            i++;
                        }
                        
                        break;
                    }
                    case JumpIfNZero: {
                        u8 val = opstack.Pop();
                        if (val != 0) {
                            int jump = unchecked((sbyte) program[i + 1]) - 1;
                            int res = i + jump;
                            if (debug) Debug($"Top of operand stack was not zero, jumping from #{i} by {jump}, new will be {res}");
                        
                        
                            i = res;
                        } else {
                            if (debug) Debug("Top of operand stack was zero, not jumping");
                            i++;
                        }
                        
                        break;
                    }
                    case Exit: {
                        u8 code = opstack.Pop();
                        if (debug) Debug($"Exiting with code {code}");
                        return code;
                    }
                    
                    // Unary
                    case Do1Not: {
                        u8 top = opstack.Pop();
                        u8 res = ~top;
                        if (debug) Debug($"Popping and pushing ~{top} = {res}");
                        opstack += res;
                        break;
                    }
                    
                    // Binary
                    case Do2And: {
                        u8 a = opstack.Pop();
                        u8 b = opstack.Pop();
                        u8 res = a & b;
                        if (debug) Debug($"Popping and pushing {a} & {b} = {res}");
                        
                        opstack += res;
                        break;
                    }
                    case Do2Or: {
                        u8 a = opstack.Pop();
                        u8 b = opstack.Pop();
                        u8 res = a | b;
                        if (debug) Debug($"Popping and pushing {a} | {b} = {res}");
                        
                        opstack += res;
                        break;
                    }
                    case Do2Xor: {
                        u8 a = opstack.Pop();
                        u8 b = opstack.Pop();
                        u8 res = a ^ b;
                        if (debug) Debug($"Popping and pushing {a} ^ {b} = {res}");
                        
                        opstack += res;
                        break;
                    }
                    case Do2Left: {
                        u8 a = opstack.Pop();
                        u8 b = opstack.Pop();
                        u8 res = a << b;
                        if (debug) Debug($"Popping and pushing {a} << {b} = {res}");
                        
                        opstack += res;
                        break;
                    }
                    case Do2Right: {
                        u8 a = opstack.Pop();
                        u8 b = opstack.Pop();
                        u8 res = a >> b;
                        if (debug) Debug($"Popping and pushing {a} >> {b} = {res}");
                        
                        opstack += res;
                        break;
                    }
                    
                    // print
                    case PrintAscii: {
                        char ascii = (char) opstack.Pop();
                        if (debug) Debug($"Writing ascii val to console: {ascii}");
                        Console.Write(ascii);
                        break;
                    }
                    case PrintBin: {
                        string s = opstack.Pop().Bin;
                        if (debug) Debug($"Writing binary val to console: {s}");
                        Console.Write(s); break;
                    }
                    case PrintHex: {
                        string s = opstack.Pop().Hex;
                        if (debug) Debug($"Writing hexadecimal val to console: {s}");
                        Console.Write(s); break;
                    }
                    case PrintDec: {
                        string s = opstack.Pop().Dec;
                        if (debug) Debug($"Writing decimal val to console: {s}");
                        Console.Write(s); break;
                    }
                    
                    case InputAscii: {
                        if (debug) Debug("Awaiting character input...");
                        opstack += (u8) Console.ReadKey().KeyChar;
                        break;
                    }
                    case InputBin: {
                        if (debug) Debug("Awaiting binary input...");
                        string input = Console.ReadLine() ?? "";
                        if (byte.TryParse(input, NumberStyles.BinaryNumber, null, out byte result))
                            opstack += result;

                        break;
                    }
                    
                    case InputHex: {
                        if (debug) Debug("Awaiting hexadecimal input...");
                        string input = Console.ReadLine() ?? "";
                        if (byte.TryParse(input, NumberStyles.HexNumber, null, out byte result))
                            opstack += result;

                        break;
                    }
                    
                    case InputDec:
                    
                    case InputInt: {
                        if (debug) Debug("Awaiting decimal input...");
                        string input = Console.ReadLine() ?? "";
                        if (byte.TryParse(input, NumberStyles.Integer, null, out byte result))
                            opstack += result;
                        break;
                    }

                    case InputStr: {
                        if (debug) Debug("Awaiting string input...");
                        string input = Console.ReadLine() ?? "";

                        opstack += (u8) '\0';
                        foreach (char c in input.Reverse()) {
                            opstack += (u8) c;
                        }

                        break;
                    }
                    
                    default: throw new InvalidOperationException($"Unknown opcode: 0x{(byte)opcode:X2}");
                }
            }
        }
        return 0;
    }

    static void Debug(string s) {
        Console.WriteLine($"\e[0;37m{s}\e[0m");
    }
}