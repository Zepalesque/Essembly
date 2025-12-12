namespace EsmRuntime.Memory.Heap;

public unsafe ref partial struct HeapTree {
    
    // Fix for inserting a value
    static void InsertFixup(ref HeapTree* root, HeapTree* inserted) {
        HeapTree* curr = inserted;

        while (curr->_parent != null && curr->_parent->_isRed) {
            HeapTree* parent = curr->_parent;
            // By red-black tree rules, given that a node is red, it MUST have a parent
            HeapTree* grand = parent->_parent;

            if (parent == grand->_left) {
                HeapTree* aunt = grand->_right;
                
                // check if aunt is Red (null nodes are black, remember, ugh this is complex)
                if (aunt != null && aunt->_isRed) {
                    parent->_isRed = false;
                    aunt->_isRed = false;
                    grand->_isRed = true;
                    curr = grand;
                } else {
                    if (curr == parent->_right) {
                        RotLeft(ref parent->_right);
                        curr = parent;
                        parent = curr->_parent;
                        grand = parent->_parent;
                    }
                    
                    parent->_isRed = false;
                    grand->_isRed = true;

                    if (grand->_parent == null) RotRight(ref root);
                    else if (grand == grand->_parent->_left)
                         RotRight(ref grand->_parent->_left); // lol 5-space indent but i do love some nice alignment
                    else RotRight(ref grand->_parent->_right);
                }
            } else {
                HeapTree* aunt = grand->_left;
                
                if (aunt != null && aunt->_isRed) {
                    parent->_isRed = false;
                    aunt->_isRed = false;
                    grand->_isRed = true;
                    curr = grand;
                } else {
                    if (curr == parent->_left) {
                        RotRight(ref parent->_left);
                        curr = parent;
                        parent = curr->_parent;
                        grand = parent->_parent;
                    }
                    
                    parent->_isRed = false;
                    grand->_isRed = true;

                    if (grand->_parent == null) RotLeft(ref root);
                    else if (grand == grand->_parent->_right)
                        RotLeft(ref grand->_parent->_right); // lol 5-space indent but i do love some nice alignment
                    else RotLeft(ref grand->_parent->_left);
                }
                
            }
        }

        root->_isRed = false;
    }
}