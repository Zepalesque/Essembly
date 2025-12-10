using EsmCompiler.Util;
using EsmRuntime;
using EsmRuntime.Common;
using EsmRuntime.Common.Types;

namespace EsmCompiler;

public abstract record ConstantNode(FilePos Pos, OpCode Code) : FinalizedStmt(Pos);

public abstract unsafe record ConstantIntNode<T>(T Val, FilePos Pos, OpCode Code) : ConstantNode(Pos, Code) where T : unmanaged, ISizedValue<T> {
    public override ReadOnlySpan<byte> OpCode {
        get {
            var bytes = new byte[sizeof(T) + sizeof(OpCode)];
            return bytes;
        }
    }

}

public abstract record Int8BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.Common.OpCode.Push8) where T : unmanaged, ISizedValue<T>;
public abstract record Int16BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.Common.OpCode.Push16) where T: unmanaged, ISizedValue<T>;
public abstract record Int32BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.Common.OpCode.Push32) where T: unmanaged, ISizedValue<T>;
public abstract record Int64BitNode<T>(T Val, FilePos Pos) : ConstantIntNode<T>(Val, Pos, EsmRuntime.Common.OpCode.Push64) where T: unmanaged, ISizedValue<T>;

public record U8Constant(u8 Val, FilePos Pos) : Int8BitNode<u8>(Val, Pos);
public record U16Constant(u16 Val, FilePos Pos) : Int16BitNode<u16>(Val, Pos);
public record U32Constant(u32 Val, FilePos Pos) : Int32BitNode<u32>(Val, Pos);
public record U64Constant(u64 Val, FilePos Pos) : Int64BitNode<u64>(Val, Pos);
// public record U128Literal(UInt128 val, FilePos pos) : LiteralNode(pos);



public record I8Constant(i8 Val, FilePos Pos) : Int8BitNode<i8>(Val, Pos);
public record I16Constant(i16 Val, FilePos Pos) : Int16BitNode<i16>(Val, Pos);
public record I32Constant(i32 Val, FilePos Pos) : Int32BitNode<i32>(Val, Pos);
public record I64Constant(i64 Val, FilePos Pos) : Int64BitNode<i64>(Val, Pos);
// public record I128Literal(Int128 val, FilePos pos) : LiteralNode(pos);


// public record F16Literal(Half val, FilePos pos) : LiteralNode(pos);
// public record F32Literal(float val, FilePos pos) : LiteralNode(pos);
// public record F64Literal(double val, FilePos pos) : LiteralNode(pos);
// public record FloatLiteral(double highestPrecision, FilePos pos) : LiteralNode(pos);

