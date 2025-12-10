using System.Numerics;
using System.Text;
using System.Text.Unicode;
using EsmRuntime.Common.Types;
using EsmRuntime.Debug;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace EsmRuntime;

public static partial class EsmVM {
    static u8 LoadConst(ReadOnlySpan<byte> program, ref int pc) {
        u8 val = program[++pc];
        if (_debug) Debug($"Pushing {val} to stack");
        return val;
    }
    
    static T LoadConst<T>(ReadOnlySpan<byte> program, ref int pc) where T: struct, ISizedValue<T> {
        T val = T.FromSpan(program[(pc + 1)..(pc + T.ByteCount + 1)]);
        pc += T.ByteCount;
        if (_debug) Debug($"Pushing {val} to stack");
        return val;
    }

    static unsafe void AllocRef<T>(ReadOnlySpan<byte> program, ref int pc, ref Heap heap) where T: struct, IByteSerializable<T>, IBytecodeSerializable<T>, allows ref struct {
        
        usize loc = usize.FromSpan(program[(pc + 1)..(pc + usize.ByteCount + 1)]);
        pc += usize.ByteCount + 1;

        fixed (byte* ptr = &program[pc]) {
            var data = T.DataToSerialize(ptr);
            var reference = heap.AllocateUnsized<T>(data);
            if (_debug) Debug($"Allocating a {reference.Dereference().InstanceSize}-byte memory block and storing to {loc.Hex}");
            reference.ToPtr(heap[loc]);
        }


    }

    static unsafe u8 LoadMem(ReadOnlySpan<byte> program, ref int pc, ref Heap mem) {
        pc++;
        usize addr = usize.FromProgram(program[++pc..], ref pc);
        if (_debug) Debug($"Attempting to get @.{addr:X2}");
        byte* ptr = mem[addr];
        var val = (u8) (*ptr);
        if (_debug) Debug($"Pushing @.{addr:X2} ({val}) to stack");
        return val;
    }

    static void StoreMem<T>(ReadOnlySpan<byte> program, ref int pc, ref Heap mem, T val)  where T : struct, ISizedValue<T> {
        usize addr = usize.FromSpan(program[++pc..(pc + usize.ByteCount)]);
        if (_debug) Debug($"Storing {val} to &.{addr:X2}");
        pc += usize.ByteCount - 1;
        var span = mem[(int)addr..(int)(addr + T.ByteCount)];
        val.ToSpan(span);
    }

    static void Jump(ReadOnlySpan<byte> program, ref int pc, ref OpStack stack, bool? cond) {
        int? res;
        string? jumpMsg = null;
        if (cond == null) {
            int jump = unchecked((sbyte) program[++pc]) - 2;
            res = pc + jump;
            if (_debug) jumpMsg = $"Jumping: #{pc} + {jump} = #{res}";
        } else {
            var top = stack.Pop<boolean>();
            if (top == cond.Value) {
                int jump = unchecked((sbyte) program[++pc]) - 2;
                res = pc + jump;
                
                if (_debug) {
                    string msgCond = cond.Value ? "false" : "true";
                    jumpMsg = $"stack.Pop() is {msgCond}, jumping: #{pc} + {jump} = #{res}";
                }
            } else {
                res = null;
                if (_debug) {
                    string val = !cond.Value ? "true" : "false";
                    jumpMsg = $"stack.Pop() is {val}, skipping jump";
                }
                pc++;
            }
        }



        if (_debug) Debug(jumpMsg!);
        if (res != null) {
            pc = res.Value;
        }
    }

    static u8 Exit(u8 code) {
        if (_debug) Debug($"User-specified exit: {code}");
        return code;
    }

    static T UnaryNot<T>(T val) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T> {
        T res = ~val;
        if (_debug) Debug($"~{val}: -> {val} ~= -> {res}");
        return res;
    }
    
    static T BinaryAnd<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T> {
        T res = a & b;
        if (_debug) Debug($"{a} & {b}: -> {b}, {a} &= -> {res}");
        return res;
    }
    
    static T BinaryOr<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>  {
        T res = a | b;
        if (_debug) Debug($"{a} | {b}: -> {b}, {a} |= -> {res}");
        return res;
    }
    
    static T BinaryXor<T>(T a, T b) where T: struct, ISizedValue<T>, IBitwiseOperators<T, T, T>  {
        T res = a ^ b;
        if (_debug) Debug($"{a} ^ {b}: -> {b}, {a} ^= -> {res}");
        return res;
    }
    
    static T BinaryLeft<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a << b;
        if (_debug) Debug($"{a} << {b}: -> {b}, {a} <<= -> {res}");
        return res;
    }
    
    static T BinaryRight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a >> b;
        if (_debug) Debug($"{a} >>> {b}: -> {b}, {a} >>= -> {res}");
        return res;
    }
    
    static T BinaryURight<T>(T a, T b) where T: struct, ISizedValue<T>, IShiftOperators<T, T, T>  {
        T res = a >>> b;
        if (_debug) Debug($"{a} >> {b}: -> {b}, {a} >>= -> {res}");
        return res;
    }

    static void PrintBinary<T>(T val) where T : struct, ISizedValue<T>, INumberFormattable {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Bin}");
        Console.Write(val.Bin);
    }
    
    
    static void PrintDecimal<T>(T val) where T : struct, ISizedValue<T>, INumberFormattable {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Dec}");
        Console.Write(val.Dec);
    }
    
    static void PrintHex<T>(T val) where T : struct, ISizedValue<T>, INumberFormattable {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Hex}");
        Console.Write(val.Hex);
    }
    
    static void PrintAscii<T>(T val) where T : struct, ISizedValue<T>, IAsciiFormattable<T> {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        Console.Write((char) val);
    }
    
    static void PrintUtf16<T>(T val) where T : struct, ISizedValue<T>, IUtf16Formattable<T> {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        Console.Write((char) val);
    }
    
    static void PrintString(ref OpStack stack) {
        var s = stack.Pop<Reference<StringSlice>>();
        StringSlice slice = s.Dereference();
        if (_debug) Debug($"Printing string to console: \"{slice.ToString()}\"");

        foreach (byte b in slice.Utf8.Bytes) {
            Console.Write(b);
        }
    }
}