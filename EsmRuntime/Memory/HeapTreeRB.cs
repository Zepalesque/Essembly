namespace EsmRuntime.Memory;

public unsafe ref partial struct HeapTree {

    static void RotLeft(ref HeapTree* self) {
        //  X (self)
        //    / \
        //   ?   Y
        //      / \
        //     B   ?
        
        // Assume self has a right value
        HeapTree* selfPrev = self; // X
        self = selfPrev->_right; // self = Y
        HeapTree* b = self->_left; // B = Y.left (from before)
        self->_parent = selfPrev->_parent; // Y.parent = X.parent
        selfPrev->_parent = self; // X.parent = Y
        if (b != null) b->_parent = selfPrev; // Ensure proper relinking before...
        selfPrev->_right = b; // ... changing X's right child to B
        
        
        Recalc(ref selfPrev); // Recalc(X)
        self->_left = selfPrev; // Y.left = X
        Recalc(ref self); // Recalc(Y)
        
        //  Y (self)
        //    / \
        //   X   ?
        //  / \
        // ?   B
    }

    static void RotRight(ref HeapTree* self) {
        //  Y (self)
        //    / \
        //   X   ?
        //  / \
        // ?   B
        HeapTree* selfPrev = self; // Y
        self = selfPrev->_left; // self = X
        HeapTree* b = self->_right; // B = X.right (from before)
        self->_parent = selfPrev->_parent; // X.parent = Y.parent
        selfPrev->_parent = self; // Y.parent = X
        if (b != null) b->_parent = selfPrev; // Ensure proper relinking before...
        selfPrev->_left = b; // ... changing Y's left child to B
        
        
                
        Recalc(ref selfPrev); // Recalc(Y)
        self->_right = selfPrev; // X.right = Y
        Recalc(ref self); // Recalc(X)
        
        //  X (self)
        //    / \
        //   ?   Y
        //      / \
        //     B   ?
    }
    
    
}