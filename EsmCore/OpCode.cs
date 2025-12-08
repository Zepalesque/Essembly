namespace EsmCore;

public enum OpCode : byte {
    LoadConst    = 0,
    LoadStr      = 1,
    LoadMem      = 2,
    Store        = 3,
    Exit         = 4,
    Jump         = 5,
    JumpIfZero   = 6,
    JumpIfNZero  = 7,
    BwNot        = 8,
    BwAnd        = 9,
    BwOr         = 10,
    BwXor        = 11,
    BwLeft       = 12,
    BwRight      = 13,
    PrintAscii   = 14,
    PrintBin     = 15,
    PrintDec     = 16,
    PrintHex     = 17,
    PrintStr     = 18,
    InputAscii   = 19,
    InputDec     = 20,
    InputHex     = 21,
    InputBin     = 22,
    InputStr     = 23,
}

public static class OpCodeExt {
    extension(OpCode self) {
        public bool? JumpCondition
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            => self switch {
                OpCode.Jump => null,
                OpCode.JumpIfNZero => false,
                OpCode.JumpIfZero => true,
                _ => throw new InvalidOperationException()
            };

        public static OpCode get_Item(bool? cond)
            => cond switch {
                null => OpCode.Jump,
                false => OpCode.JumpIfNZero,
                true => OpCode.JumpIfZero,
            };
    }
}