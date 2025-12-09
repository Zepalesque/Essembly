using EsmRuntime;
using EsmRuntime.Common.Types;

namespace EsmCompiler;

public enum FixedSizeType {
    I8, U8,
    I16, U16,
    I32, U32,
    I64, U64,
    StrPointer
}

public static class FixedSizeExt {
    extension(FixedSizeType self) {
        public int? SizeInBytecode
            => self switch {
                FixedSizeType.I8 => i8.ByteCount,
                FixedSizeType.U8 => u8.ByteCount,
                FixedSizeType.I16 => i16.ByteCount,
                FixedSizeType.U16 => u16.ByteCount,
                FixedSizeType.I32 => i32.ByteCount,
                FixedSizeType.U32 => u32.ByteCount,
                FixedSizeType.I64 => i64.ByteCount,
                FixedSizeType.U64 => u64.ByteCount,
                FixedSizeType.StrPointer => -1,
                _ => throw new InvalidOperationException()
            }; 
        
        
        public int ByteCount =>
            self switch {
                FixedSizeType.I8 => i8.ByteCount,
                FixedSizeType.I16 => i16.ByteCount,
                FixedSizeType.I32 => i32.ByteCount,
                FixedSizeType.I64 => i64.ByteCount,
                FixedSizeType.U8 => u8.ByteCount,
                FixedSizeType.U16 => u16.ByteCount,
                FixedSizeType.U32 => u32.ByteCount,
                FixedSizeType.U64 => u64.ByteCount,
                FixedSizeType.StrPointer => usize.ByteCount,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode MemStorage =>
            self switch {
                FixedSizeType.I8 => OpCode.StoreMem8,
                FixedSizeType.I16 => OpCode.StoreMem16,
                FixedSizeType.I32 => OpCode.StoreMem32,
                FixedSizeType.I64 => OpCode.StoreMem64,
                FixedSizeType.U8 => OpCode.StoreMem8,
                FixedSizeType.U16 => OpCode.StoreMem16,
                FixedSizeType.U32 => OpCode.StoreMem32,
                FixedSizeType.U64 => OpCode.StoreMem64,
                FixedSizeType.StrPointer => OpCode.StoreMem16,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode LocStorage =>
            self switch {
                FixedSizeType.I8 => OpCode.StoreLoc8,
                FixedSizeType.I16 => OpCode.StoreLoc16,
                FixedSizeType.I32 => OpCode.StoreLoc32,
                FixedSizeType.I64 => OpCode.StoreLoc64,
                FixedSizeType.U8 => OpCode.StoreLoc8,
                FixedSizeType.U16 => OpCode.StoreLoc16,
                FixedSizeType.U32 => OpCode.StoreLoc32,
                FixedSizeType.U64 => OpCode.StoreLoc64,
                FixedSizeType.StrPointer => OpCode.StoreLoc16,
                _ => throw new InvalidOperationException()
            };
        
        public static FixedSizeType ByIndex(int i) {
            return i switch {
                EsmLexer.I8 => FixedSizeType.I8,
                EsmLexer.I16 => FixedSizeType.I16,
                EsmLexer.I32 => FixedSizeType.I32,
                EsmLexer.I64 => FixedSizeType.I64,
                EsmLexer.U8 => FixedSizeType.U8,
                EsmLexer.U16 => FixedSizeType.U16,
                EsmLexer.U32 => FixedSizeType.U32,
                EsmLexer.U64 => FixedSizeType.U64,
                EsmLexer.Str => FixedSizeType.StrPointer,
                _ => throw new InvalidOperationException()
            };
        }
    }
}