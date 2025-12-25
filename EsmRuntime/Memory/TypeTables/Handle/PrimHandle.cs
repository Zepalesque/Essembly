using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Memory.TypeTables.Handle;

public readonly unsafe ref partial struct TypeHandle {
    public readonly ref struct PrimImpl(Signature sig, bool dyn) : ITypeHandle<PrimImpl> {
        readonly Signature _sig = sig;
        
        [MethodImpl(Utils.Inline)]
        public void Dispose() {
            Sig.Dispose();
        }
        
        public u128 SigHash {
            [MethodImpl(Utils.Inline)] get;
        } = sig.Hash();
        
        public Signature Sig {
            [MethodImpl(Utils.Inline)] get => _sig;
        }
    }
    
    [MethodImpl(Utils.Inline)]
    public static NatBox<TypeHandle> Primitive(Signature sig, bool dyn) {
        var impl = new PrimImpl(sig, dyn);
        var ptr = (PrimImpl*)NativeMemory.AlignedAlloc((nuint)sizeof(PrimImpl), (nuint)sizeof(PrimImpl));
        *ptr = impl;
        var handle = new TypeHandle(ptr, TypeHandleKind.Primitive,
            dyn ? TypeFlags.Builtin | TypeFlags.DynSize : TypeFlags.Builtin);
        var box = new NatBox<TypeHandle>(in handle);
        return box;
    }
    
    public ref PrimImpl AsPrim {
        [MethodImpl(Utils.Inline)]
        get {
            if (_kind == TypeHandleKind.Primitive) return ref AsPrimUnsafe;
            throw new InvalidOperationException();
        }
    }
    
    internal ref PrimImpl AsPrimUnsafe {
        [MethodImpl(Utils.Inline)]
        get => ref *PrimPtr;
    }
    
    PrimImpl* PrimPtr {
        [MethodImpl(Utils.Inline)] get => (PrimImpl*)_ptr;
    }
}