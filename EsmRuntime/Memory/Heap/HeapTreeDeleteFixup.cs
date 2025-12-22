namespace EsmRuntime.Memory.Heap;

public unsafe ref partial struct HeapTree {
    
    // hell.
    static void DeleteFixup(ref HeapTree* root, HeapTree* curr, HeapTree* parent) {
        while (curr != root && (curr == null || !curr->_isRed)) {
            if (curr == parent->_left) {
                HeapTree* aunt = parent->_right;
                
                // case 1: aunt is red 
                if (IsRed(aunt)) {
                    aunt->_isRed = false;
                    parent->_isRed = true;
                    RotLeft(parent, ref root);
                    aunt = parent->_right;
                }
                
                // case 2: aunt is black, both 'cousins' are too
                if (IsBlack(aunt->_left) && IsBlack(aunt->_right)) {
                    aunt->_isRed = true;
                    curr = parent;
                    parent = ParentOrNull(curr);
                } else {
                    // case 3: 'far cousin' is black
                    if (IsBlack(aunt->_right)) {
                        if (aunt->_left != null) aunt->_left->_isRed = false;
                        aunt->_isRed = true;
                        RotRight(ref parent->_right);
                        aunt = parent->_right;
                    }
                    
                    // case 4: 'far cousin' is red
                    aunt->_isRed = parent->_isRed;
                    parent->_isRed = false;
                    if (aunt->_right != null) aunt->_right->_isRed = false;
                    RotLeft(parent, ref root);
                    curr = root; // resolved :3
                }
            } else {
                // mirror of it
                
                
                HeapTree* aunt = parent->_left;
                
                // case 1: aunt is red 
                if (IsRed(aunt)) {
                    aunt->_isRed = false;
                    parent->_isRed = true;
                    RotRight(parent, ref root);
                    aunt = parent->_left;
                }
                
                // case 2: aunt is black, both 'cousins' are too
                if (IsBlack(aunt->_right) && IsBlack(aunt->_left)) {
                    aunt->_isRed = true;
                    curr = parent;
                    parent = ParentOrNull(curr);
                } else {
                    // case 3: 'far cousin' is black
                    if (IsBlack(aunt->_left)) {
                        if (aunt->_right != null) aunt->_right->_isRed = false;
                        aunt->_isRed = true;
                        RotLeft(ref parent->_left);
                        aunt = parent->_left;
                    }
                    
                    // case 4: 'far cousin' is red
                    aunt->_isRed = parent->_isRed;
                    parent->_isRed = false;
                    if (aunt->_left != null) aunt->_left->_isRed = false;
                    RotRight(parent, ref root);
                    curr = root; // resolved :3
                }

            }
            
        }
        
        if (curr != null) curr->_isRed = false;
    }
}