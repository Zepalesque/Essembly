using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;
public readonly unsafe struct Heap(byte* start, nint size, HeapTree** tree) {
    // NOT ref readonly
    byte* Start { get; } = start;
    nint Size { get; } = size;
    
    public Ptr<T> AllocatePtr<T>(scoped ref T value) where T: struct, ITypedValue<T>, allows ref struct {
        usize u = value.InstSize;
        if (typeof(T) == typeof(Unit))
            return new(EsmVM.UnitAddr);
        
        return HeapTree.TryAllocate(ref *tree, u, out byte* ptr)
            ? Ptr<T>.CreateAt(ptr, ref value)
            : throw new MemoryAccessError("No memory left in reference heap :(");
    }
    
    public Ptr AllocateRawPtr(nuint size) {
        if (size == 0) return new(EsmVM.UnitAddr);
        
        return HeapTree.TryAllocate(ref *tree, size, out byte* ptr)
            ? Ptr.CreateAt(ptr)
            : throw new MemoryAccessError("No memory left in reference heap :(");
    }

    public void Free<T>(Ptr<T> ptr) where T: struct, ITypedValue<T>, allows ref struct {
        isize addr = ptr.Address;
        if (addr == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to free the null pointer!");
        if (addr == EsmVM.UnitAddr) return;
        if (addr < (isize)Start || addr >= (isize)Start + Size) {
            throw new MemoryAccessError($"Free address {addr} is outside reference heap bounds!");
        }
        
        usize size = ptr.DerefSize;
        
        if (!HeapTree.TryFree(ref *tree, ptr.Address, ptr.Address + (isize)size))
            throw new MemoryAccessError("Tried to free already freed memory!");
    }
}