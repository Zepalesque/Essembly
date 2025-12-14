using System.Collections;

namespace EsmRuntime.Common.Types;

public readonly unsafe ref struct Slice<T>(byte* start, usize length): IByteSerializable<Slice<T>> where T: struct, ISizedValue<T>, allows ref struct {
    byte* Start { get; } = start;
    public usize Length { get; } = length;
    
    public ReadOnlySpan<byte> Bytes => new(Start, Length);

    public T this[nuint index] {
        get {
            nuint offs = index * T.ByteCount;
            return T.FromPtr(Start + offs);
        }
    }

    public void ToPtr(byte* ptr) {
        for (nuint i = 0; i < Length; i++) {
            byte* varSpan = ptr + i;
            this[i].ToPtr(varSpan);
        }
    }
    
    public static Slice<T> FromFatPtr(byte* ptr, usize size) 
        => new(ptr, size / T.ByteCount);
    
    
    public nuint InstanceSize => Length * T.ByteCount;

    public Enumerator GetEnumerator() => new(Start, Length);
    
    public struct Enumerator(byte* start, nuint length) {
        nuint _offset = 0;

        public bool MoveNext() => _offset++ < length;

        public void Reset() {
            _offset = 0;
        }

        public T Current => T.FromPtr(start + _offset);
        
    }
}

