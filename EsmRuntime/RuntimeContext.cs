using System.Runtime.CompilerServices;
using EsmRuntime.Common;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using static EsmRuntime.Constants;

namespace EsmRuntime;

[method: MethodImpl(Inline)]
public readonly unsafe ref struct RuntimeContext(FatPtr program, ReferenceHeap* heap, OpStack* stack, nuint* pc) {
    public FatPtr Program { [MethodImpl(Inline)] get; } = program;
    
    public ref readonly ReferenceHeap Heap { [MethodImpl(Inline)] get => ref *heap; }
    
    public ref nuint Pc { [MethodImpl(Inline)] get => ref *pc; }
    
    public ref OpStack Stack { [MethodImpl(Inline)] get => ref *stack; }
}