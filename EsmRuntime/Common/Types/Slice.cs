using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EsmRuntime.Memory.Util;

namespace EsmRuntime.Common.Types;

[method: MethodImpl(Utils.Inline)]
public readonly unsafe ref struct Slice<T>(byte* start, usize byteLength): ITypedValue<Slice<T>>, IBytecodeSerializable<Slice<T>> where T: struct, ISizedTypeValue<T>, allows ref struct {
    byte* Start {
        [MethodImpl(Utils.Inline)] get => start;
    }
    
    public usize Length {
        [MethodImpl(Utils.Inline)] get;
    } = byteLength / T.ByteCount;
    
    [MethodImpl(Utils.Inline)]
    public static Slice<T> Create(ReadOnlySpan<byte> span) {
        ref byte reference = ref MemoryMarshal.GetReference(span);
        var ptr = (byte*)Unsafe.AsPointer(ref reference);
        return new(ptr, span.Length);
    }
    
    // TODO: Custom span impl with nuints
    public ReadOnlySpan<byte> Bytes {
        [MethodImpl(Utils.Inline)]
        get => new(Start, checked((int)(usize)InstSize));
    }
    
    public T this[nuint index] {
        [MethodImpl(Utils.Inline)]
        get {
            if (index >= Length)
                throw new InvalidIndexError($"Cannot access value at index {index} for slice of length {Length} (index should be < {Length})");
            nuint offs = index * T.ByteCount;
            return T.FromPtr(Start + offs);
        }
    }

    [MethodImpl(Utils.Inline)]
    public void ToPtr(byte* ptr) {
        for (nuint i = 0; i < InstSize; i += T.ByteCount) {
            byte* valStart = ptr + i;
            this[i].ToPtr(valStart);
        }
    }
    
    [MethodImpl(Utils.Inline)]
    public static Slice<T> FromFatPtr(byte* ptr, usize size) 
        => new(ptr, size);
    
    public nuint InstSize {
        [MethodImpl(Utils.Inline)]
        get => byteLength;
    }
    
    public bool IsConstSize {
        [MethodImpl(Utils.Inline)]
        get => false;
    }
    
    [MethodImpl(Utils.Inline)]
    public Enumerator GetEnumerator() => new Enumerator(Start, Length);
    
    [method: MethodImpl(Utils.Inline)]
    public struct Enumerator(byte* start, nuint length) {
        nuint _offset = nuint.MaxValue;

    [MethodImpl(Utils.Inline)]
        public bool MoveNext() => ++_offset < length;
        
        [MethodImpl(Utils.Inline)]
        public void Reset() {
            _offset = nuint.MaxValue;
        }

        public T Current {
            [MethodImpl(Utils.Inline)]
            get => T.FromPtr(start + _offset * T.ByteCount);
        }
    }
    
    [MethodImpl(Utils.Inline)]
    public static Slice<T> FromBytecode(byte* start, scoped ref nuint pc) {
        usize length = usize.FromBytecode(start, ref pc);
        usize byteLength = length * T.ByteCount;
        pc += (nuint)byteLength;
        return FromFatPtr(start + usize.ByteCount, byteLength);
    }
    
    [MethodImpl(Utils.Inline)]
    public byte[] ToBytecode() {
        usize fullByteLength = usize.ByteCount + byteLength;
        var data = new byte[fullByteLength];
        fixed (byte* ptr = &data[0])
            byteLength.ToPtr(ptr);
        Bytes.CopyTo(data.AsSpan()[(int)usize.ByteCount..]);
        return data;
    }
}