grammar Esm;
options { language=CSharp; }
base: statement* EOF;

statement
    : stmt = baseStmt
    | label = LabelId ':' stmt = baseStmt
;

baseStmt
    : Op op = stackOp ';' #operationPerform
    | Let id = Identifier ';' #varDec
    | Load load = loadClause ';' #toStack
    | Store id = Identifier ';' #storeToVar
    | Print io = ioMode ';' #print
    | Goto (label = LabelId) (If condition = ifCond)? ';' #goto
    | Exit ';' #exit
;

// IO
Print: 'print';
ioMode: io = Ascii | io = Hex | io = Dec | io = Bin | io = Str;
Ascii: 'ascii';
Hex: 'hex';
Dec: 'dec';
Bin: 'bin';
Int: 'int';
Str: 'str';

Semi: ';';
Colon: ':';

stackOp
    : op = '&'
    | op = '|'
    | op = '~'
    | op = '^'
    | op = '<<'
    | op = '>>'
;

loadClause
    : Const lit = literal #loadConst
    | Local loc = Identifier #loadLocal
    | Input io = ioMode #loadInput
;

ifCond: not = Not? Zero;

Exit: 'exit';



// Load
Load: 'load';
Local: 'local';
Const: 'const';
Input: 'input';

// Store
Store: 'store';

// Local var, declare a new one
Let: 'let';


// Stack operations
Op: 'operate';
BwAnd: '&';
BwOr: '|';
BwNot: '~';
BwXor: '^';
BwLShift: '<<';
BwRShift: '>>';



// Jump operations
Goto: 'goto';

// Conditionals
If: 'if';
Not: '!';
Zero: 'zero';

literal: int = DecIntLiteral | int = HexIntLiteral | int = BinIntLiteral | char = CharLiteral | str = StrLiteral;

// Constants
DecIntLiteral: DECIMAL('_'* DECIMAL)* /*INT_SUFFIX?*/ ;
HexIntLiteral: '0'[Xx]('_'* HEX)+ /*INT_SUFFIX?*/ ;
BinIntLiteral: '0'[Bb]('_'* BINARY)+ /*INT_SUFFIX?*/ ;


LabelId: '#' ID_UNPREFIXED;

Identifier: '.' ID_UNPREFIXED;

CharLiteral: '\'' STR_CHAR '\'';
StrLiteral: '"' STR_CHAR* '"';

fragment STR_CHAR: (~[\n\r\\"] | ESC_SEQ);
fragment ESC_SEQ: '\\'(["\\0abfnrtv]|HEX_ESC);
fragment HEX_ESC: 'x' HEX (HEX (HEX HEX?)?)?;


//fragment INT_SUFFIX: [ui]'8';

fragment ID_UNPREFIXED: ID_START ID_CHAR*;

fragment ID_START :           LETTER | '_';
fragment ID_CHAR  : DECIMAL | LETTER | '_';

fragment LETTER  : [A-Za-z]   ;
fragment DECIMAL : [0-9]      ;
fragment HEX     : [0-9A-Fa-f];
fragment BINARY   : [01]      ;

fragment BLOCK_CMT : '/*' .*? '*/';
fragment LINE_CMT  : '//' .*? '\n';
fragment LINE_CMT_EOF  : '//' ~[\n]*? EOF;

Comment: (BLOCK_CMT|LINE_CMT|LINE_CMT_EOF) -> skip;

Whitespace: [ \t\r\n]+ -> skip;