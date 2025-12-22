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
            InsertFixup(ref root, self);
            return true;
        }
        
        if (to == self->_low) {
            self->_low = from;
            TryMergeLeft(ref self, ref root);
            TryMergeRight(ref self, ref root);
            Recalc(ref self);
            return true;
        }
        
        if (from == self->_high) {
            self->_high = to;
            TryMergeRight(ref self, ref root);
            TryMergeLeft(ref self, ref root);
            Recalc(ref self);
            return true;
        }
        
        if (to < self->_low) {
            bool result = FreeImpl(ref self->_left, ref root, from, to);

            if (!result) return result;
            TryMergeLeft(ref self, ref root);
            TryMergeRight(ref self, ref root);
            Recalc(ref self);
            return result;
        }

        if (from > self->_high) {
            bool result = FreeImpl(ref self->_right, ref root, from, to);

            if (!result) return result;
            TryMergeRight(ref self, ref root);
            TryMergeLeft(ref self, ref root);
            Recalc(ref self);
            return result;
        }

        return false;
    }
    
    static void TryMergeLeft(ref HeapTree* self, ref HeapTree* root) {
        if (self->_left == null) return;
        ref var node = ref RightMostChild(ref self->_left); // predecessor
        
        if (node->_high == self->_low) {
            self->_low = node->_low;
            
            bool wasBlack = !node->_isRed;
            var left = node->_left; 
            var parent = node->_parent;

            AlignedFree(node);
        
            node = left;
            if (node != null) node->_parent = parent;
            
            if (wasBlack) DeleteFixup(ref root, node, parent);
        }
    }
   
    static void TryMergeRight(ref HeapTree* self, ref HeapTree* root) {
        if (self->_right == null) return;

        ref var node = ref LeftMostChild(ref self->_right); // successor

        if (self->_high == node->_low) {
            self->_high = node->_high;
            
            bool wasBlack = !node->_isRed;
            var right = node->_right;
            var parent = node->_parent;

            AlignedFree(node);
        
            node = right;
        
            if (node != null) node->_parent = parent;
            
            if (wasBlack) DeleteFixup(ref root, node, parent);
        }
    }
}