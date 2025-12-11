using EsmRuntime;
using EsmRuntime.Common.Types;
using EsmRuntime.Memory;
using static System.Console;
using static System.Runtime.InteropServices.NativeMemory;

namespace LowLevelTests;

class Program {
    static unsafe void Main(string[] args) {
        WriteLine("Hi world lol");
        WriteLine("Memory tree test time");
        WriteLine();
        
        int size;
        
        do {
            WriteLine("Please input a size, in bytes.");
            WriteLine(
                "These will be actually allocated AND visualized via print, so don't choose too high a number.");
            
        } while (!int.TryParse(ReadLine(), out size));
        

        var alloc = (byte*) AllocZeroed((nuint) size);
        EsmVM.MemStart = alloc;
        
        MemoryTree* tree = MemoryTree.Create(0, size);
        
        bool exit = false;

        while (!exit) {
            PerformTreeOperation(ref exit, ref tree);
        }
    }

    static unsafe void PerformTreeOperation(ref bool exit, ref MemoryTree* tree) {
        
        WriteLine();
        WriteLine("--OPTIONS--");
        WriteLine("  * a: Allocate a range of memory");
        WriteLine("  * f: Free a range of memory");
        WriteLine("  * q: Query a range");
        WriteLine("  * v: Visualize the tree");
        WriteLine("  * e: Exit");
        WriteLine();

        char c = ReadKey().KeyChar;

        switch (c) {
            case 'a': PerformAllocation(ref tree); break;
            case 'f': PerformFree(ref tree); break;
            // case 'q': PerformQuery(ref tree); break;
            case 'v': Visualize(ref tree); break;
            case 'e': exit = true; break;
            default: WriteLine($"Invalid option: {c}"); break;
        }
    }

    static unsafe void PerformFree(ref MemoryTree* tree) {
        int start, end;
                
        do {
            WriteLine("Please input the start to the interval to free.");
        } while (!int.TryParse(ReadLine(), out start));

        
        do {
            WriteLine("Please input the end to the interval to free.");
        } while (!int.TryParse(ReadLine(), out end));
        
        WriteLine($"Attempting to free the range [{start}, {end})...");
        
        WriteLine(MemoryTree.TryFree(ref tree, start, end)
            ? $"Success! Freed [{start}, {end}) from memory"
            : "Failure, could not free the range. Perhaps it was already free?");
    }

    static unsafe void PerformAllocation(ref MemoryTree* tree) {
        int size;
        
        do {
            WriteLine("Please input an allocation size.");
        } while (!int.TryParse(ReadLine(), out size));
        
        WriteLine($"Attempting to allocate {size} bytes...");

        WriteLine(MemoryTree.TryAllocate(ref tree, size, out byte* start)
            ? $"Success! Allocated starting at &{(usize) start}"
            : "Failure, could not allocate.");
    }
    
    static unsafe void Visualize(ref MemoryTree* tree) {
        WriteLine("Visualizing tree...");
        WriteLine();
        WriteLine(MemoryTree.DebugVisualize(ref tree));
        
    }
}