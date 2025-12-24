using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using EsmRuntime.Common;
using EsmRuntime.Common.Types;
using EsmRuntime.Debug;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using EsmRuntime.Memory.TypeTables;
using JetBrains.Annotations;
using static EsmRuntime.Constants;

// ReSharper disable InconsistentNaming

namespace EsmRuntime;

public static partial class EsmVM {
    [MethodImpl(Inline)]
    static unsafe T LoadConst<T>(RuntimeContext* context) where T: struct, ISizedValue<T> {
        FatPtr program = context->Program;
        ref nuint pc = ref context->Pc;
        
        var val = T.FromBytecode(program, ref pc);
        pc += (nuint) T.ByteCount;
        #if ESM_DEBUG
        Debug($"Pushing {val} to stack");
        #endif
        return val;
    }

    [MethodImpl(Inline)]
    static unsafe Ptr<T> AllocRef<T>(RuntimeContext* context) where T: struct, ITypedValue<T>, IBytecodeSerializable<T>, allows ref struct {
        FatPtr program = context->Program;
        ref nuint pc = ref context->Pc;
        ref readonly Heap heap = ref context->Heap;
        
        usize addr = usize.FromBytecode(program.Ptr + pc, ref pc);
        var data = T.FromBytecode(program.Ptr + pc, ref pc);
        Ptr<T> ptr = heap.Allocate(ref data);
        #if ESM_DEBUG
        Debug($"Allocating a {ptr.Dereference().InstSize}-byte memory block and storing to &{addr.Hex}"); 
        #endif
        return ptr;
    }
    
    // TODO: Jump table for local frames, rework
    [MethodImpl(Inline)]
    static void Jump(FatPtr program, ref nuint pc, ref OpStack stack, bool? cond) {
        
        if (cond == null) {
            nuint jump = (nuint)(unchecked((sbyte) program[++pc]) - 2);
            nuint res = pc + jump;
            
            #if ESM_DEBUG
            Debug($"Jumping: #{pc} + {jump} = #{res}");
            #endif
            
     
            pc = res;
            
        } else {
            var top = stack.Pop<@bool>();
            if (top == cond.Value) {
                nuint jump = (nuint)(unchecked((sbyte) program[++pc]) - 2);
                nuint res = pc + jump;

                #if ESM_DEBUG
                string msgCond = cond.Value ? "false" : "true";
                Debug($"stack.Pop() is {msgCond}, jumping: #{pc} + {jump} = #{res}");
                #endif
                pc = res;
                
            } else {
                #if ESM_DEBUG
                string val = !cond.Value ? "true" : "false";
                Debug($"stack.Pop() is {val}, skipping jump");
                #endif
                
                pc++;
            }
        }

    }

    [MethodImpl(Inline)]
    static u8 Exit(u8 code) {
        
        #if ESM_DEBUG
        Debug($"User-specified exit: {code}");
        #endif
        
        return code;
    }

