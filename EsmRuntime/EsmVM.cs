using System.CommandLine;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using EsmRuntime.Memory.Util;
using HeapTree = EsmRuntime.Memory.Heap.HeapTree;

namespace EsmRuntime;

// ReSharper disable once InconsistentNaming
public static partial class EsmVM {
    public static readonly isize NullAddr = 0;
    public static readonly isize UnitAddr = 1;

    public static unsafe void* MemStart { get; set; }
    public static unsafe nuint MemAddrUSize => (nuint) MemStart;
    public static unsafe nint MemAddrISize => (nint) MemStart;

    public static unsafe int Main(string[] args) {
        var inputOption = new Option<FileInfo>("--input");
        var rootCommand = new RootCommand("The Esm runtime.") { inputOption };
        ParseResult parseResult = rootCommand.Parse(args);
        
        
        FileInfo input = parseResult.GetRequiredValue(inputOption);
        
        // TODO: Binary archive
        if (input.Extension == ".ebn") {
            Console.WriteLine($"Running program: \"{input.Name}\"");
            Console.WriteLine();
            
            byte[] program = File.ReadAllBytes(input.FullName);
            
            
            fixed (byte* ptr = &program[0]) {
                u8 b =  Run(new(ptr, (nuint) program.Length), new(256, 256, 256, 256));
                
                Console.WriteLine(b == ExitCode.Success
                    ? $"Program finished with exit code: 0x{b.Hex}!"
                    : $"\e[1;91mProgram finished with exit code: 0x{b.Hex}!\e[0m");
                
                return b;
            }
        }
        
        return -1;
    }
    
    readonly ref struct MemorySizes(nuint heap, nuint functionStack, nuint globalStack, nuint opStack) {
        public readonly nuint Heap = heap;
        public readonly nuint FunctionStack = functionStack;
        public readonly nuint GlobalStack = globalStack;
        public readonly nuint OpStack = opStack;

        [MethodImpl(Utils.Inline)]
        public nuint FullSize() => Heap + FunctionStack + GlobalStack + OpStack + 2;
    }

