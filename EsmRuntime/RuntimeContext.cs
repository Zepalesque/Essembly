using System.Runtime.CompilerServices;
using EsmRuntime.Common;
using EsmRuntime.Memory;
using EsmRuntime.Memory.Heap;
using EsmRuntime.Memory.Util;

namespace EsmRuntime;

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct RuntimeContext(FatPtr program, Heap* heap, OpStack* stack, nuint* pc) {
    public FatPtr Program { [MethodImpl(Utils.Inline)] get; } = program;
    
    public ref readonly Heap Heap { [MethodImpl(Utils.Inline)] get => ref *heap; }
    
    public ref nuint Pc { [MethodImpl(Utils.Inline)] get => ref *pc; }
    
    public ref OpStack Stack { [MethodImpl(Utils.Inline)] get => ref *stack; }
}