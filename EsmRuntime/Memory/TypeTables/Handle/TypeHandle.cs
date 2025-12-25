using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Memory.TypeTables.Handle;

public readonly unsafe ref partial struct TypeHandle : IDisposable {
    readonly TypeHandleKind _kind;
    readonly void* _ptr;
    readonly TypeFlags _flags;
    
    Signature Sig {
        [MethodImpl(Utils.Inline)]
        get => _kind switch {
            TypeHandleKind.Primitive => AsPrim.Sig,
            TypeHandleKind.Pointer => AsPtr.Sig,
            _ => throw new InvalidOperationException()
        };
    }
    
    u128 SigHash {
        [MethodImpl(Utils.Inline)]
        get => _kind switch {
            TypeHandleKind.Primitive => AsPrim.SigHash,
            TypeHandleKind.Pointer => AsPtr.SigHash,
            _ => throw new InvalidOperationException()
        };
    }
    
    [MethodImpl(Utils.Inline)]
    public bool Is(TypeFlags flags)
        => _flags.HasFlag(flags);
    
    public void Dispose() {
        switch (_kind) {
            case TypeHandleKind.Primitive: AsPrim.Dispose(); break;
            default: throw new InvalidOperationException();
        }
        NativeMemory.AlignedFree(_ptr);
    }
    
    [MethodImpl(Utils.Inline)]
    TypeHandle(void* ptr, TypeHandleKind kind, TypeFlags flags) {
        _ptr = ptr;
        _kind = kind;
        _flags = flags;
    }
    
    public enum TypeHandleKind : byte {
        Primitive, Pointer
    }
}

public interface ITypeHandle<out TSelf> : IDisposable
    where TSelf : struct, ITypeHandle<TSelf>, allows ref struct {
    
    u128 SigHash { get; }
    
    Signature Sig { get; }
}
