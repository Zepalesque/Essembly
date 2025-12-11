using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory;

using static NativeMemory;

public unsafe ref struct IntervalNode(usize low, usize high) {
    IntervalNode* _left = null;
    usize _low = low, _high = high;
    IntervalNode* _right = null;
    usize _maxInterval = high - low;
    usize _leftmostFree = low, _rightmostFree = high;
    IntervalNode* _parent = null;
    
    internal static bool TryFree(ref IntervalNode* self, usize from, usize to) {
        if (self == null) {
            self = Create(from, to);
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
            bool result = TryFree(ref self->_left, from, to);

            if (!result) return result;
            TryMergeLeft(ref self);
            Recalc(ref self);
            return result;
        }

        if (from > self->_high) {
            bool result = TryFree(ref self->_right, from, to);

            if (!result) return result;
            TryMergeRight(ref self);
            Recalc(ref self);
            return result;
        }

        return false;
    }
    
    static void TryMergeLeft(ref IntervalNode* self) {
        if (self->_left == null) return;
        ref var pre = ref RightMostChild(ref self->_left);
        if (pre->_high == self->_low) {
            self->_low = pre->_low;
            
            var preLeft = pre->_left; 
            var preParent = pre->_parent;

            Free(pre);
        
            pre = preLeft;
        
            if (pre != null) pre->_parent = preParent;
        }
    }
   
    static void TryMergeRight(ref IntervalNode* self) {
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

    // Me when i binary search the binary tree (i hate this)
    internal static bool TryAllocate(ref IntervalNode* self, usize size, out byte* start) {
        usize max = self->_maxInterval;
        if (size > max) {
            start = null;
            return false;
        }

        if (max == size) {
            if (self->_high - self->_low == size) {
                start = (byte*) self -> _low;
                if (self->_left == null && self->_right == null) {
                    Free(self);
                    self = null;
                }
                else if (self->_left != null && self->_right == null) {
                    var l = self->_left; 
                    Free(self); 
                    self = l; 
                }
                else if (self->_right != null && self->_left == null) {
                    var r = self->_right; 
                    Free(self); 
                    self = r;
                } else {
                    ref var leftmost = ref LeftMostChild(ref self->_right);
                    var leftmostVal = leftmost;
                    
                    var leftmostParent = leftmostVal->_parent; 
                    
                    self->_low = leftmostVal->_low;
                    self->_high = leftmostVal->_high;
                    
                    leftmost = leftmostVal->_right;
                    if (leftmost != null) leftmost->_parent = leftmostParent;
                    
                    Free(leftmostVal);
                    
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

            bool b = TryAllocate(ref self->_left, size, out start);

            Recalc(ref self);

            return b;
        }

        if (right != null && right->_maxInterval >= size) {
            bool b = TryAllocate(ref self->_right, size, out start);
            Recalc(ref self);


            return b;
        }

        start = null;

        return false;
    }


    static void Recalc(ref IntervalNode* self) {
        if (self == null) return;
        self->_maxInterval = self->RecalculateMaxInterval();
        self->_leftmostFree = self->RecalculateLeftmost();
        self->_rightmostFree = self->RecalculateRightmost();

        if (self->_left != null) self->_left->_parent = self;
        if (self->_right != null) self->_right->_parent = self;
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
    

    static ref IntervalNode* LeftMostChild(ref IntervalNode* self) {
        if (self->_left == null) return ref self;
        return ref LeftMostChild(ref self->_left);
    }
    
    
    static ref IntervalNode* RightMostChild(ref IntervalNode* self) {
        if (self->_right == null) return ref self;
        return ref RightMostChild(ref self->_right);
    }


    public static IntervalNode* Create(usize from, usize to) {

        // no need for it to be zeroed, we overwrite it immediately anyway
        // surprised this method call doesnt need unsafe also
        void* ptr = Alloc((nuint) sizeof(IntervalNode));

        IntervalNode node = new(from, to);

        IntervalNode* treePtr = (IntervalNode*) ptr;
        
        *treePtr = node;

        return treePtr;
    }
}