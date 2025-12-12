using EsmRuntime.Common;
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
                FixedSizeType.StrPointer => null,
                _ => throw new InvalidOperationException()
            };


        public int ByteCount
            => self switch {
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

        public OpCode MemStorage
            => self switch {
                FixedSizeType.I8 => OpCode.StoreGlobal8,
                FixedSizeType.I16 => OpCode.StoreGlobal16,
                FixedSizeType.I32 => OpCode.StoreGlobal32,
                FixedSizeType.I64 => OpCode.StoreGlobal64,
                FixedSizeType.U8 => OpCode.StoreGlobal8,
                FixedSizeType.U16 => OpCode.StoreGlobal16,
                FixedSizeType.U32 => OpCode.StoreGlobal32,
                FixedSizeType.U64 => OpCode.StoreGlobal64,
                FixedSizeType.StrPointer => OpCode.StoreGlobal16,
                _ => throw new InvalidOperationException()
            };

        public OpCode LocStorage
            => self switch {
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

        public OpCode Plus
            => self switch {
                FixedSizeType.I8 => OpCode.Plus8,
                FixedSizeType.I16 => OpCode.Plus16,
                FixedSizeType.I32 => OpCode.Plus32,
                FixedSizeType.I64 => OpCode.Plus64,
                FixedSizeType.U8 => OpCode.Plus8,
                FixedSizeType.U16 => OpCode.Plus16,
                FixedSizeType.U32 => OpCode.Plus32,
                FixedSizeType.U64 => OpCode.Plus64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Minus
            => self switch {
                FixedSizeType.I8 => OpCode.Minus8,
                FixedSizeType.I16 => OpCode.Minus16,
                FixedSizeType.I32 => OpCode.Minus32,
                FixedSizeType.I64 => OpCode.Minus64,
                FixedSizeType.U8 => OpCode.Minus8,
                FixedSizeType.U16 => OpCode.Minus16,
                FixedSizeType.U32 => OpCode.Minus32,
                FixedSizeType.U64 => OpCode.Minus64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode And
            => self switch {
                FixedSizeType.I8 => OpCode.And8,
                FixedSizeType.I16 => OpCode.And16,
                FixedSizeType.I32 => OpCode.And32,
                FixedSizeType.I64 => OpCode.And64,
                FixedSizeType.U8 => OpCode.And8,
                FixedSizeType.U16 => OpCode.And16,
                FixedSizeType.U32 => OpCode.And32,
                FixedSizeType.U64 => OpCode.And64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Or
            => self switch {
                FixedSizeType.I8 => OpCode.Or8,
                FixedSizeType.I16 => OpCode.Or16,
                FixedSizeType.I32 => OpCode.Or32,
                FixedSizeType.I64 => OpCode.Or64,
                FixedSizeType.U8 => OpCode.Or8,
                FixedSizeType.U16 => OpCode.Or16,
                FixedSizeType.U32 => OpCode.Or32,
                FixedSizeType.U64 => OpCode.Or64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Xor
            => self switch {
                FixedSizeType.I8 => OpCode.Xor8,
                FixedSizeType.I16 => OpCode.Xor16,
                FixedSizeType.I32 => OpCode.Xor32,
                FixedSizeType.I64 => OpCode.Xor64,
                FixedSizeType.U8 => OpCode.Xor8,
                FixedSizeType.U16 => OpCode.Xor16,
                FixedSizeType.U32 => OpCode.Xor32,
                FixedSizeType.U64 => OpCode.Xor64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Not
            => self switch {
                FixedSizeType.I8 => OpCode.Not8,
                FixedSizeType.I16 => OpCode.Not16,
                FixedSizeType.I32 => OpCode.Not32,
                FixedSizeType.I64 => OpCode.Not64,
                FixedSizeType.U8 => OpCode.Not8,
                FixedSizeType.U16 => OpCode.Not16,
                FixedSizeType.U32 => OpCode.Not32,
                FixedSizeType.U64 => OpCode.Not64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode LShift
            => self switch {
                FixedSizeType.I8 => OpCode.Left8,
                FixedSizeType.I16 => OpCode.Left16,
                FixedSizeType.I32 => OpCode.Left32,
                FixedSizeType.I64 => OpCode.Left64,
                FixedSizeType.U8 => OpCode.Left8,
                FixedSizeType.U16 => OpCode.Left16,
                FixedSizeType.U32 => OpCode.Left32,
                FixedSizeType.U64 => OpCode.Left64, 
                _ => throw new InvalidOperationException()
            };
        
                
        public OpCode RShift
            => self switch {
                FixedSizeType.I8 => OpCode.RightI8,
                FixedSizeType.I16 => OpCode.RightI16,
                FixedSizeType.I32 => OpCode.RightI32,
                FixedSizeType.I64 => OpCode.RightI64,
                FixedSizeType.U8 => OpCode.RightU8,
                FixedSizeType.U16 => OpCode.RightU16,
                FixedSizeType.U32 => OpCode.RightU32,
                FixedSizeType.U64 => OpCode.RightU64, 
                _ => throw new InvalidOperationException()
            };

        public OpCode UrShift
            => self switch {
                FixedSizeType.I8 => OpCode.RightU8,
                FixedSizeType.I16 => OpCode.RightU16,
                FixedSizeType.I32 => OpCode.RightU32,
                FixedSizeType.I64 => OpCode.RightU64,
                FixedSizeType.U8 => OpCode.RightU8,
                FixedSizeType.U16 => OpCode.RightU16,
                FixedSizeType.U32 => OpCode.RightU32,
                FixedSizeType.U64 => OpCode.RightU64,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Equal
            => self switch {
                FixedSizeType.I8 => OpCode.Equal8,
                FixedSizeType.I16 => OpCode.Equal16,
                FixedSizeType.I32 => OpCode.Equal32,
                FixedSizeType.I64 => OpCode.Equal64,
                FixedSizeType.U8 => OpCode.Equal8,
                FixedSizeType.U16 => OpCode.Equal16,
                FixedSizeType.U32 => OpCode.Equal32,
                FixedSizeType.U64 => OpCode.Equal64,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode NEqual
            => self switch {
                FixedSizeType.I8 => OpCode.NEqual8,
                FixedSizeType.I16 => OpCode.NEqual16,
                FixedSizeType.I32 => OpCode.NEqual32,
                FixedSizeType.I64 => OpCode.NEqual64,
                FixedSizeType.U8 => OpCode.NEqual8,
                FixedSizeType.U16 => OpCode.NEqual16,
                FixedSizeType.U32 => OpCode.NEqual32,
                FixedSizeType.U64 => OpCode.NEqual64,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Mult
            => self switch {
                FixedSizeType.I8 => OpCode.Mult8,
                FixedSizeType.I16 => OpCode.Mult16,
                FixedSizeType.I32 => OpCode.Mult32,
                FixedSizeType.I64 => OpCode.Mult64,
                FixedSizeType.U8 => OpCode.Mult8,
                FixedSizeType.U16 => OpCode.Mult16,
                FixedSizeType.U32 => OpCode.Mult32,
                FixedSizeType.U64 => OpCode.Mult64,
                _ => throw new InvalidOperationException()
            };
        
        
        public OpCode Div
            => self switch {
                FixedSizeType.I8 => OpCode.DivI8,
                FixedSizeType.I16 => OpCode.DivI16,
                FixedSizeType.I32 => OpCode.DivI32,
                FixedSizeType.I64 => OpCode.DivI64,
                FixedSizeType.U8 => OpCode.DivU8,
                FixedSizeType.U16 => OpCode.DivU16,
                FixedSizeType.U32 => OpCode.DivU32,
                FixedSizeType.U64 => OpCode.DivU64, 
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Mod
            => self switch {
                FixedSizeType.I8 => OpCode.ModI8,
                FixedSizeType.I16 => OpCode.ModI16,
                FixedSizeType.I32 => OpCode.ModI32,
                FixedSizeType.I64 => OpCode.ModI64,
                FixedSizeType.U8 => OpCode.ModU8,
                FixedSizeType.U16 => OpCode.ModU16,
                FixedSizeType.U32 => OpCode.ModU32,
                FixedSizeType.U64 => OpCode.ModU64, 
                _ => throw new InvalidOperationException()
            };
        
    }
}