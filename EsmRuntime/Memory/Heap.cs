using EsmRuntime.Common.Types;
using EsmRuntime.Memory;

namespace EsmRuntime.Storage;

// TODO: Split into sized heap (ints, structs(?), pointers) and unsized (slices, string slices)
public unsafe ref struct Heap(byte* start, int size) {
    
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
        T value = T.FromSpan(dest);
        
        return Reference<T>.CreateAt(ptr, value);
    }
    
    // TODO
    public void Free<T>(Reference<T> reference) where T: struct, IByteSerializable<T>, allows ref struct {
        
    }

    public Span<byte> this[Range range] => new Span<byte>(start, Size)[range];

    void CheckAddr(usize offset) {
        if (offset >= Size) throw new MemoryAccessError($"Invalid index {offset} for heap with size {Size}");
    }
}

public unsafe ref struct ReferenceHeap(byte* start, int size) {
    public byte* Start { get; } = start;
    public int Size { get; } = size;
    int _cursor = 0;
    
    public Reference<T> Allocate<T>(ReadOnlySpan<byte> data) where T: struct, IByteSerializable<T>, allows ref struct {
        usize u = data.Length;
        _cursor += u;
        if (!HeapTree.TryAllocate(ref EsmVM.HeapMemoryTree, u, out byte* ptr))
            throw new MemoryAccessError("No memory left :(");
        Span<byte> dest = new(ptr, u);
        data.CopyTo(dest);
        T value = T.FromSpan(dest);
        
        return Reference<T>.CreateAt(ptr, value);
    }

    public void Free<T>(Reference<T> reference) where T: struct, IByteSerializable<T>, allows ref struct {
        usize loc = reference.Address;
        if (loc == EsmVM.NullAddr)
            throw new NullAccessError("Attempted to free the null pointer!");
        if (loc < (usize)Start || loc >= (usize)Start +  Size) {
            throw new MemoryAccessError($"Free address {loc} is outside heap bounds");
        }
        
        usize size = reference.Dereference().InstanceSize;
        
        if (!HeapTree.TryFree(ref EsmVM.HeapMemoryTree, reference.Address, reference.Address + size))
            throw new MemoryAccessError("Already free!");
    }

    
}