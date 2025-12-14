using System.CommandLine;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using EsmRuntime.Common;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using static EsmRuntime.Constants;
using HeapTree = EsmRuntime.Memory.Heap.HeapTree;

namespace EsmRuntime;

// ReSharper disable once InconsistentNaming
public static partial class EsmVM {
    public static readonly usize NullAddr = 0;
    public static readonly usize UnitAddr = 1;

    public static unsafe void* MemStart { get; set; }
    public static unsafe nuint MemAddrUSize => (nuint) MemStart;
    public static unsafe nint MemAddrISize => (nint) MemStart;

    internal static unsafe HeapTree* HeapMemoryTree;
    

    public static unsafe int Main(string[] args) {
        var inputOption = new Option<FileInfo>("--input");
        var rootCommand = new RootCommand("The Esm runtime.") { inputOption };
        ParseResult parseResult = rootCommand.Parse(args);
        
        
        FileInfo input = parseResult.GetRequiredValue(inputOption);
        
        Console.WriteLine($"Running program: \"{input.Name}\"");
        Console.WriteLine();

        byte[] program = File.ReadAllBytes(input.FullName);

        fixed (byte* ptr = &program[0]) {
            u8 b = Run(new(ptr, (nuint) program.Length), new(256, 256, 256, 256));

            Console.WriteLine(b == ExitCode.Success
                ? $"Program finished with exit code: 0x{b.Hex}!"
                : $"\e[1;91mProgram finished with exit code: 0x{b.Hex}!\e[0m");

            return b;
        }
    }
    
    readonly ref struct MemorySizes(nuint heap, nuint functionStack, nuint globalStack, nuint opStack) {
        public readonly nuint Heap = heap;
        public readonly nuint FunctionStack = functionStack;
        public readonly nuint GlobalStack = globalStack;
        public readonly nuint OpStack = opStack;

        [MethodImpl(Inline)]
        public nuint FullSize() => Heap + FunctionStack + GlobalStack + OpStack + 2;
    }

    static unsafe u8 Run(FatPtr program, MemorySizes sizes) {
        nuint size = sizes.FullSize();
        var alloc = (byte*) NativeMemory.AlignedAlloc(size, 16);
        try {
            NativeMemory.Clear(alloc, size);
            var node = HeapTree.Create(2, sizes.Heap + 2, false);

            HeapMemoryTree = node;
            MemStart = alloc;
        
            var mem = new ReferenceHeap(alloc, (nint) sizes.Heap + 2);
            var stack = new OpStack(alloc + sizes.Heap + 2, (nint) sizes.OpStack);
            try {
                var exit = Run(program, in mem, ref stack);
                Console.WriteLine();
                return exit == null ? ExitCode.Unterminated : exit.Value == ExitCode.Success ? exit.Value : exit.Value | ExitCode.UserExit;
            } catch (RuntimeError e) {
                Console.WriteLine($"\e[1;91m{e.GetType().Name} (esmr::{(byte)e.Type:X2}): {e.Message}\e[0m");
                return (u8) (byte) e.Type | ExitCode.RuntimeError;
            }
        } catch {
            NativeMemory.Free(alloc);
            return ExitCode.Failure;
        }
    }

