lexer grammar EsmLexer;
options { language=CSharp; }
Print: 'print';
Utf8: 'utf8';
Utf16: 'utf16';
Hex: 'hex';
Dec: 'decimal';
Oct: 'octal';

Binary: 'binary';
Unary: 'unary';

Str: 'str';

Let: 'let';

Semi: ';';
Colon: ':';

Op: 'operate';

Arrow: '->';
Equal: '='; Equal2: '=='; BangEq: '!=';
BwAnd: '&';
BwOr: '|';
BwNot: '~';
BwXor: '^';
BwLShift: '<<';
BwRShift: '>>';
BwSRShift: '+>>';
Plus: '+';
Minus: '-';
Times: '*';
Slash: '/';
Mod: '%';

LCurly: '{';
RCurly: '}';

Exit: 'exit';

True: 'true';
False: 'false';

// Load
Push: 'push';
Input: 'input';

// Store
Store: 'store';

// Allocate a new pointer/variable
Alloc: 'alloc';


// Stack operations


Cast: 'cast';

I8:  'i8';
I16: 'i16';
I32: 'i32';
I64: 'i64';
ISize: 'isize';
U8:  'u8';
U16: 'u16';
U32: 'u32';
U64: 'u64';
USize: 'usize';

// Jump operations
Goto: 'goto';

Free: 'free';

// Conditionals
If: 'if';
Bang: '!';
Zero: 'zero';



// Constants
DecIntLiteral: DECIMAL('_'* DECIMAL)* /*INT_SUFFIX?*/ ;
HexIntLiteral: '0'[Xx]('_'* HEX)+ /*INT_SUFFIX?*/ ;
BinIntLiteral: '0'[Bb]('_'* BINARY)+ /*INT_SUFFIX?*/ ;
OctIntLiteral: '0'[Oo]('_'* OCTAL)+ /*INT_SUFFIX?*/ ;


LabelId: '#' ID_UNPREFIXED;

Identifier: '`'? ID_UNPREFIXED;

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
fragment OCTAL   : [0-7]      ;

LineComment  : '//' ~[\n]*? (EOF | '\n') -> skip;

BlockCommentStart: '/*' -> skip, pushMode(BLOCK_COMMENT_MODE);
Whitespace: [ \t\r\n]+ -> skip;

mode BLOCK_COMMENT_MODE;

BlockCommentRecStart: '/*' -> skip, pushMode(BLOCK_COMMENT_MODE);
BlockCommentEnd: '*/' -> skip, popMode;

CommentContents:  .+? -> skip;