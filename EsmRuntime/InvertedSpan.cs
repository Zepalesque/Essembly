using System.Collections;
using System.Runtime.CompilerServices;

namespace EsmRuntime;

public readonly ref struct RevReadOnlySpan<T>(ReadOnlySpan<T> span) {
    readonly ReadOnlySpan<T> _span = span;

    public Enumerator GetEnumerator() => new(_span);
    
    public ref struct Enumerator : IEnumerator<T> {
        readonly ReadOnlySpan<T> _span;
        int _index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(ReadOnlySpan<T> span) {
            _span = span;
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() {
            int index = _index + 1;
            if (index >= _span.Length) return false;
            _index = index;
            return true;

        }

        public ref readonly T Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[^_index];
        }

        T IEnumerator<T>.Current => Current;

        object IEnumerator.Current => Current!;

        void IEnumerator.Reset() => _index = -1;

        void IDisposable.Dispose() { }
    }
}