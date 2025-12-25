using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Memory.TypeTables.Handle;

public readonly unsafe ref partial struct TypeHandle {
    public readonly ref struct PtrImpl : ITypeHandle<PtrImpl> {
        readonly Signature _sig;
        readonly u128 _sigHash;
        readonly bool _mut;
        
        public PtrImpl(TypeHandle inner, bool mut) {
            using Signature prefix = Signature.AllocCopy(_mut ? "*mut"u8 : "*"u8);
            using Signature sig = inner.Sig;
            Signature full = prefix + sig;
            _sig = full;
            
            var hashSpan = new Buf16<byte>();
            MemoryMarshal.Write(hashSpan, inner.SigHash);
            using Signature hashSig = Signature.AllocCopy(hashSpan);
            _sigHash = (prefix + hashSig).Hash();
            
            _mut = mut;
        }
        
        [MethodImpl(Utils.Inline)]
        public void Dispose() {
            _sig.Dispose();
        }
        
        public u128 SigHash {
            [MethodImpl(Utils.Inline)] get => _sigHash;
        }
        
        public Signature Sig {
            [MethodImpl(Utils.Inline)] get => _sig;
        }
        
        public bool Mutable {
            [MethodImpl(Utils.Inline)] get => _mut;
        }
    }
    
    public static NatBox<TypeHandle> Pointer(TypeHandle inner, bool mut) {
        var impl = new PtrImpl(inner, mut);
        var ptr = (PtrImpl*)NativeMemory.AlignedAlloc((nuint)sizeof(PtrImpl), 16);
        *ptr = impl;
        var handle = new TypeHandle(ptr, TypeHandleKind.Pointer,
            TypeFlags.Builtin | TypeFlags.Generic);
        var box = new NatBox<TypeHandle>(in handle);
        return box;
    }
    
    public ref PtrImpl AsPtr {
        [MethodImpl(Utils.Inline)]
        get {
            if (_kind == TypeHandleKind.Primitive) return ref AsPtrUnsafe;
            throw new InvalidOperationException();
        }
    }
    
    internal ref PtrImpl AsPtrUnsafe {
        [MethodImpl(Utils.Inline)]
        get => ref *PtrPtr;
    }
    
    PtrImpl* PtrPtr {
        [MethodImpl(Utils.Inline)] get => (PtrImpl*)_ptr;
    }
}