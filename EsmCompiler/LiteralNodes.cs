using System.Numerics;
using EsmCompiler.Util;
using EsmCore;
using EsmRuntime;

namespace EsmCompiler;

public abstract record ConstantNode(FilePos Pos, OpCode Code) : FinalizedStmt(Pos);

public abstract unsafe record ConstantIntNode<T>(T Val, FilePos Pos, OpCode Code) : ConstantNode(Pos, Code) where T : unmanaged {
    public override ReadOnlySpan<byte> OpCode {
        get {
            var bytes = new byte[sizeof(T) + sizeof(OpCode)];
            return bytes;
        }
    }

}

public abstract record Int8BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.OpCode.Load8) where T : unmanaged, IBinaryInteger<T>;
public abstract record Int16BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.OpCode.Load16) where T: unmanaged, IBinaryInteger<T>;
public abstract record Int32BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.OpCode.Load32) where T: unmanaged, IBinaryInteger<T>;
public abstract record Int64BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.OpCode.Load64) where T: unmanaged, IBinaryInteger<T>;

public record U8Constant(byte Val, FilePos Pos) : Int8BitNode<byte>(Val, Pos);
public record U16Constant(ushort Val, FilePos Pos) : Int16BitNode<ushort>(Val, Pos);
public record U32Constant(uint Val, FilePos Pos) : Int32BitNode<uint>(Val, Pos);
public record U64Constant(ulong Val, FilePos Pos) : Int16BitNode<ulong>(Val, Pos);
// public record U128Literal(UInt128 val, FilePos pos) : LiteralNode(pos);


public record I8Constant(sbyte Val, FilePos Pos) : Int8BitNode<sbyte>(Val, Pos);
public record I16Constant(short Val, FilePos Pos) : Int16BitNode<short>(Val, Pos);
public record I32Constant(int Val, FilePos Pos) : Int32BitNode<int>(Val, Pos);
public record I64Constant(long Val, FilePos Pos) : Int16BitNode<long>(Val, Pos);
// public record I128Literal(Int128 val, FilePos pos) : LiteralNode(pos);


// public record F16Literal(Half val, FilePos pos) : LiteralNode(pos);
// public record F32Literal(float val, FilePos pos) : LiteralNode(pos);
// public record F64Literal(double val, FilePos pos) : LiteralNode(pos);
// public record FloatLiteral(double highestPrecision, FilePos pos) : LiteralNode(pos);

