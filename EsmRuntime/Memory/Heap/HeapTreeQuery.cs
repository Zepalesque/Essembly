using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;

public unsafe ref partial struct HeapTree {
    
    
    public static bool IsFree(ref HeapTree* self, usize start, usize end) {
        if (self == null) return true;

        if (self->_low <= start && self->_high >= end)
            return true;

        if (self->_leftmostFree > end || self->_rightmostFree < start)
            return false;

        bool freeInLeft = self->_left != null && IsFree(ref self->_left, start, end);
        if (freeInLeft) return true;

        bool freeInRight = self->_right != null && IsFree(ref self->_right, start, end);
        if (freeInRight) return true;

        return false;
    }
}