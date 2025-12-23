using EsmRuntime.Common;
using EsmRuntime.Common.Types;
using static EsmCompiler.FixedSizeType;

namespace EsmCompiler;

public enum FixedSizeType {
    I8, U8,
    I16, U16,
    I32, U32,
    I64, U64,
    Isize, Usize,
    StrPointer
}

public static class FixedSizeExt {
    extension(FixedSizeType self) {
        public int? SizeInBytecode
            => self switch {
                I8 => i8.ByteCount,
                U8 => u8.ByteCount,
                I16 => i16.ByteCount,
                U16 => u16.ByteCount,
                I32 => i32.ByteCount,
                U32 => u32.ByteCount,
                I64 => i64.ByteCount,
                U64 => u64.ByteCount,
                Usize or Isize or StrPointer => null,
                _ => throw new InvalidOperationException()
            };

        public int ByteCount
            => self switch {
                I8 => i8.ByteCount,
                U8 => u8.ByteCount,
                I16 => i16.ByteCount,
                U16 => u16.ByteCount,
                I32 => i32.ByteCount,
                U32 => u32.ByteCount,
                I64 => i64.ByteCount,
                U64 => u64.ByteCount,
                Isize => isize.ByteCount,
                Usize or StrPointer => usize.ByteCount,
                _ => throw new InvalidOperationException()
            };

        public OpCode MemStorage
            => self switch {
                I8 or U8 => OpCode.StoreGlobalX8,
                I16 or U16 => OpCode.StoreGlobalX16,
                I32 or U32 => OpCode.StoreGlobalX32,
                I64 or U64 => OpCode.StoreGlobalX64,
                Isize or Usize or StrPointer => OpCode.StoreGlobalXsize,
                _ => throw new InvalidOperationException()
            };

        public OpCode LocStorage
            => self switch {
                I8 or U8 => OpCode.StoreLocX8,
                I16 or U16 => OpCode.StoreLocX16,
                I32 or U32 => OpCode.StoreLocX32,
                I64 or U64 => OpCode.StoreLocX64,
                Isize or Usize or StrPointer => OpCode.StoreLocXsize,
                _ => throw new InvalidOperationException()
            };

        public static FixedSizeType ByToken(int token) {
            return token switch {
                EsmLexer.I8 => I8,
                EsmLexer.I16 => I16,
                EsmLexer.I32 => I32,
                EsmLexer.I64 => I64,
                EsmLexer.U8 => U8,
                EsmLexer.U16 => U16,
                EsmLexer.U32 => U32,
                EsmLexer.U64 => U64,
                EsmLexer.USize => Usize,
                EsmLexer.ISize => Isize,
                EsmLexer.Str => StrPointer,
                _ => throw new InvalidOperationException()
            };
        }

        public OpCode Plus
            => self switch {
                I8 or U8 => OpCode.PlusX8,
                U16 or I16 => OpCode.PlusX16,
                I32 or U32 => OpCode.PlusX32,
                I64 or U64 => OpCode.PlusX64,
                Usize or Isize => OpCode.PlusPtr,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Minus
            => self switch {
                I8 or U8 => OpCode.MinusX8,
                U16 or I16 => OpCode.MinusX16,
                I32 or U32 => OpCode.MinusX32,
                I64 or U64 => OpCode.MinusX64,
                Usize or Isize => OpCode.MinusXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode And
            => self switch {
                I8 or U8 => OpCode.MinusX8,
                U16 or I16 => OpCode.MinusX16,
                I32 or U32 => OpCode.MinusX32,
                I64 or U64 => OpCode.MinusX64,
                Usize or Isize => OpCode.MinusXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Or
            => self switch {
                I8 or U8 => OpCode.OrX8,
                U16 or I16 => OpCode.OrX16,
                I32 or U32 => OpCode.OrX32,
                I64 or U64 => OpCode.OrX64,
                Usize or Isize => OpCode.OrXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Xor
            => self switch {
                I8 or U8 => OpCode.XorX8,
                U16 or I16 => OpCode.XorX16,
                I32 or U32 => OpCode.XorX32,
                I64 or U64 => OpCode.XorX64,
                Usize or Isize => OpCode.XorXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Not
            => self switch {
                I8 or U8 => OpCode.NotX8,
                U16 or I16 => OpCode.NotX16,
                I32 or U32 => OpCode.NotX32,
                I64 or U64 => OpCode.NotX64,
                Usize or Isize => OpCode.NotXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode LShift
            => self switch {
                I8 or U8 => OpCode.LeftX8,
                U16 or I16 => OpCode.LeftX16,
                I32 or U32 => OpCode.LeftX32,
                I64 or U64 => OpCode.LeftX64,
                Usize or Isize => OpCode.LeftXsize,
                _ => throw new InvalidOperationException()
            };
        
                
        public OpCode RShift
            => self switch {
                I8 => OpCode.RightI8,
                I16 => OpCode.RightI16,
                I32 => OpCode.RightI32,
                I64 => OpCode.RightI64,
                Isize => OpCode.RightIsize,
                U8 => OpCode.RightU8,
                U16 => OpCode.RightU16,
                U32 => OpCode.RightU32,
                U64 => OpCode.RightU64, 
                Usize => OpCode.RightUsize, 
                _ => throw new InvalidOperationException()
            };

        public OpCode UrShift
            => self switch {
                I8 or U8 => OpCode.RightU8,
                U16 or I16 => OpCode.RightU16,
                I32 or U32 => OpCode.RightU32,
                I64 or U64 => OpCode.RightU64,
                Usize or Isize => OpCode.RightUsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Equal
            => self switch {
                I8 or U8 => OpCode.EqualX8,
                U16 or I16 => OpCode.EqualX16,
                I32 or U32 => OpCode.EqualX32,
                I64 or U64 => OpCode.EqualX64,
                Usize or Isize => OpCode.EqualXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode NEqual
            => self switch {
                I8 or U8 => OpCode.NEqualX8,
                U16 or I16 => OpCode.NEqualX16,
                I32 or U32 => OpCode.NEqualX32,
                I64 or U64 => OpCode.NEqualX64,
                Usize or Isize => OpCode.NEqualXsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Mult
            => self switch {
                I8 or U8 => OpCode.MultX8,
                U16 or I16 => OpCode.MultX16,
                I32 or U32 => OpCode.MultX32,
                I64 or U64 => OpCode.MultX64,
                Usize or Isize => OpCode.MultXsize,
                _ => throw new InvalidOperationException()
            };
        
        
        public OpCode Div
            => self switch {
                I8 => OpCode.DivI8,
                I16 => OpCode.DivI16,
                I32 => OpCode.DivI32,
                I64 => OpCode.DivI64,
                Isize => OpCode.DivIsize,
                U8 => OpCode.DivU8,
                U16 => OpCode.DivU16,
                U32 => OpCode.DivU32,
                U64 => OpCode.DivU64, 
                Usize => OpCode.DivUsize,
                _ => throw new InvalidOperationException()
            };
        
        public OpCode Mod
            => self switch {
                I8 => OpCode.ModI8,
                I16 => OpCode.ModI16,
                I32 => OpCode.ModI32,
                I64 => OpCode.ModI64,
                Isize => OpCode.ModIsize,
                U8 => OpCode.ModU8,
                U16 => OpCode.ModU16,
                U32 => OpCode.ModU32,
                U64 => OpCode.ModU64,
                Usize => OpCode.ModUsize,
                _ => throw new InvalidOperationException()
            };
        
    }
}