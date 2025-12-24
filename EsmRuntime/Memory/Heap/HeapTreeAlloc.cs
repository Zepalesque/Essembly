using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;
using static NativeMemory;

public unsafe ref partial struct HeapTree {

    public static bool TryAllocate(ref HeapTree* self, nuint size, out byte* start) 
        => AllocImpl(ref self, ref self, size, out start);

    // Me when i binary search the binary tree (i hate this)
    static bool AllocImpl(ref HeapTree* self, ref HeapTree* root, nuint size, out byte* start) {
        if (self == null) {
            start = null;
            return false;
        }
        
        nuint max = self->_maxInterval;
        if (size > max) {
            start = null;
            return false;
        }

        
        // perfect match case
        if (max == size) {
            if (self->_high - self->_low == size) {
                start = (byte*) self -> _low;
                
                bool wasBlack = !self->_isRed;
                HeapTree* parent = self->_parent;
                
                // 0 or 1 child(ren)
                if (self->_left == null || self->_right == null) {
                    HeapTree* n = self->_left != null ? self->_left : self->_right;
                    AlignedFree(self);
                    self = n;
                    if (self != null) self->_parent = parent;
                    if (wasBlack) DeleteFixup(ref root, self, parent);
                } else { // 2 children
                    ref HeapTree* succRef = ref LeftMostChild(ref self->_right);
                    HeapTree* succ = succRef;
                    
                    self->_low = succ->_low;
                    self->_high = succ->_high;
                    
                    bool succWasBlack = !succ->_isRed;
                    var succParent = succ->_parent;
                    var succRepl = succ->_right;
                    
                    AlignedFree(succ);
                    
                    succRef = succRepl;
                    if (succRef != null) succRef->_parent = succParent;
                    if (succWasBlack) DeleteFixup(ref root, succRef, succParent);
                    
                    RecalcUp(succParent, self);
                }
                
                Recalc(ref self);

                return true;
            }
        }

        // le big blocc
        if (self -> _high - self -> _low > size) {
            start = (byte*) self -> _low;
            self -> _low += size;

            Recalc(ref self);
            return true;
        }

        ref var left = ref self -> _left;
        if (left != null && left->_maxInterval >= size) {
            bool ret = AllocImpl(ref left, ref root, size, out start);
            Recalc(ref self);

            return ret;
        }

        ref var right = ref self -> _right;
        if (right != null && right->_maxInterval >= size) {
            bool ret = AllocImpl(ref right, ref root, size, out start);
            Recalc(ref self);

            return ret;
        }

        start = null;
        return false;
    }
    
    nuint RecalculateMaxInterval() {
        var left = _left;
        var right = _right;          
        nuint selfSize = _high - _low;
        nuint leftSize = left == null ? 0 : left->_maxInterval;
        nuint rightSize = right == null ? 0 : right->_maxInterval;
        nuint maxLr = leftSize > rightSize ? leftSize : rightSize;
        return selfSize > maxLr ? selfSize : maxLr;
    }

    nuint RecalculateLeftmost() {
        var left = _left;
        return left != null ? _left->_leftmostEnd : _leftmostEnd;
    }


    nuint RecalculateRightmost() {
        var right = _right;
        return right != null ? _right->_rightmostEnd : _rightmostEnd;
    }
    

    static ref HeapTree* LeftMostChild(ref HeapTree* self) {
        if (self->_left == null) return ref self;
        return ref LeftMostChild(ref self->_left);
    }
    
    
    static ref HeapTree* RightMostChild(ref HeapTree* self) {
        if (self->_right == null) return ref self;
        return ref RightMostChild(ref self->_right);
    }
}