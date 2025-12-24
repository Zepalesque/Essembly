

namespace EsmRuntime.Common.Types;

public interface IByteReadable<out T> where T: struct, IByteReadable<T>, allows ref struct {
    public static abstract unsafe T FromFatPtr(byte* ptr, usize size);
}

public interface IByteSerializable<out T> : IByteReadable<T> where T: struct, IByteSerializable<T>, allows ref struct {
    unsafe void ToPtr(byte* ptr);
    public nuint InstSize { get; }
    
    public bool IsConstSize => false;
}


public interface IBytecodeSerializable<out T>: IByteSerializable<T> where T : struct, IBytecodeSerializable<T>, allows ref struct {
    public static abstract unsafe T FromBytecode(byte* start, scoped ref nuint pc);
    public byte[] ToBytecode();
}

public interface ISizedValue<out T> : IBytecodeSerializable<T> where T : struct, ISizedValue<T>, allows ref struct {
    public static abstract usize ByteCount { get; }
    nuint IByteSerializable<T>.InstSize => T.ByteCount;
    bool IByteSerializable<T>.IsConstSize => true;
    
    static abstract unsafe T FromPtr(byte* ptr);
}


public interface ISizedPrimValue<out T> : ISizedTypeValue<T>, IPrimValue<T> where T : struct, ISizedPrimValue<T>, allows ref struct;

public interface ITypedValue<out T> : IByteSerializable<T> where T : struct, ITypedValue<T>, allows ref struct { }

public interface ISizedTypeValue<out T> : ISizedValue<T>, ITypedValue<T>
    where T : struct, ISizedTypeValue<T>, allows ref struct {
    
}

public interface IPrimValue<out T> : ITypedValue<T> where T : struct, IPrimValue<T>, allows ref struct {
    public static abstract ReadOnlySpan<byte> Signature { get; }
}