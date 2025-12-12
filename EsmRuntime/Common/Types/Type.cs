/*namespace EsmRuntime.Common.Types;

public struct Type: IByteSerializable<Type> {
    
    long Type 

    TypeFlags flags;
    
    public void ToSpan(Span<byte> span) {
        throw new NotImplementedException();
    }
    public unsafe void ToPtr(byte* ptr) {
        throw new NotImplementedException();
    }
    public static Type FromSpan(ReadOnlySpan<byte> bytes) => throw new NotImplementedException();

    public static unsafe Type FromPtr(byte* ptr) => throw new NotImplementedException();
    public int InstanceSize { get; }
}

public enum TypeFlags : byte {
    // Kind
    Value = 0b0001,
    Reference = 0b0010,
    Kind = Value | Reference,
    
    // Ref, pointer, etc
    Direct  = 0b0100,
    Pointer = 0b1000,
    
}*/