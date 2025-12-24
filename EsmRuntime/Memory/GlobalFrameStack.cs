using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory;

// TODO: Non-top level var access, preferably O(log n)s
//  Perhaps even O(1) if offsets from current frame are computed at compile time - Store offset table, frames can go at the end maybe
public unsafe ref struct GlobalStack(byte* start, int length) {
    nint _offs = -1;

    nint NextAvailableOffset => _offs == -1 ? 0 : _offs + *(start + _offs);

    public Frame Curr { get; private set; } = default;
    
    public Frame Pop() {
        switch (_offs) {
            case -1: throw new StackUnderflowError("Cannot pop global stack as it is empty!");
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

