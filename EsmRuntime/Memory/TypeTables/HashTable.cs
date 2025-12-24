using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static EsmRuntime.Constants;

namespace EsmRuntime.Memory.TypeTables;

public readonly unsafe ref struct HashTable<T> : IDisposable
    where T : unmanaged, IDisposable, allows ref struct {
    readonly HashNode<T>** _nodes;
    readonly uint _mask;
    
    [MethodImpl(Inline)]
    public HashTable(uint capPwr) {
        uint cap = 1u << (int)capPwr;
        _mask = cap - 1;
        uint byteCount = cap * (uint) sizeof(HashNode<T>*);
        var nodes = (HashNode<T>**)NativeMemory.AlignedAlloc(byteCount, 16);
        if (nodes != null) {
            NativeMemory.Clear(nodes, byteCount);
            _nodes = nodes;
        } else {
            NativeMemory.Free(nodes);
            _nodes = null;
        }
    }
    
    [MethodImpl(Inline)]
    public bool Insert(UInt128 hash, T value) {
        if (_nodes == null) return false;
        
        uint index = BucketKey(hash);
        uint psl = 0;
        
        HashNode<T>* curr = HashNode<T>.Alloc(hash, value, psl);
        
        while (psl <= _mask) {
            uint bucket = Wrap(index + psl);
            ref HashNode<T>* node = ref _nodes[bucket];
            
            if (node == null) {
                curr->Psl = psl;
                node = curr;
                return true;
            }
            
            if (psl > node->Psl) {
                curr->Psl = psl;
                
                HashNode<T>* capture = node;
                node = curr;
                curr = capture;
                
                psl = curr->Psl;
                index = BucketKey(curr->Key);
            }
            
            psl++;
        }
        
        NativeMemory.AlignedFree(curr);
        return false;
    }
    
    [MethodImpl(Inline)]
    public bool Get(UInt128 hash, out T* value) {
        if (_nodes == null) {
            value = null;
            return false;
        }
        
        uint index = BucketKey(hash);
        uint psl = 0;
        
        while (psl <= _mask) {
            uint bucket = Wrap(index + psl);
            HashNode<T>* node = _nodes[bucket];
            
            if (node == null) {
                value = null;
                return false;
            }
            
            if (node->Key == hash) {
                value = &node->Value;
                return true;
            }
            
            if (psl > node->Psl) {
                value = null;
                return false;
            }
            
            psl++;
        }
        
        value = null;
        return false;
    }
    
    [MethodImpl(Inline)]
    uint BucketKey(UInt128 key) => Wrap((uint) key);
    
    [MethodImpl(Inline)]
    uint Wrap(uint key) => key & _mask;
    
    [MethodImpl(Inline)]
    public void Dispose() {
        if (_nodes == null) return;
        for (uint i = 0; i <= _mask; i++) {
            HashNode<T>* node = _nodes[i];
            if (node != null) {
                node->Value.Dispose();
                NativeMemory.AlignedFree(node);
            };
        }
        
        NativeMemory.AlignedFree(_nodes);
    }
}

[method: MethodImpl(Inline)]
public unsafe ref struct HashNode<T>(UInt128 key, T value, uint psl) where T: unmanaged, allows ref struct {
    public readonly UInt128 Key = key;
    public T Value = value;
    public uint Psl = psl;
    
    [MethodImpl(Inline)]
    public static HashNode<T>* Alloc(UInt128 key, T value, uint offset = 0) {
        var ptr = (HashNode<T>*)NativeMemory.AlignedAlloc((nuint)sizeof(HashNode<T>), 16);
        
        *ptr = new(key, value, offset);
        
        return ptr;
    }
}