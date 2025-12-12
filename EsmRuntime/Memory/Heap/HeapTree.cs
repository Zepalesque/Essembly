using System.Runtime.InteropServices;
using EsmRuntime.Common.Types;

namespace EsmRuntime.Memory.Heap;

using static NativeMemory;

// TODO: Refactor or even rewrite as a RB Tree (red black)
public unsafe ref partial struct HeapTree(usize low, usize high, bool isRed) {

    bool _isRed = isRed;
    
    HeapTree* _left = null;
    usize _low = low, _high = high;
    HeapTree* _right = null;
    usize _maxInterval = high - low;
    
    usize _leftmostFree = low, _rightmostFree = high;
    HeapTree* _parent = null;

    static void Recalc(ref HeapTree* self) {
        if (self == null) return;
        self->_maxInterval = self->RecalculateMaxInterval();
        self->_leftmostFree = self->RecalculateLeftmost();
        self->_rightmostFree = self->RecalculateRightmost();

        if (self->_left != null) self->_left->_parent = self;
        if (self->_right != null) self->_right->_parent = self;
    }

    


    public static HeapTree* Create(usize from, usize to, bool isRed = true) {

        // no need for it to be zeroed, we overwrite it immediately anyway
        // surprised this method call doesnt need unsafe also
        void* ptr = Alloc((nuint) sizeof(HeapTree));

        HeapTree node = new(from, to, isRed);

        HeapTree* treePtr = (HeapTree*) ptr;
        
        *treePtr = node;

        return treePtr;
    }

    public static string DebugVisualize(ref HeapTree* self, int ansiCode = 0) {
        string colorCode = $"\e[0;9{ColorCode(ansiCode)}m";
        if (self == null) return $"{colorCode}{{}}\e[0m";

        ref HeapTree* selfLeft = ref self->_left;
        ref HeapTree* selfRight = ref self->_right;
        
        string left = selfLeft == null ? "" : "\e[0m" + DebugVisualize(ref selfLeft, ansiCode + 1) + $"{colorCode}, ";
        string right = selfRight == null ? "" : ", \e[0m" + DebugVisualize(ref selfRight, ansiCode + 1);
        return $"{colorCode}::{{ {left}[{self->_low}, {self->_high}){right}{colorCode} }}\e[0m";


        int ColorCode(int i) {
            return (i % 5) switch {
                0 => 6,
                1 => 5,
                2 => 7,
                3 => 5,
                4 => 6,
                _ => throw new InvalidOperationException()
            };
        }
    }
}

