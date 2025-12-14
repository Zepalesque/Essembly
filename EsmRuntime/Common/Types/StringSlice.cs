using System.Text;

// ReSharper disable InconsistentNaming

namespace EsmRuntime.Common.Types;

// Null-terminated in the bytecode
public readonly unsafe ref struct StringSlice(Slice<u8> utf8) : IByteSerializable<StringSlice>, IBytecodeSerializable<StringSlice> {
    readonly Slice<u8> _utf8 = utf8;

    public Slice<u8> Utf8 => _utf8;

    public static StringSlice FromBytecode(byte* start, int* pc) {
        var i = 0;
        while (*(start + i) != '\0') {
            i++;
        }

        Slice<u8> slice = new(start, i);
        *pc += i + 1;
        return new(slice);
    }

    public FatPtr ToBytecode(delegate*<nuint, byte*> generator) {
        nuint size = _utf8.InstanceSize + 1;
        
        byte* ptr = generator(size);
        
        _utf8.ToPtr(ptr);
        
        *(ptr + size - 1) = 0x00;

        return new(ptr, size);
    }


    public override string ToString() => Encoding.ASCII.GetString(_utf8.Bytes);
    
    public void ToPtr(byte* ptr) {
        _utf8.ToPtr(ptr);
    }
    public static StringSlice FromFatPtr(byte* ptr, usize size)
        => new(Slice<u8>.FromFatPtr(ptr, size));
    
    public nuint InstanceSize => _utf8.InstanceSize;
}