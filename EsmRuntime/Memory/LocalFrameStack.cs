using EsmRuntime.Common;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory;

public unsafe ref struct CallStack(byte* start, int length) {
    nint _offs = -1;

    nint NextAvailableOffset => _offs == -1 ? 0 : _offs + *(start + _offs);

    public Frame Curr { get; private set; } = default;
    
    public Frame Pop() {
        switch (_offs) {
            case -1: throw new StackUnderflowError("Cannot pop call stack as it is empty!");
            case 0: {
                Frame prev = Curr;
                prev.Clear();
                Curr = default;
                _offs = -1;
                return prev;
            }
            default: {
                Frame prev = Curr;
                prev.Clear();
                usize prevSize = usize.FromPtr(start + _offs - usize.ByteCount);
                _offs -= prevSize;
                Curr = Frame.FromPtr(start + _offs);
                return prev;
            }
        }
    }
    
    public void Push(byte* operand) {
        nint newOffs = NextAvailableOffset;
        byte* frameStart = start + newOffs;
        usize count = usize.FromPtr(operand);
        var varSizes = Slice<usize>.FromFatPtr(operand + usize.ByteCount, count * usize.ByteCount);
        var table = VariableTable.CreateFromSizes(varSizes, frameStart + usize.ByteCount,
            out usize fullSize);
        
        fullSize.ToPtr(frameStart);
        
        if (_offs + fullSize >= length)
            throw new StackOverflowError("Call stack is full!");

        for (nuint i = 0; i < count; i++) {
            usize offset = table[i];
            usize varSize = varSizes[i];
            
            varSize.ToPtr(frameStart + offset);
        }
        
        
        fullSize.ToPtr(frameStart + fullSize - usize.ByteCount);
        
        var frame = new Frame(frameStart, table);
        Curr = frame;
        _offs = NextAvailableOffset;
    }
}

public readonly unsafe ref struct VariableTable(Slice<usize> offsets): IByteSerializable<VariableTable> {
    readonly Slice<usize> _offsets = offsets;
    public usize this[nuint index] => _offsets[index];

    
    public static VariableTable CreateFromSizes(Slice<usize> sizes, byte* dest, out usize frameSize) {
        sizes.Length.ToPtr(dest);
        usize tableSize = usize.ByteCount + sizes.Length;
        usize partialFrameSize = tableSize;
        for (nuint i = 0; i < sizes.Length; i++) {
            usize size = sizes[i];
            byte* offsetLoc = dest + usize.ByteCount + i * usize.ByteCount;
            partialFrameSize.ToPtr(offsetLoc);
            usize partialOffs = size + usize.ByteCount;
            partialFrameSize += partialOffs;
        }

        frameSize = partialFrameSize + usize.ByteCount;

        return new(Slice<usize>.FromFatPtr(dest, sizes.Length));
    }

    
    public nuint InstSize => (_offsets.Length + 1) * usize.ByteCount;
    
    public void ToPtr(byte* ptr) {
        _offsets.Length.ToPtr(ptr);
        for (nuint i = 0; i < _offsets.Length; i ++)
            this[i].ToPtr(ptr + usize.ByteCount + i * usize.ByteCount);
    }

    public static VariableTable FromFatPtr(byte* ptr, usize size) {
        var slice = Slice<usize>.FromFatPtr(ptr, size);
        return new(slice);
    }

    public static VariableTable FromPtr(byte* ptr) {
        usize size = usize.FromPtr(ptr);
        var slice = Slice<usize>.FromFatPtr(ptr + usize.ByteCount, size * usize.ByteCount);
        return new(slice);
    }
}

public readonly unsafe ref struct Frame(byte* start, VariableTable table) {
    public usize Size => usize.FromPtr(start);

    VariableTable Table { get; } = table;

    public Span<byte> this[byte index] {
        get {
            VariableTable table = Table;
            usize offset = table[index];
            byte* ptr = start + offset;
            usize size = usize.FromPtr(ptr);
            return new(ptr + usize.ByteCount, size - usize.ByteCount);
        }
    }
    
    public static Frame FromPtr(byte* ptr) {
        VariableTable table = VariableTable.FromPtr(ptr + usize.ByteCount);
        return new(ptr, table);
    }

    public void Clear() {
        usize size = usize.FromPtr(start);
        var byteSpan = new Span<byte>(start, size);
        byteSpan.Clear();
    }
}