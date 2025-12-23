using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;

/*public unsafe ref struct Heap(byte* start, int size) {
    
    public readonly int Size = size;

    int _endCursor = 0;
    // int _startCursor = 0;

    public usize this[usize addr] {
        get {
            CheckAddr(addr);
            return  (usize) (start + addr);
        }
    }

    public usize Transform(usize addr) => (usize) (start + addr);


    public Reference<T> AllocateUnsized<T>(ReadOnlySpan<byte> data) where T: struct, IByteSerializable<T>, allows ref struct {
        int u = data.Length;
        _endCursor += u;
        byte* ptr = start + Size - _endCursor;
        Span<byte> dest = new(ptr, u);
        data.CopyTo(dest);
        T value = T.FromFatPtr(dest);
        
        return Reference<T>.CreateAt(ptr, value);
    }
    
    // TODO
    public void Free<T>(Reference<T> reference) where T: struct, IByteSerializable<T>, allows ref struct {
        
    }

    public Span<byte> this[Range range] => new Span<byte>(start, Size)[range];

    void CheckAddr(usize offset) {
        if (offset >= Size) throw new MemoryAccessError($"Invalid index {offset} for heap with size {Size}");
    }
}*/

public readonly unsafe ref struct ReferenceHeap(byte* start, nint size) {
    byte* Start { get; } = start;
    nint Size { get; } = size;
    
    public Reference<T> Allocate<T>(T value) where T: struct, IByteSerializable<T>, allows ref struct {
        usize u = value.InstSize;
        if (typeof(T) == typeof(Unit))
            return new(EsmVM.UnitAddr);
        
        if (!HeapTree.TryAllocate(ref EsmVM.HeapMemoryTree, u, out byte* ptr))
            throw new MemoryAccessError("No memory left in reference heap :(");
        
        return Reference<T>.CreateAt(ptr, value);
    }

    public void Free<T>(Reference<T> reference) where T: struct, IByteSerializable<T>, allows ref struct {
        usize addr = reference.Address;
        if (addr == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to free the null pointer!");
        if (addr == EsmVM.UnitAddr) return;
        if (addr < (usize)Start || addr >= (usize)Start +  Size) {
            throw new MemoryAccessError($"Free address {addr} is outside reference heap bounds!");
        }
        
        usize size = reference.DerefSize;
        
        if (!HeapTree.TryFree(ref EsmVM.HeapMemoryTree, reference.Address, reference.Address + size))
            throw new MemoryAccessError("Tried to free already freed memory!");
    }
    
    public void Free(usize addr) {
        if (addr == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to free the null pointer!");
        if (addr == EsmVM.UnitAddr) return;
        if (addr < (usize)Start || addr >= (usize)Start +  Size) {
            throw new MemoryAccessError($"Free address {addr} is outside reference heap bounds!");
        }

        usize size = usize.FromPtr((byte*) addr);
        
        if (!HeapTree.TryFree(ref EsmVM.HeapMemoryTree, addr, addr + size))
            throw new MemoryAccessError("Tried to free already freed memory!");
    }
    
}