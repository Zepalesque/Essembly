parser grammar EsmParser;
options { tokenVocab=EsmLexer; language=CSharp; }

base: blockInner? EOF;

block: '{' blockInner? '}';
blockInner: statement+;

statement
    : stmt = baseStmt
    | label = LabelId ':' stmt = baseStmt
    | frame = block
;

baseStmt
    : Op intType op = stackOp ';' #operationPerform
    | Cast intType '->' intType ';' #castOperation
    | Alloc id = Identifier ':' type = instanceType ('=' ptrAndMaybeRefPrefix? anyLiteral)? ';' #allocMem
    | Let id = Identifier ':' type = instanceType ';' #localVar
    | Free id = Identifier ';' #freeMem
    | Push push = pushClause ';' #toStack
    | Store id = Identifier ';' #storeToPointer
    | Print io = ioMode ';' #print
    | Goto (label = LabelId) (If condition = ifCond)? ';' #goto
    | Exit ';' #exit
;

// IO
ioMode: io = Utf8| io = Utf16 | ('&' io = Str) |
    io = U8|io = U16|io = U32|io = U64|io = I8|io = I16|io = I32|io = I64|io = USize|io = ISize
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
    | opMode ambigOp
    | op = '*'
    | op = '/'
    | op = '%'
    | op = '=='
    | op = '!='
    | op = '!'
;

ambigOp
    : op = '+'
    | op = '-'
;
pushClause
    : type = intType lit = sizedLiteral #pushConst
    | loc = Identifier #pushMem
;

ifCond: value = (True|False);


intType: type = U8|type = U16|type = U32|type = U64|type = I8|type = I16|type = I32|type = I64;

instanceType
    : ptrAndMaybeRefPrefix? baseType
    | ptrAndMaybeRefPrefix Raw
;

ptrAndMaybeRefPrefix: refPrefix* ptrPrefix+;

refPrefix: '&' (Mut)?;
ptrPrefix: '*' (Mut)?;

baseType
    : int = intType
    | type = Str
;


sizedLiteral: int = DecIntLiteral | int = HexIntLiteral | int = BinIntLiteral | int = OctIntLiteral | char = CharLiteral;
anyLiteral: int = DecIntLiteral | int = HexIntLiteral | int = BinIntLiteral | int = OctIntLiteral | char = CharLiteral | str = StrLiteral;
