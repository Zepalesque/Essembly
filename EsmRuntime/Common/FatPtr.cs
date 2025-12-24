using System.Runtime.CompilerServices;
using EsmRuntime.Common.Types;
using static EsmRuntime.Constants;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

namespace EsmRuntime.Common;


[method: MethodImpl(Inline)]
public readonly unsafe ref struct FatPtr(byte* ptr, nuint size) {
    public readonly byte* Ptr = ptr;
    public readonly usize Size = size;
    
    public byte this[nuint i] {
        [MethodImpl(Inline)]
        get => Ptr[i];
    }
    
    [MethodImpl(Inline)]
    public static implicit operator byte*(FatPtr ptr) => ptr.Ptr;
    
    [MethodImpl(Inline)]
    public void Deconstruct(out byte* ptr, out nuint size) {
        ptr = Ptr;
        size = Size;
    }
}