    static unsafe u8 Run(FatPtr program, MemorySizes sizes) {
        nuint size = sizes.FullSize();
        var alloc = (byte*) NativeMemory.AlignedAlloc(size, 16);
        try {
            NativeMemory.Clear(alloc, size);
            HeapTree* node = HeapTree.Create(2, (usize) sizes.Heap + 2, false);

            MemStart = alloc;
        
            var heap = new Heap(alloc, (nint) sizes.Heap + 2, &node);
            var stack = new OpStack(alloc + sizes.Heap + 2, (nint) sizes.OpStack);
            try {
                u8? exit = Run(program, &heap, &stack);
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

    static unsafe u8? Run(FatPtr program, Heap* heap, OpStack* stack) {
        nuint pc = 0;
        RuntimeContext context = new(program, heap, stack, &pc);
        
        while (pc < program.Size) {
            OpCode opcode = *(OpCode*)(program.Ptr + pc);
            switch (opcode) {
                case OpCode.AllocStr: {
                    stack->Push(AllocPtr<StringSlice>(&context));
                    break;
                }
                
                case OpCode.AllocPtr: {
                    stack->Push(AllocPtr(&context));
                    break;
                }
                
                case OpCode.FreeHeap: {
                    heap->Free(stack->Pop<Ptr<Unit>>());
                    break;
                }
                
                case OpCode.PushX8: 
                    stack->Push(LoadConst<u8>(&context));
                    break;
                case OpCode.PushX16: 
                    stack->Push(LoadConst<u16>(&context));
                    break;
                case OpCode.PushX32: 
                    stack->Push(LoadConst<u32>(&context));
                    break;
                case OpCode.PushX64:
                    stack->Push(LoadConst<u64>(&context));
                    break;
                case OpCode.PushXsize:
                    stack->Push(LoadConst<usize>(&context));
                    break;
                
                // TODO: Properly implement global frames
                case OpCode.PushGlobalRef: {
                    stack->Push(stack->Pop<Ptr<Unit>>().DerefSize);
                    break;
                }


                
                case OpCode.Deref: {
                    stack->Push(stack->Pop<Ptr<Slice<u8>>>().Dereference());
                    break;
                }

                case OpCode.StoreGlobalX8: {
                    // StoreMem(ref stack, ref heap, stack->Pop<u8>());
                    break;
                }
                case OpCode.StoreGlobalX16: {
                    // StoreMem(program, ref pc, ref heap, stack->Pop<u16>());
                    break;
                }
                    
                // Jump statements
                case OpCode.Jump: case OpCode.JumpIfFalse: case OpCode.JumpIfTrue: {
                    Jump(program, ref pc, ref *stack, opcode.JumpCondition);
                    break;
                }
                case OpCode.Exit: {
                    return Exit(stack->Pop<u8>());
                }
                    
                // Unary
                case OpCode.NotX8: {
                    stack->Push(UnaryNot(stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.NotX16: {
                    stack->Push(UnaryNot(stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.NotX32: {
                    stack->Push(UnaryNot(stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.NotX64: {
                    stack->Push(UnaryNot(stack->Pop<u64>()));
                    break;
                }
                    
                // Binary
                case OpCode.AndX8: {
                    stack->Push(BinaryAnd(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.AndX16: {
                    stack->Push(BinaryAnd(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.AndX32: {
                    stack->Push(BinaryAnd(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                case OpCode.AndX64: {
                    stack->Push(BinaryAnd(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.AndXsize: {
                    stack->Push(BinaryAnd(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.OrX8: {
                    stack->Push(BinaryOr(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.OrX16: {
                    stack->Push(BinaryOr(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }

                case OpCode.OrX32: {
                    stack->Push(BinaryOr(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }

                case OpCode.OrX64: {
                    stack->Push(BinaryOr(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.OrXsize: {
                    stack->Push(BinaryOr(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }

                case OpCode.XorX8: {
                    stack->Push(BinaryXor(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.XorX16: {
                    stack->Push(BinaryXor(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.XorX32: {
                    stack->Push(BinaryXor(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }            
                
                case OpCode.XorX64: {
                    stack->Push(BinaryXor(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.XorXsize: {
                    stack->Push(BinaryXor(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.LeftX8: {
                    stack->Push(BinaryLeft(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.LeftX16: {
                    stack->Push(BinaryLeft(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                                
                case OpCode.LeftX32: {
                    stack->Push(BinaryLeft(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                                                
                case OpCode.LeftX64: {
                    stack->Push(BinaryLeft(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.LeftXsize: {
                    stack->Push(BinaryLeft(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }

                case OpCode.RightI8: {
                    stack->Push(BinaryRight(stack->Pop<i8>(), stack->Pop<i8>()));
                    break;
                }
                
                case OpCode.RightI16: {
                    stack->Push(BinaryRight(stack->Pop<i16>(), stack->Pop<i16>()));
                    break;
                }
                
                case OpCode.RightI32: {
                    stack->Push(BinaryRight(stack->Pop<i32>(), stack->Pop<i32>()));
                    break;
                }
                
                case OpCode.RightI64: {
                    stack->Push(BinaryRight(stack->Pop<i64>(), stack->Pop<i64>()));
                    break;
                }
                
                case OpCode.RightU8: {
                    stack->Push(BinaryURight(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.RightU16: {
                    stack->Push(BinaryURight(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.RightU32: {
                    stack->Push(BinaryURight(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.RightU64: {
                    stack->Push(BinaryURight(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                
                case OpCode.PlusX8: {
                    stack->Push(BinaryPlus(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.PlusX16: {
                    stack->Push(BinaryPlus(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                                
                case OpCode.PlusX32: {
                    stack->Push(BinaryPlus(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                                                
                case OpCode.PlusX64: {
                    stack->Push(BinaryPlus(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.PlusPtr: {
                    stack->Push(BinaryPlus(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.MinusX8: {
                    stack->Push(BinaryMinus(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.MinusX16: {
                    stack->Push(BinaryMinus(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.MinusX32: {
                    stack->Push(BinaryMinus(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.MinusX64: {
                    stack->Push(BinaryMinus(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.MinusXsize: {
                    stack->Push(BinaryMinus(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.MultX8: {
                    stack->Push(BinaryMult(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.MultX16: {
                    stack->Push(BinaryMult(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.MultX32: {
                    stack->Push(BinaryMult(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.MultX64: {
                    stack->Push(BinaryMult(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.MultXsize: {
                    stack->Push(BinaryMult(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.DivI8: {
                    stack->Push(BinaryDiv(stack->Pop<i8>(), stack->Pop<i8>()));
                    break;
                }
                
                case OpCode.DivI16: {
                    stack->Push(BinaryDiv(stack->Pop<i16>(), stack->Pop<i16>()));
                    break;
                }
                
                case OpCode.DivI32: {
                    stack->Push(BinaryDiv(stack->Pop<i32>(), stack->Pop<i32>()));
                    break;
                }
                
                case OpCode.DivI64: {
                    stack->Push(BinaryDiv(stack->Pop<i64>(), stack->Pop<i64>()));
                    break;
                }
                
                case OpCode.DivIsize: {
                    stack->Push(BinaryDiv(stack->Pop<isize>(), stack->Pop<isize>()));
                    break;
                }
                
                case OpCode.DivU8: {
                    stack->Push(BinaryDiv(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.DivU16: {
                    stack->Push(BinaryDiv(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.DivU32: {
                    stack->Push(BinaryDiv(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.DivU64: {
                    stack->Push(BinaryDiv(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.DivUsize: {
                    stack->Push(BinaryDiv(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                case OpCode.ModI8: {
                    stack->Push(BinaryMod(stack->Pop<i8>(), stack->Pop<i8>()));
                    break;
                }
                
                case OpCode.ModI16: {
                    stack->Push(BinaryMod(stack->Pop<i16>(), stack->Pop<i16>()));
                    break;
                }
                
                case OpCode.ModI32: {
                    stack->Push(BinaryMod(stack->Pop<i32>(), stack->Pop<i32>()));
                    break;
                }
                
                case OpCode.ModI64: {
                    stack->Push(BinaryMod(stack->Pop<i64>(), stack->Pop<i64>()));
                    break;
                }
                
                case OpCode.ModIsize: {
                    stack->Push(BinaryMod(stack->Pop<isize>(), stack->Pop<isize>()));
                    break;
                }
                
                case OpCode.ModU8: {
                    stack->Push(BinaryMod(stack->Pop<u8>(), stack->Pop<u8>()));
                    break;
                }
                
                case OpCode.ModU16: {
                    stack->Push(BinaryMod(stack->Pop<u16>(), stack->Pop<u16>()));
                    break;
                }
                
                case OpCode.ModU32: {
                    stack->Push(BinaryMod(stack->Pop<u32>(), stack->Pop<u32>()));
                    break;
                }
                
                case OpCode.ModU64: {
                    stack->Push(BinaryMod(stack->Pop<u64>(), stack->Pop<u64>()));
                    break;
                }
                
                case OpCode.ModUsize: {
                    stack->Push(BinaryMod(stack->Pop<usize>(), stack->Pop<usize>()));
                    break;
                }
                
                // print
                case OpCode.PrintUtf8: {
                    PrintAscii(stack->Pop<u8>());
                    break;
                }
                case OpCode.PrintUtf16: {
                    PrintUtf16(stack->Pop<u16>());
                    break;
                }
                case OpCode.PrintU8: {
                    PrintInteger(stack->Pop<u8>());
                    break;
                }
                case OpCode.PrintU16: {
                    PrintInteger(stack->Pop<u16>());
                    break;
                }
                
                case OpCode.PrintStr: {
                    PrintString(ref *stack);
                    break;
                }
                    
                case OpCode.InputUtf8: {
                    #if ESM_DEBUG
                    Debug("Awaiting character input...");
                    #endif
                    stack->Push((u8) Console.ReadKey().KeyChar);
                    Console.WriteLine();
                    break;
                }

                    
                case OpCode.InputU8: {
                    Console.WriteLine();
                    #if ESM_DEBUG
                    Debug("Awaiting u8 decimal input...");
                    #endif
                    string input = Console.ReadLine() ?? "";
                    if (byte.TryParse(input, NumberStyles.Integer, null, out byte result))
                        stack->Push<u8>(result);
                    else throw new InvalidFormatError($"Invalid decimal u8: {input}");

                    break;
                }
                
                case OpCode.InputU16: {
                    Console.WriteLine();
                    #if ESM_DEBUG
                    Debug("Awaiting u16 decimal input...");
                    #endif
                    string input = Console.ReadLine() ?? "";
                    if (ushort.TryParse(input, NumberStyles.Integer, null, out ushort result))
                        stack->Push<u16>(result);
                    else throw new InvalidFormatError($"Invalid decimal u16: {input}");

                    break;
                }
             
                case OpCode.AllocInputStr: {
                    InputString(Console.In, &context);
                    break;
                }
                
                default: throw new InvalidOperationException($"Unknown opcode: 0x{(byte)opcode:X2}");
            }
            
            pc++;
        }

        return null;
    }

    static void Debug(string s) {
        Console.WriteLine($"\e[0;37m{s}\e[0m");
    }
}