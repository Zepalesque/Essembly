using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.TypeTables;

public ref struct TypeTable {
    
    HashTable<TypeResult> table;
    
}

public unsafe ref struct TypeResult(TypeFlags flags, void* data) : IDisposable {
    
    TypeFlags _flags = flags;
    void* _data = data;
    
    public bool TryGetAs<TSelf>(out TSelf self) 
        where TSelf : unmanaged, IType<TSelf>, allows ref struct {
        if (TSelf.Flags != _flags) {
            self = default;
            return false;
        }
        
        self = *(TSelf*)_data;
        return true;
    }
    
    public void Dispose() {
    }
}