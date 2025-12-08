using System.Text;
using EsmCompiler.Util;
using JetBrains.Annotations;

namespace EsmRuntime;

public static partial class EsmRuntime {
    static u8 LoadConst(ReadOnlySpan<byte> program, ref int pc) {
        u8 val = program[++pc];
        if (_debug) Debug($"Pushing {val} to stack");
        return val;
    }

    static void LoadStr(ReadOnlySpan<byte> program, ref int pc, ref OpStack stack) {
        int start = pc + 1;
        do {
            pc++; 
        } while (program[pc] != '\0');

        var span = program[start..(pc+1)];
        if (_debug) Debug($"Pushing \"{Encoding.ASCII.GetString(span)}\\0\" to stack");

        stack += span;
    }

    static u8 LoadMem(ReadOnlySpan<byte> program, ref int pc, ref Memory mem) {
        byte ptr = program[++pc];
        if (_debug) Debug($"Attempting to get @.{ptr:X2}");
        u8 val = mem[ptr];
        if (_debug) Debug($"Pushing @.{ptr:X2} ({val}) to stack");
        return val;
    }

    static void StoreMem(ReadOnlySpan<byte> program, ref int pc, ref Memory mem, u8 val) {
        byte loc = program[++pc];
        if (_debug) Debug($"Storing {val} to &.{loc:X2}");
        mem[loc] = val;
    }

    static void Jump(ReadOnlySpan<byte> program, ref int pc, ref OpStack stack, bool? cond) {
        int? res;
        string? jumpMsg = null;
        if (cond == null) {
            int jump = unchecked((sbyte) program[++pc]) - 2;
            res = pc + jump;
            if (_debug) jumpMsg = $"Jumping: #{pc} + {jump} = #{res}";
        } else {
            u8 top = stack.Pop();
            if (top == 0 == cond.Value) {
                int jump = unchecked((sbyte) program[++pc]) - 2;
                res = pc + jump;
                
                if (_debug) {
                    string msgCond = cond.Value ? "==" : "!=";
                    jumpMsg = $"stack.Pop() {msgCond} 0, jumping: #{pc} + {jump} = #{res}";
                }
            } else {
                res = null;
                if (_debug) {
                    string msgCond = !cond.Value ? "==" : "!=";
                    jumpMsg = $"stack.Pop() {msgCond} 0, skipping jump";
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

    static u8 UnaryNot(u8 val) {
        u8 res = ~val;
        if (_debug) Debug($"~{val}: -> {val} ~= -> {res}");
        return res;
    }
    
    static u8 BinaryAnd(u8 a, u8 b) {
        u8 res = a & b;
        if (_debug) Debug($"{a} & {b}: -> {b}, {a} &= -> {res}");
        return res;
    }
    
    static u8 BinaryOr(u8 a, u8 b) {
        u8 res = a | b;
        if (_debug) Debug($"{a} | {b}: -> {b}, {a} |= -> {res}");
        return res;
    }
    
    static u8 BinaryXor(u8 a, u8 b) {
        u8 res = a ^ b;
        if (_debug) Debug($"{a} ^ {b}: -> {b}, {a} ^= -> {res}");
        return res;
    }
    
    static u8 BinaryLeft(u8 a, u8 b) {
        u8 res = a << b;
        if (_debug) Debug($"{a} << {b}: -> {b}, {a} <<= -> {res}");
        return res;
    }
    
    static u8 BinaryRight(u8 a, u8 b) {
        u8 res = a >> b;
        if (_debug) Debug($"{a} >> {b}: -> {b}, {a} >>= -> {res}");
        return res;
    }

    static void PrintBinary(u8 val) {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Bin}");
        Console.Write(val.Bin);
    }
    
    
    static void PrintDecimal(u8 val) {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Dec}");
        Console.Write(val.Dec);
    }
    
    static void PrintHex(u8 val) {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted {val.Hex}");
        Console.Write(val.Hex);
    }
    
    static void PrintAscii(u8 val) {
        if (_debug) Debug($"Printing stack.Pop(): {val} formatted \'{(char) val}\'");
        Console.Write((char) val);
    }
    
    static void PrintString(ref OpStack stack) {
        ReadOnlySpan<byte> str;
        if (_debug) {
            int index = stack.LastIndexOf((byte) '\0');
            if (index == -1) {
                Debug("Printing unbound string to console, StackUnderflow is inevitable...");
                str = stack.PopStr();
            } else
                Debug(
                    $"Printing string to console: \"{(str = stack.PopStr()).ToArray().Map(x => ((char) x).ToString()).Aggregate("", (s1, s2) => s1 + s2)}\"");
        } else {
            str = stack.PopStr();
        }

        foreach (byte b in str) {
            Console.Write((char) b);
        }
    }
}