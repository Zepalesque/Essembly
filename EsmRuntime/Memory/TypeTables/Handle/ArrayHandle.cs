using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory.TypeTables.Signatures;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Memory.TypeTables.Handle;

public readonly unsafe ref partial struct TypeHandle {
    public readonly ref struct ArrayImpl : ITypeHandle<SliceImpl> {
        readonly Signature _sig;
        readonly u128 _sigHash;
        
        public ArrayImpl(TypeHandle inner, usize size) {
            using Signature sig = inner.Sig;
            ReadOnlySpan<byte> bytes;
            if (nuint.Size == 8)
                bytes = new Buf16<byte>();
            else 
                bytes = new Buf8<byte>();
            if (size.TryFormat()) {
                
            }
            
            using Signature postfix = Signature.AllocCopy("[]"u8);
            Signature full = postfix + sig;
            _sig = full;
            
            var hashSpan = new Buf16<byte>();
            MemoryMarshal.Write(hashSpan, inner.SigHash);
            using Signature hashSig = Signature.AllocCopy(hashSpan);
            _sigHash = (postfix + hashSig).Hash();
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
    }
    
    public static NatBox<TypeHandle> Array(TypeHandle inner) {
        var impl = new ArrayImpl(inner);
        SliceImpl* ptr = Utils.AllocNat(in impl);
        *ptr = impl;
        var handle = new TypeHandle(ptr, TypeHandleKind.Pointer,
            TypeFlags.Builtin | TypeFlags.Generic | TypeFlags.DynSize);
        var box = new NatBox<TypeHandle>(in handle);
        return box;
    }
    
    public ref SliceImpl AsSlice {
        [MethodImpl(Utils.Inline)]
        get {
            if (_kind == TypeHandleKind.Primitive) return ref AsSliceUnsafe;
            throw new InvalidOperationException();
        }
    }
    
    internal ref SliceImpl AsSliceUnsafe {
        [MethodImpl(Utils.Inline)]
        get => ref *SlicePtr;
    }
    
    SliceImpl* SlicePtr {
        [MethodImpl(Utils.Inline)] get => (SliceImpl*)_ptr;
    }
}