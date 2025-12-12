using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using EsmRuntime.Common.Types;
using EsmRuntime.Debug;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace EsmRuntime;

public static partial class EsmVM {
    [MethodImpl(MethodImplOptions.AggressiveInlining), Obsolete("Use generic func")]
    static unsafe u8 LoadConst(ReadOnlySpan<byte> program, int* pc) {
        u8 val = program[++*pc];
        #if DEBUG
        Debug($"Pushing {val} to stack");
        #endif
        return val;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe T LoadConst<T>(ReadOnlySpan<byte> program, int* pc) where T: struct, ISizedValue<T> {
        T val = T.FromSpan(program[(*pc + 1)..(*pc + T.ByteCount + 1)]);
        *pc += T.ByteCount;
        #if DEBUG
        Debug($"Pushing {val} to stack");
        #endif
        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static unsafe Reference<T> AllocRef<T>(ReadOnlySpan<byte> program, int* pc, ReferenceHeap heap) where T: struct, IByteSerializable<T>, IBytecodeSerializable<T>, allows ref struct {

        int ipc = *pc;
        fixed (byte* ptr = &program[ipc]) {
            usize addr = usize.FromBytecode(ptr, pc);
            var data = T.FromBytecode(ptr, pc);
            var reference = heap.Allocate(data);
            #if DEBUG
            Debug($"Allocating a {reference.Dereference().InstanceSize}-byte memory block and storing to &{addr.Hex}");
            #endif
            return reference;
        }
    }
    
    // TODO: Jump table for local frames
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Jump(ReadOnlySpan<byte> program, ref int pc, ref OpStack stack, bool? cond) {
        
        if (cond == null) {
            int jump = unchecked((sbyte) program[++pc]) - 2;
            int res = pc + jump;
            
            #if DEBUG
            Debug($"Jumping: #{pc} + {jump} = #{res}");
            #endif
            
     
            pc = res;
            
        } else {
            var top = stack.Pop<boolean>();
            if (top == cond.Value) {
                int jump = unchecked((sbyte) program[++pc]) - 2;
                int res = pc + jump;

                #if DEBUG
                string msgCond = cond.Value ? "false" : "true";
                Debug($"stack.Pop() is {msgCond}, jumping: #{pc} + {jump} = #{res}");
                #endif
                pc = res;
                
            } else {
                #if DEBUG
                string val = !cond.Value ? "true" : "false";
                Debug($"stack.Pop() is {val}, skipping jump");
                #endif
                
                pc++;
            }
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static u8 Exit(u8 code) {
        
        #if DEBUG
        Debug($"User-specified exit: {code}");
        #endif
        
        return code;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T UnaryNot<T>(T val) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T> {
        T res = ~val;
        
        #if DEBUG
        Debug($"~{val}: -> {val} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryAnd<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T> {
        T res = a & b;
        
        #if DEBUG
        Debug($"{a} & {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryOr<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>  {
        T res = a | b;
        
        #if DEBUG
        Debug($"{a} | {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryXor<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>  {
        T res = a ^ b;
        
        #if DEBUG
        Debug($"{a} ^ {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryLeft<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a << b;
        
        #if DEBUG
        Debug($"{a} << {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryRight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a >> b;
       
       #if DEBUG
        Debug($"{a} +>> {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryURight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a >>> b;
        
        #if DEBUG
        Debug($"{a} >> {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryPlus<T>(T a, T b) where T: struct, ISizedValue<T>, IAdditionOperators<T, T, T>  {
        T res = a + b;
        #if DEBUG
        Debug($"{a} + {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryMinus<T>(T a, T b) where T: struct, ISizedValue<T>, ISubtractionOperators<T, T, T>  {
        T res = a - b;
        #if DEBUG
        Debug($"{a} - {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryMult<T>(T a, T b) where T: struct, ISizedValue<T>, IMultiplyOperators<T, T, T>  {
        T res = a * b;
        #if DEBUG
        Debug($"{a} * {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryDiv<T>(T a, T b) where T: struct, ISizedValue<T>, IDivisionOperators<T, T, T>  {
        T res = a / b;
        #if DEBUG
        Debug($"{a} / {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static T BinaryMod<T>(T a, T b) where T: struct, ISizedValue<T>, IModulusOperators<T, T, T>  {
        T res = a % b;
        #if DEBUG
        Debug($"{a} % {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static boolean BinaryEqual<T>(T a, T b) where T: struct, ISizedValue<T>, IEqualityOperators<T, T, bool>  {
        boolean res = a == b;
        #if DEBUG
        Debug($"{a} == {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static boolean BinaryNEqual<T>(T a, T b) where T: struct, ISizedValue<T>, IEqualityOperators<T, T, bool>  {
        boolean res = a != b;
        #if DEBUG
        Debug($"{a} == {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PrintInteger<T>(T val) where T : struct, ISizedValue<T>, INumberFormattable {
        #if DEBUG
        Debug($"Printing stack.Pop(): {val} formatted {val.Dec}");
        #endif
        Console.Write(val.Dec);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PrintAscii<T>(T val) where T : struct, ISizedValue<T>, IAsciiFormattable<T> {
        #if DEBUG
        Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        #endif
        Console.Write((char) val);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PrintUtf16<T>(T val) where T : struct, ISizedValue<T>, IUtf16Formattable<T> {
        #if DEBUG
        Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        #endif
        Console.Write((char) val);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void PrintString(ref OpStack stack) {
        var s = stack.Pop<Reference<StringSlice>>();
        StringSlice slice = s.Dereference();
        #if DEBUG
        Debug($"Printing string to console: \"{slice.ToString()}\"");
        #endif

        foreach (byte b in slice.Utf8.Bytes) {
            Console.Write(b);
        }
    }
}