using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;
using static NativeMemory;

public unsafe ref partial struct HeapTree {

    public static bool TryFree(ref HeapTree* self, nuint from, nuint to)
        => FreeImpl(ref self, null, ref self, from, to/*, out _*/);

    static bool FreeImpl(ref HeapTree* self, HeapTree* parent, ref HeapTree* root, nuint from, nuint to/*, out HeapTree* inserted*/) {
        if (self == null) {
            self = Create(from, to);
            self->_parent = parent;
            
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
            bool result = FreeImpl(ref self->_left, self, ref root, from, to);

            if (!result) return result;
            TryMergeLeft(ref self, ref root);
            TryMergeRight(ref self, ref root);
            Recalc(ref self);
            return result;
        }

        if (from > self->_high) {
            bool result = FreeImpl(ref self->_right, self, ref root, from, to);

            if (!result) return result;
            TryMergeRight(ref self, ref root);
            TryMergeLeft(ref self, ref root);
            Recalc(ref self);
            return result;
        }

        return false;
    }
    
    static void TryMergeLeft(ref HeapTree* self, ref HeapTree* root) {
        if (self == null || self->_left == null) return;
        ref HeapTree* node = ref RightMostChild(ref self->_left); // predecessor
        
        if (node->_high == self->_low) {
            self->_low = node->_low;
            
            bool wasBlack = !node->_isRed;
            HeapTree* left = node->_left; 
            HeapTree* parent = node->_parent;

            AlignedFree(node);
        
            node = left;
            if (node != null) node->_parent = parent;
            
            if (wasBlack) DeleteFixup(ref root, node, parent);
        }
    }
   
    static void TryMergeRight(ref HeapTree* self, ref HeapTree* root) {
        if (self == null || self->_right == null) return;

        ref HeapTree* node = ref LeftMostChild(ref self->_right); // successor

        if (self->_high == node->_low) {
            self->_high = node->_high;
            
            bool wasBlack = !node->_isRed;
            HeapTree* right = node->_right;
            HeapTree* parent = node->_parent;

            AlignedFree(node);
        
            node = right;
        
            if (node != null) node->_parent = parent;
            
            if (wasBlack) DeleteFixup(ref root, node, parent);
        }
    }
}