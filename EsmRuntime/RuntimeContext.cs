using EsmRuntime.Common;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;

namespace EsmRuntime;

public readonly unsafe ref struct RuntimeContext(FatPtr program, ReferenceHeap* heap, OpStack* stack, nuint* pc) {
    public FatPtr Program { get; } = program;
    
    public ref readonly ReferenceHeap Heap => ref *heap;
    
    public ref nuint Pc => ref *pc;
    public ref OpStack Stack => ref *stack;
}