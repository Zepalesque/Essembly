namespace EsmRuntime;

public static class LinqUtil {

    // direct array to array conversions
    extension<T>(IEnumerable<T> self) {
        
        public TRes[] Map<TRes>(Func<T, TRes> mapper) => self.Select(mapper).ToArray();
        public TRes[] FlatMap<TRes>(Func<T, TRes[]> mapper) => self.SelectMany(mapper).ToArray();
        public T[] Filter(Func<T, bool> predicate) => self.Where(predicate).ToArray();
    }

    extension<T>(T? self) where T : struct {
        public TRes? MapNull<TRes>(Func<T, TRes> mapper)
            => self == null ? default : mapper(self.Value);
    }
    
    extension<T>(T? self) where T : class {
        public TRes? MapNull<TRes>(Func<T, TRes> mapper)
            => self == null ? default : mapper(self);
    }
}