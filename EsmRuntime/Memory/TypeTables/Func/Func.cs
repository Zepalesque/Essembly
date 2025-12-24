using EsmRuntime.Common;

namespace EsmRuntime.Memory.TypeTables.Func;

public ref struct Func(VariableTable parameters, FatPtr bytes) {
    readonly VariableTable _parameters = parameters;
    readonly FatPtr _bytes = bytes;
    
    // public 
}