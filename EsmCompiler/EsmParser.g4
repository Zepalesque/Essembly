parser grammar EsmParser;
options { tokenVocab=EsmLexer; language=CSharp; }

base: statement* EOF;

statement
    : stmt = baseStmt
    | label = LabelId ':' stmt = baseStmt
;

baseStmt
    : Op specificInt opMode? op = stackOp ';' #operationPerform
    | Promote arbitraryInt '->' specificInt ';' #promotion
    | Demote arbitraryInt '->' arbitraryInt ';' #demotion
    | Alloc id = Identifier ':' type = instanceType ('=' anyLiteral)? ';' #allocMem
    | Let id = Identifier ':' type = instanceType ';' #localVar
    | Free id = Identifier ';' #freeMem
    | Push push = pushClause ';' #toStack
    | Store id = Identifier ';' #storeToPointer
    | Print io = ioMode ';' #print
    | Goto (label = LabelId) (If condition = ifCond)? ';' #goto
    | Exit ';' #exit
;

// IO
ioMode: io = Ascii| io = Utf16 | ('&' io = Str) |
    io = U8|io = U16|io = U32|io = U64|io = I8|io = I16|io = I32|io = I64
;




opMode: Binary | Unary;


stackOp
    : op = '&'
    | op = '|'
    | op = '~'
    | op = '^'
    | op = '<<'
    | op = '>>'
    | op = '+>>'
    | op = '+'
    | op = '-'
    | op = '*'
    | op = '/'
    | op = '%'
;

pushClause
    : type = specificInt lit = sizedLiteral #pushConst
    | loc = /*'@' */Identifier #pushMem
    | Input io = ioMode #pushInput
;

ifCond: not = Bang? Zero;


specificInt: type = U8|type = U16|type = U32|type = U64|type = I8|type = I16|type = I32|type = I64;
arbitraryInt: type = X8|type = X16|type = X32|type = X64;

instanceType: type = U8|type = U16|type = U32|type = U64|type = I8|type = I16|type = I32|type = I64
    | ('&' type = Str)
;



sizedLiteral: int = DecIntLiteral | int = HexIntLiteral | int = BinIntLiteral | int = OctIntLiteral | char = CharLiteral;
anyLiteral: int = DecIntLiteral | int = HexIntLiteral | int = BinIntLiteral | int = OctIntLiteral | char = CharLiteral | str = StrLiteral;
