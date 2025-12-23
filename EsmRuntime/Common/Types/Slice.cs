using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EsmRuntime.Common.Types;

public readonly unsafe ref struct Slice<T>(byte* start, usize byteLength): IBytecodeSerializable<Slice<T>> where T: struct, ISizedValue<T>, allows ref struct {
    byte* Start { get; } = start;
    public usize Length { get; } = byteLength / T.ByteCount;
    
    public static Slice<T> Create(ReadOnlySpan<byte> span) {
        ref byte reference = ref MemoryMarshal.GetReference(span);
        var ptr = (byte*)Unsafe.AsPointer(ref reference);
        return new(ptr, span.Length);
    }
    
    public ReadOnlySpan<byte> Bytes => new ReadOnlySpan<byte>(Start, (usize) InstSize);

    public T this[nuint index] {
        get {
            if (index >= Length)
                throw new InvalidIndexError($"Cannot access value at index {index} for slice of length {Length} (index should be < {Length})");
            nuint offs = index * T.ByteCount;
            return T.FromPtr(Start + offs);
        }
    }

    public void ToPtr(byte* ptr) {
        for (nuint i = 0; i < InstSize; i += T.ByteCount) {
            byte* start = ptr + i;
            this[i].ToPtr(start);
        }
    }
    
    public static Slice<T> FromFatPtr(byte* ptr, usize size) 
        => new(ptr, size);
    
    
    public nuint InstSize { get; } = byteLength;
    
    public Enumerator GetEnumerator() => new Enumerator(Start, Length);
    
    public struct Enumerator(byte* start, nuint length) {
        nuint _offset = nuint.MaxValue;

        public bool MoveNext() => ++_offset < length;

        public void Reset() {
            _offset = nuint.MaxValue;
        }

        public T Current => T.FromPtr(start + _offset * T.ByteCount);
    }
    
    public static Slice<T> FromBytecode(byte* start, int* pc) {
        usize length = usize.FromBytecode(start, pc);
        usize byteLength = length * T.ByteCount;
        *pc += (int) byteLength;
        return FromFatPtr(start + usize.ByteCount, byteLength);
    }
    
    public byte[] ToBytecode() {
        usize byteLength = InstSize;
        usize fullByteLength = usize.ByteCount + byteLength;
        var data = new byte[fullByteLength];
        fixed (byte* ptr = &data[0])
            byteLength.ToPtr(ptr);
        Bytes.CopyTo(data.AsSpan()[(int)usize.ByteCount..]);
        return data;
    }
}

