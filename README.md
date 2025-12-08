
## Essembly - a funny stack-based "assemblish" language written in C#
This language is very simple, and currently operates on singular bytes and null-terminated strings (loaded to stack as ascii bytes)

#### Basic Hello World example
```
load const "Hello, World!"
print str;

load const 0;
exit;
```
More code examples can be found [here.](https://github.com/Zepalesque/Essembly/tree/master/EsmCompiler/input)

### constants
constants are loaded to the stack via a `load const [value];`
the value may any of:
- a binary integer (`0b0010_1101` for instance)
- a hex integer (`0x8D`)
- a decimal integer (`11`)
- an ascii character or escape sequence (`'e'`, `'\n'`, `'\0'`, etc...)
- a string (`"hi world :3"`, loaded into the stack with a `'\0'` appended to the end, the whole thing is in reverse order)


### operations
there are 5 binary operations and one unary.
these are all bitwise and operate with single-byte values
- `operate &;`: `stack += stack.Pop() & stack.Pop()` (AND)
- `operate |;`: `stack += stack.Pop() | stack.Pop()` (OR)
- `operate ^;`: `stack += stack.Pop() ^ stack.Pop()` (XOR)
- `operate <<;`: `stack += stack.Pop() << stack.Pop()` (LSHIFT)
- `operate >>;`: `stack += stack.Pop() >> stack.Pop()` (RSHIFT, *unsigned*)
- `operate ~;`: `stack += ~` (NOT)


### I/O
there is one form of input and one form of output.
- `input` statements await user input from the console.
- `print` statements push output to the console.

each require an I/O mode to be specified:
- `ascii`: inputs the user's next input as an ascii byte to the stack / pops the top of the stack and prints it formatted as an ascii character
- `dec`: inputs the next line of user next input as a decimal number to the stack / pops the top of the stack and prints it formatted as a decimal number
- `hex`: inputs the next line of user next input as a hexadecimal number to the stack / pops the top of the stack and prints it formatted as a hexadecimal number
- `bin`: inputs the next line of user next input as a binary number to the stack / pops the top of the stack and prints it formatted as a binary number
- `str`: inputs the next line of user next input as a string to the stack, with a `'\0'` terminator much like loading string constants / pops and prints the top of the stack iteratively until reaching `'\0'`, which is discarded


### variables
variables are essentially syntactical sugar for memory addresses
you can declare them via a let statement, such as:
```
let .e;
```
variable identifiers must start with a period (`.`).

these are untyped, single-byte slots in memory. you cannot have more than 256 due to the memory size (at the moment)
var declarations are entirely removed from the bytecode and all usages are replaced with indexed memory access.


### labels and goto statements
`#my_label`
any non -labeled statement can have a label
for instance:
```
#my_label: load const 11;
```


labels are used with `goto` statements:
- the unconditional `goto #label;` statement
- the conditional `goto #label if zero;`, jumps iff the top byte of the stack (which gets popped) is zero
- the conditional `goto #label if !zero;`, jumps iff the top byte of the stack (which gets popped) is NOT zero


### stuff id like to add
- multibyte integers (`load u32 const 0xFFFFFFFF;` for instance as a statement, and stuff like `operate i16 +;`
- arithmetic operators (would be easy lol)
- 32-bit memory locations
- pointers
- functions maybe
