using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;
using static NativeMemory;

public unsafe ref partial struct HeapTree {

    public static bool TryAllocate(ref HeapTree* self, usize size, out byte* start) 
        => AllocImpl(ref self, ref self, size, out start);

    // Me when i binary search the binary tree (i hate this)
    static bool AllocImpl(ref HeapTree* self, ref HeapTree* root, usize size, out byte* start) {
        if (self == null) {
            start = null;
            return false;
        }
        
        usize max = self->_maxInterval;
        if (size > max) {
            start = null;
            return false;
        }

        if (max == size) {
            if (self->_high - self->_low == size) {
                start = (byte*) self -> _low;
                if (self->_left == null && self->_right == null) {
                    AlignedFree(self);
                    self = null;
                }
                else if (self->_left != null && self->_right == null) {
                    var l = self->_left; 
                    AlignedFree(self); 
                    self = l; 
                }
                else if (self->_right != null && self->_left == null) {
                    var r = self->_right; 
                    AlignedFree(self); 
                    self = r;
                } else {
                    ref var leftmost = ref LeftMostChild(ref self->_right);
                    var leftmostVal = leftmost;
                    
                    var leftmostParent = leftmostVal->_parent; 
                    
                    self->_low = leftmostVal->_low;
                    self->_high = leftmostVal->_high;
                    
                    leftmost = leftmostVal->_right;
                    if (leftmost != null) leftmost->_parent = leftmostParent;
                    
                    AlignedFree(leftmostVal);
                    
                    var traversal = leftmostParent;
                    while (traversal != null && traversal != self) {
                        traversal->_maxInterval = traversal->RecalculateMaxInterval();
                        traversal->_leftmostFree = traversal->RecalculateLeftmost();
                        traversal->_rightmostFree = traversal->RecalculateRightmost();
                        traversal = traversal->_parent;
                    }
                }
                
                            
                Recalc(ref self);

                return true;
            }
        }

        if (self -> _high - self -> _low > size) {
            
            
            start = (byte*) self -> _low;
            self -> _low += size;

            Recalc(ref self);

            return true;
        }

        var left = self -> _left;
        var right = self -> _right;
        if (left != null && left->_maxInterval >= size) {

            bool b = AllocImpl(ref self->_left, ref root, size, out start);

            Recalc(ref self);

            return b;
        }

        if (right != null && right->_maxInterval >= size) {
            bool b = AllocImpl(ref self->_right, ref root, size, out start);
            Recalc(ref self);


            return b;
        }

        start = null;

        return false;
    }
    
    usize RecalculateMaxInterval() {
        var left = _left;
        var right = _right;          
        usize selfSize = _high - _low;
        usize leftSize = left == null ? 0 : left->_maxInterval;
        usize rightSize = right == null ? 0 : right->_maxInterval;
        usize maxLr = leftSize > rightSize ? leftSize : rightSize;
        return selfSize > maxLr ? selfSize : maxLr;
    }

    usize RecalculateLeftmost() {
        var left = _left;
        return left != null ? _left->_leftmostFree : _leftmostFree;
    }


    usize RecalculateRightmost() {
        var right = _right;
        return right != null ? _right->_rightmostFree : _rightmostFree;
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