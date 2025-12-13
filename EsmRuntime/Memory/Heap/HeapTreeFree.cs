using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;
using static NativeMemory;

public unsafe ref partial struct HeapTree {

    public static bool TryFree(ref HeapTree* self, usize from, usize to)
        => FreeImpl(ref self, ref self, from, to/*, out _*/);

    static bool FreeImpl(ref HeapTree* self, ref HeapTree* root, usize from, usize to/*, out HeapTree* inserted*/) {
        if (self == null) {
            self = Create(from, to);
            // inserted = self;
            return true;
        }
        
        if (to == self->_low) {
            self->_low = from;
            TryMergeLeft(ref self);
            Recalc(ref self);
            return true;
        }
        
        if (from == self->_high) {
            self->_high = to;
            TryMergeRight(ref self);
            Recalc(ref self);
            return true;
        }
        
        if (to < self->_low) {
            bool result = FreeImpl(ref self->_left, ref root, from, to);

            if (!result) return result;
            TryMergeLeft(ref self);
            Recalc(ref self);
            return result;
        }

        if (from > self->_high) {
            bool result = FreeImpl(ref self->_right, ref root, from, to);

            if (!result) return result;
            TryMergeRight(ref self);
            Recalc(ref self);
            return result;
        }

        return false;
    }
    
    static void TryMergeLeft(ref HeapTree* self) {
        if (self->_left == null) return;
        ref var pre = ref RightMostChild(ref self->_left);
        if (pre->_high == self->_low) {
            self->_low = pre->_low;
            
            var preLeft = pre->_left; 
            var preParent = pre->_parent;

            AlignedFree(pre);
        
            pre = preLeft;
        
            if (pre != null) pre->_parent = preParent;
        }
    }
   
    static void TryMergeRight(ref HeapTree* self) {
        if (self->_right == null) return;

        ref var succ = ref LeftMostChild(ref self->_right);

        if (self->_high == succ->_low) {
            self->_high = succ->_high;

            var succRight = succ->_right;
            var succParent = succ->_parent;

            Free(succ);
        
            succ = succRight;
        
            if (succ != null) succ->_parent = succParent;
        }
    }
}