using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.TypeTable;

/*public ref struct TypeTable {
    
    HashTable<TypeResult> 
    
}*/

public unsafe ref struct TypeResult(TypeFlags flags, void* data) : IDisposable {
    
    TypeFlags _flags = flags;
    void* _data = data;
    
    public bool TryGetAs<TSelf, TValue>(out TSelf self) 
        where TSelf : unmanaged, IType<TSelf, TValue>, allows ref struct
        where TValue : struct, ITypedValue<TValue>, allows ref struct {
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