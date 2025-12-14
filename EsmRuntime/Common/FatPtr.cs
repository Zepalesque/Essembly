using EsmRuntime.Common.Types;
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common;


public readonly unsafe ref struct FatPtr(byte* ptr, nuint size) {
    public readonly byte* Ptr = ptr;
    public readonly usize Size = size;

    public byte this[int i] => *(Ptr + i);
    
    public static implicit operator byte*(FatPtr ptr) => ptr.Ptr;

    public void Deconstruct(out byte* ptr, out int size) {
        ptr = Ptr;
        size = Size;
    }
}