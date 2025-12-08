using System.CommandLine;
using System.Globalization;
using System.Text;
using EsmCore;

namespace EsmRuntime;

public static partial class EsmRuntime {

    static bool _debug;
    public static int Main(string[] args) {
        var inputOption = new Option<FileInfo>("--input");
        var debugOption = new Option<bool>("--debug");
        var rootCommand = new RootCommand("The Esm runtime.") { inputOption, debugOption };
        var parseResult = rootCommand.Parse(args);
        
        
        FileInfo input = parseResult.GetRequiredValue(inputOption);
        _debug = parseResult.GetValue(debugOption);
        
        Console.WriteLine($"Running program: \"{input.Name}\"");
        Console.WriteLine();

        byte[] program = File.ReadAllBytes(input.FullName);

        byte b = Run(program, 256, 256);
        
        Console.WriteLine(b == ExitCode.Success
            ? $"Program finished with exit code: 0x{b:X2}"
            : $"\e[1;91mProgram finished with exit code: 0x{b:X2}\e[0m");

        return b;
    }

    static unsafe u8 Run(ReadOnlySpan<byte> program, int memorySize, int stackSize) {
        byte* alloc = stackalloc byte[memorySize + stackSize];

        if (!CheckPtrSize(memorySize) || !CheckPtrSize(stackSize)) return ExitCode.Failure;
        var maxMem = memorySize - 1;
        var mem = new Memory(alloc, maxMem);
        var stack = new OpStack(new (alloc + memorySize, stackSize));
        try {
            var exit = Run(program, ref mem, ref stack);
            Console.WriteLine();
            return exit == null ? ExitCode.Unterminated : exit.Value == ExitCode.Success ? exit.Value : exit.Value | ExitCode.UserExit;
        } catch (RuntimeError e) {
            Console.WriteLine($"\e[1;91m{e.GetType().Name} (esmr::{(byte)e.Type:X2}): {e.Message}\e[0m");
            return (u8) (byte) e.Type | ExitCode.RuntimeError;
        }
    }

    static bool CheckPtrSize(int memorySize) => memorySize - 1 <= byte.MaxValue;


    static u8? Run(ReadOnlySpan<byte> program, ref Memory mem, ref OpStack stack) {
        for (var pc = 0; pc < program.Length; pc++) {
            var opcode = (OpCode) program[pc];
            switch (opcode) {
                case OpCode.LoadConst: 
                    stack += LoadConst(program, ref pc);
                    break;
                case OpCode.LoadStr: {
                    LoadStr(program, ref pc, ref stack);
                    break;
                }
                case OpCode.LoadMem: {
                    stack += LoadMem(program, ref pc, ref mem);
                    break;
                }
                case OpCode.Store: {
                    StoreMem(program, ref pc, ref mem, stack.Pop());
                    break;
                }
                    
                // Jump statements
                case OpCode.Jump: case OpCode.JumpIfZero: case OpCode.JumpIfNZero: {
                    Jump(program, ref pc, ref stack, opcode.JumpCondition);
                    break;
                }
                case OpCode.Exit: {
                    return Exit(stack.Pop());
                }
                    
                // Unary
                case OpCode.BwNot: {
                    stack += UnaryNot(stack.Pop());
                    break;
                }
                    
                // Binary
                case OpCode.BwAnd: {
                    stack += BinaryAnd(stack.Pop(), stack.Pop());
                    break;
                }
                case OpCode.BwOr: {
                    stack += BinaryOr(stack.Pop(), stack.Pop());
                    break;
                }
                case OpCode.BwXor: {
                    stack += BinaryXor(stack.Pop(), stack.Pop());
                    break;
                }
                case OpCode.BwLeft: {
                    stack += BinaryLeft(stack.Pop(), stack.Pop());
                    break;
                }
                case OpCode.BwRight: {
                    stack += BinaryRight(stack.Pop(), stack.Pop());
                    break;
                }
                    
                // print
                case OpCode.PrintAscii: {
                    PrintAscii(stack.Pop());
                    break;
                }
                case OpCode.PrintBin: {
                    PrintBinary(stack.Pop());
                    break;
                }
                case OpCode.PrintHex: {
                    PrintHex(stack.Pop());
                    break;
                }
                case OpCode.PrintDec: {
                    PrintDecimal(stack.Pop());
                    break;
                }
                
                case OpCode.PrintStr: {
                    PrintString(ref stack);
                    break;
                }
                    
                case OpCode.InputAscii: {
                    if (_debug) Debug("Awaiting character input...");
                    stack += (u8) Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    break;
                }
                case OpCode.InputBin: {
                    Console.WriteLine();
                    if (_debug) Debug("Awaiting binary input...");
                    string input = Console.ReadLine() ?? "";
                    if (byte.TryParse(input, NumberStyles.BinaryNumber, null, out byte result))
                        stack += result;
                    else throw new InvalidFormatError($"Invalid binary byte: {input}");

                    break;
                }
                    
                case OpCode.InputHex: {
                    Console.WriteLine();
                    if (_debug) Debug("Awaiting hexadecimal input...");
                    string input = Console.ReadLine() ?? "";
                    if (byte.TryParse(input, NumberStyles.HexNumber, null, out byte result))
                        stack += result;
                    else throw new InvalidFormatError($"Invalid hexadecimal byte: {input}");
                    
                    break;
                }
                    
                case OpCode.InputDec: {
                    Console.WriteLine();
                    if (_debug) Debug("Awaiting decimal input...");
                    string input = Console.ReadLine() ?? "";
                    if (byte.TryParse(input, NumberStyles.Integer, null, out byte result))
                        stack += result;
                    else throw new InvalidFormatError($"Invalid decimal byte: {input}");

                    break;
                }

                case OpCode.InputStr: {
                    if (_debug) Debug("Awaiting string input...");
                    string input = Console.ReadLine() ?? "";

                    stack += (u8) '\0';
                    stack += Encoding.ASCII.GetBytes(input);
                    break;
                }
                    
                default: throw new InvalidOperationException($"Unknown opcode: 0x{(byte)opcode:X2}");
            }
        }

        return null;
    }

    static void Debug(string s) {
        Console.WriteLine($"\e[0;37m{s}\e[0m");
    }
}