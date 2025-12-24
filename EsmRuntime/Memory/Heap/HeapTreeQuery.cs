using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;

public unsafe ref partial struct HeapTree {
    
    
    public static bool IsFree(ref HeapTree* self, nuint start, nuint end) {
        if (self == null) return true;

        if (self->_low <= start && self->_high >= end)
            return true;

        if (self->_leftmostEnd > end || self->_rightmostEnd < start)
            return false;

        bool freeInLeft = self->_left != null && IsFree(ref self->_left, start, end);
        if (freeInLeft) return true;

        bool freeInRight = self->_right != null && IsFree(ref self->_right, start, end);
        return freeInRight;
    }
}