    static unsafe u8? Run(FatPtr program, in ReferenceHeap heap, ref OpStack stack) {
        for (var pc = 0; pc < program.Size; pc++) {
            var opcode = (OpCode) program[pc];
            switch (opcode) {
                case OpCode.Push8: 
                    stack.Push(LoadConst<u8>(program, &pc));
                    break;
                case OpCode.Push16: 
                    stack.Push(LoadConst<u16>(program, &pc));
                    break;
                case OpCode.Push32: 
                    stack.Push(LoadConst<u32>(program, &pc));
                    break;
                case OpCode.Push64: 
                    stack.Push(LoadConst<u64>(program, &pc));
                    break;
                
                
                
                case OpCode.AllocStr: {
                    stack.Push(AllocRef<StringSlice>(program, &pc, heap));
                    break;
                }

                case OpCode.FreeHeap: {
                    heap.Free(stack.Pop<Reference<Unit>>());
                    break;
                }
                
                case OpCode.Deref: {
                    stack.Push(stack.Pop<Reference<Slice<u8>>>().Dereference());
                    break;
                }
                case OpCode.PushGlobalAddr: {
                    // stack.Push(PushHeapRef(program, &pc, heap));
                    break;
                }
                case OpCode.StoreGlobal8: {
                    // StoreMem(ref stack, ref heap, stack.Pop<u8>());
                    break;
                }
                case OpCode.StoreGlobal16: {
                    // StoreMem(program, ref pc, ref heap, stack.Pop<u16>());
                    break;
                }
                    
                // Jump statements
                case OpCode.Jump: case OpCode.JumpIfFalse: case OpCode.JumpIfTrue: {
                    Jump(program, ref pc, ref stack, opcode.JumpCondition);
                    break;
                }
                case OpCode.Exit: {
                    return Exit(stack.Pop<u8>());
                }
                    
                // Unary
                case OpCode.Not8: {
                    stack.Push(UnaryNot(stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Not16: {
                    stack.Push(UnaryNot(stack.Pop<u16>()));
                    break;
                }
                
                case OpCode.Not32: {
                    stack.Push(UnaryNot(stack.Pop<u32>()));
                    break;
                }
                
                case OpCode.Not64: {
                    stack.Push(UnaryNot(stack.Pop<u64>()));
                    break;
                }
                    
                // Binary
                case OpCode.And8: {
                    stack.Push(BinaryAnd(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.And16: {
                    stack.Push(BinaryAnd(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                
                case OpCode.And32: {
                    stack.Push(BinaryAnd(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }
                case OpCode.And64: {
                    stack.Push(BinaryAnd(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }
                
                case OpCode.Or8: {
                    stack.Push(BinaryOr(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Or16: {
                    stack.Push(BinaryOr(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }

                case OpCode.Or32: {
                    stack.Push(BinaryOr(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }

                case OpCode.Or64: {
                    stack.Push(BinaryOr(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }

                case OpCode.Xor8: {
                    stack.Push(BinaryXor(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Xor16: {
                    stack.Push(BinaryXor(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                
                case OpCode.Xor32: {
                    stack.Push(BinaryXor(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }            
                
                case OpCode.Xor64: {
                    stack.Push(BinaryXor(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }
                
                case OpCode.Left8: {
                    stack.Push(BinaryLeft(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Left16: {
                    stack.Push(BinaryLeft(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                                
                case OpCode.Left32: {
                    stack.Push(BinaryLeft(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }
                                                
                case OpCode.Left64: {
                    stack.Push(BinaryLeft(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }

                case OpCode.RightI8: {
                    stack.Push(BinaryRight(stack.Pop<i8>(), stack.Pop<i8>()));
                    break;
                }
                
                case OpCode.RightI16: {
                    stack.Push(BinaryRight(stack.Pop<i16>(), stack.Pop<i16>()));
                    break;
                }
                
                case OpCode.RightI32: {
                    stack.Push(BinaryRight(stack.Pop<i32>(), stack.Pop<i32>()));
                    break;
                }
                
                case OpCode.RightI64: {
                    stack.Push(BinaryRight(stack.Pop<i64>(), stack.Pop<i64>()));
                    break;
                }
                
                case OpCode.RightU8: {
                    stack.Push(BinaryURight(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.RightU16: {
                    stack.Push(BinaryURight(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                
                case OpCode.RightU32: {
                    stack.Push(BinaryURight(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }
                
                case OpCode.RightU64: {
                    stack.Push(BinaryURight(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }
                
                
                case OpCode.Plus8: {
                    stack.Push(BinaryPlus(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Plus16: {
                    stack.Push(BinaryPlus(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                                
                case OpCode.Plus32: {
                    stack.Push(BinaryPlus(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }
                                                
                case OpCode.Plus64: {
                    stack.Push(BinaryPlus(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }
                
                case OpCode.Minus8: {
                    stack.Push(BinaryMinus(stack.Pop<u8>(), stack.Pop<u8>()));
                    break;
                }
                
                case OpCode.Minus16: {
                    stack.Push(BinaryMinus(stack.Pop<u16>(), stack.Pop<u16>()));
                    break;
                }
                                
                case OpCode.Minus32: {
                    stack.Push(BinaryMinus(stack.Pop<u32>(), stack.Pop<u32>()));
                    break;
                }
                                                
                case OpCode.Minus64: {
                    stack.Push(BinaryMinus(stack.Pop<u64>(), stack.Pop<u64>()));
                    break;
                }
                
                // print
                case OpCode.PrintAscii: {
                    PrintAscii(stack.Pop<u8>());
                    break;
                }
                case OpCode.PrintUtf16: {
                    PrintUtf16(stack.Pop<u16>());
                    break;
                }
                case OpCode.PrintU8: {
                    PrintInteger(stack.Pop<u8>());
                    break;
                }
                case OpCode.PrintU16: {
                    PrintInteger(stack.Pop<u16>());
                    break;
                }
                
                case OpCode.PrintStr: {
                    PrintString(ref stack);
                    break;
                }
                    
                case OpCode.InputAscii: {
                    #if DEBUG
                    Debug("Awaiting character input...");
                    #endif
                    stack.Push((u8) Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    break;
                }

                    
                case OpCode.InputU8: {
                    Console.WriteLine();
                    #if DEBUG
                    Debug("Awaiting u8 decimal input...");
                    #endif
                    string input = Console.ReadLine() ?? "";
                    if (byte.TryParse(input, NumberStyles.Integer, null, out var result))
                        stack.Push<u8>(result);
                    else throw new InvalidFormatError($"Invalid decimal u8: {input}");

                    break;
                }
                
                case OpCode.InputU16: {
                    Console.WriteLine();
                    #if DEBUG
                    Debug("Awaiting u16 decimal input...");
                    #endif
                    string input = Console.ReadLine() ?? "";
                    if (ushort.TryParse(input, NumberStyles.Integer, null, out var result))
                        stack.Push<u16>(result);
                    else throw new InvalidFormatError($"Invalid decimal u16: {input}");

                    break;
                }
             
                case OpCode.AllocInputStr: {
                    #if DEBUG
                    Debug("Awaiting string input...");
                    #endif
                    string input = Console.ReadLine() ?? "";
                    var bytes = Encoding.UTF8.GetBytes(input);
                    fixed (byte* ptr = &bytes[0]) {
                        StringSlice slice = new(new(ptr, bytes.Length));
                        stack.Push(heap.Allocate(slice));
                    }
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