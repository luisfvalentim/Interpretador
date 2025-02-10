grammar CSubset;

program : (statement | functionCall)* EOF;

statement 
    : declaration
    | assignmentExpression ';'
    | functionCall ';'
    | returnStatement 
    | breakStatement 
    | ifStatement
    | whileStatement
    | forStatement
    | doWhileStatement
    | switchStatement
    | function
    | structDeclaration  // ✅ Permite `struct`
    | unionDeclaration   // ✅ Permite `union`
    ;

whileStatement
    : 'while' '(' expression ')' block
    ;

forStatement
    : 'for' '(' (declaration | assignmentExpression)* ';' expression? ';' (assignmentExpression)* ')' block
    ;

doWhileStatement
    : 'do' block 'while' '(' expression ')' ';'
    ;

switchStatement
    : 'switch' '(' expression ')' '{' caseStatement* defaultStatement? '}'
    ;

caseStatement
    : 'case' NUMBER ':' statement* breakStatement
    ;

defaultStatement
    : 'default' ':' statement* breakStatement
    ;

ifStatement
    : 'if' '(' expression ')' block ('else' block)?
    ;

block
    : '{' statement* '}'
    ;

logicalExpression
    : equalityExpression
    | logicalExpression ('&&' | '||') equalityExpression
    ;

equalityExpression
    : relationalExpression
    | equalityExpression ('==' | '!=') relationalExpression
    ;

relationalExpression
    : additiveExpression
    | relationalExpression ('>' | '<' | '>=' | '<=') additiveExpression
    ;

additiveExpression
    : multiplicativeExpression
    | additiveExpression ('+'|'-') multiplicativeExpression
    ;

multiplicativeExpression
    : primary
    | multiplicativeExpression ('*'|'/'|'%') primary
    ;

primary
    : NUMBER
    | ID ('[' expression ']')?  // ✅ Permite `arr[0]`
    | STRING
    | functionCall
    | CHAR 
    | structInstance  // ✅ Permite instância de `struct`
    | unionInstance   // ✅ Permite instância de `union`
    | '!' primary
    | '(' expression ')'
    ;

structDeclaration
    : 'struct' ID '{' structMember* '}' ';'
    ;

structMember
    : type ID ';'
    ;

unionDeclaration
    : 'union' ID '{' structMember* '}' ';'
    ;

structInstance
    : 'struct' ID ID ';' // ✅ `struct Pessoa p;`
    | 'struct' ID ID '=' ID ';' // ✅ `struct Pessoa p = outraPessoa;`
    ;

unionInstance
    : 'union' ID ID ';'  // ✅ Declaração de `union`
    | ID '.' ID  // ✅ Acesso a membro de `union`
    ;

expression
    : logicalExpression
    | expression '.' ID // ✅ Acesso a `struct` (`pessoa.nome`)
    | expression '->' ID // ✅ Acesso via ponteiro (`pessoa->nome`)
    ;

functionCall : ID '(' (expression (',' expression)*)? ')';

type : 'int' | 'float' | 'char' | 'string' | 'void' | 'struct' | 'union';

ID : [\p{L}_][\p{L}\p{N}_]*;
NUMBER : [0-9]+('.'[0-9]+)?;  
CHAR : '\'' (~['\\] | '\\' .)* '\'';
STRING : '"' (~["\\] | '\\' .)* '"';
WS : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;
BLOCK_COMMENT : '/*' .*? '*/' -> skip;
returnStatement : 'return' expression? ';';
function : type ID '(' parameters? ')' block;
parameters : type ID (',' type ID)*; 
declaration : type ID ('[' NUMBER ']')? ('=' expression)? ';';
assignmentExpression : ID ('[' expression ']')? '=' expression; 
breakStatement : 'break' ';';