    [MethodImpl(Inline)]
    static T UnaryNot<T>(T val) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T> {
        T res = ~val;
        
        #if ESM_DEBUG
        Debug($"~{val}: -> {val} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryAnd<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>
        => BinaryAnd<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryAnd<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IBitwiseOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a & b;
        
        #if ESM_DEBUG
        Debug($"{a} & {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryOr<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>
        => BinaryOr<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryOr<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IBitwiseOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a | b;
        
        #if ESM_DEBUG
        Debug($"{a} | {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryXor<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>
        => BinaryXor<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryXor<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IBitwiseOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a ^ b;
        
        #if ESM_DEBUG
        Debug($"{a} ^ {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryLeft<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>
        => BinaryLeft<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryLeft<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IShiftOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a << b;
        
        #if ESM_DEBUG
        Debug($"{a} << {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryRight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>
        => BinaryRight<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryRight<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IShiftOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a >> b;
       
       #if ESM_DEBUG
        Debug($"{a} +>> {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }

    [MethodImpl(Inline)]
    static T BinaryURight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>
        => BinaryURight<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryURight<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IShiftOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a >>> b;
        
        #if ESM_DEBUG
        Debug($"{a} >> {b}: -> {b}, {a} => -> {res}");
        #endif
        
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryPlus<T>(T a, T b) where T: struct, ISizedValue<T>, IAdditionOperators<T, T, T>
        => BinaryPlus<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryPlus<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IAdditionOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a + b;
        #if ESM_DEBUG
        Debug($"{a} + {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryMinus<T>(T a, T b) where T: struct, ISizedValue<T>, ISubtractionOperators<T, T, T>
        => BinaryMinus<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryMinus<T1, T2, TRes>(T1 a, T2 b) where T1: struct, ISubtractionOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a - b;
        #if ESM_DEBUG
        Debug($"{a} - {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryMult<T>(T a, T b) where T: struct, ISizedValue<T>, IMultiplyOperators<T, T, T>
        => BinaryMult<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryMult<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IMultiplyOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a * b;
        #if ESM_DEBUG
        Debug($"{a} * {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }

    [MethodImpl(Inline)]
    static T BinaryDiv<T>(T a, T b) where T: struct, ISizedValue<T>, IDivisionOperators<T, T, T>
        => BinaryDiv<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryDiv<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IDivisionOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a / b;
        #if ESM_DEBUG
        Debug($"{a} / {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static T BinaryMod<T>(T a, T b) where T: struct, ISizedValue<T>, IModulusOperators<T, T, T>
        => BinaryMod<T, T, T>(a, b);

    [MethodImpl(Inline)]
    static TRes BinaryMod<T1, T2, TRes>(T1 a, T2 b) where T1: struct, IModulusOperators<T1, T2, TRes> where T2: struct where TRes: struct, ISizedValue<TRes>  {
        TRes res = a % b;
        #if ESM_DEBUG
        Debug($"{a} % {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static @bool BinaryEqual<T>(T a, T b) where T: struct, ISizedValue<T>, IEqualityOperators<T, T, bool>  {
        @bool res = a == b;
        #if ESM_DEBUG
        Debug($"{a} == {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static @bool BinaryNEqual<T>(T a, T b) where T: struct, ISizedValue<T>, IEqualityOperators<T, T, bool>  {
        @bool res = a != b;
        #if ESM_DEBUG
        Debug($"{a} == {b}: -> {b}, {a} => -> {res}");
        #endif
        return res;
    }
    
    [MethodImpl(Inline)]
    static void PrintInteger<T>(T val) where T : struct, ISizedValue<T>, INumberFormattable {
        #if ESM_DEBUG
        Debug($"Printing stack.Pop(): {val} formatted {val.Dec}");
        #endif
        Console.Write(val.Dec);
    }
    
    [MethodImpl(Inline)]
    static void PrintAscii<T>(T val) where T : struct, ISizedValue<T>, IAsciiFormattable<T> {
        #if ESM_DEBUG
        Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        #endif
        Console.Write((char) val);
    }
    
    [MethodImpl(Inline)]
    static void PrintUtf16<T>(T val) where T : struct, ISizedValue<T>, IUtf16Formattable<T> {
        #if ESM_DEBUG
        Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        #endif
        Console.Write((char) val);
    }
    
    [MethodImpl(Inline)]
    static void PrintString(ref OpStack stack) {
        var s = stack.Pop<Ptr<StringSlice>>();
        StringSlice slice = s.Dereference();
        #if ESM_DEBUG
        Debug($"Printing string to console: \"{slice.ToString()}\"");
        #endif

        foreach (byte b in slice.Utf8) {
            Console.Write(b);
        }
    }
    
    static unsafe void InputString(TextReader reader, RuntimeContext* context) {
        ref readonly Heap heap = ref context->Heap;
        ref OpStack stack = ref context->Stack;
        #if ESM_DEBUG
        Debug("Awaiting string input...");
        #endif
        string input = reader.ReadLine() ?? "";
        int len = Encoding.UTF8.GetByteCount(input);
        Span<byte> span = stackalloc byte[len];
        ref Span<byte> spanRef = ref span;
        Encoding.UTF8.GetBytes(input, span);
        StringSlice slice = new(new(spanRef.StartPtr(), span.Length));
        stack.Push(heap.Allocate(ref slice));
    }
}