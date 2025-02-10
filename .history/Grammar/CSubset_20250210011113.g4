grammar CSubset;

program : (statement | functionCall)* EOF;

statement 
    : declaration
    | assignmentExpression ';'
    | functionCall ';'
    | returnStatement  // ✅ Agora `return` funciona!
    | ifStatement
    | whileStatement
    | forStatement
    | doWhileStatement
    | switchStatement
    | function // ✅ Agora permite declaração de funções
    ;



whileStatement
    : 'while' '(' expression ')' block
    ;

forStatement
    : 'for' '(' (declaration | assignmentExpression)? ';' expression? ';' assignmentExpression? ')' block
    ;


doWhileStatement
    : 'do' block 'while' '(' expression ')' ';'
    ;

switchStatement
    : 'switch' '(' expression ')' '{' caseStatement* defaultStatement? '}'
    ;

caseStatement
    : 'case' NUMBER ':' statement* 'break' ';'
    ;

defaultStatement
    : 'default' ':' statement* 'break' ';'
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
    | ID ('[' expression ']')?  // ✅ Permite arr[0]
    | STRING
    | functionCall
    | CHAR 
    | '!' primary
    | '(' expression ')'
    ;


functionCall : ID '(' (expression (',' expression)*)? ')';

type : 'int' | 'float' | 'char' | 'string' | 'void';

ID : [\p{L}_][\p{L}\p{N}_]*;
NUMBER : [0-9]+('.'[0-9]+)?;  // Aceita números inteiros e decimais
CHAR : '\'' (~['\\] | '\\' .)* '\'';
STRING : '"' (~["\\] | '\\' .)* '"';
WS : [ \t\r\n]+ -> skip;
COMMENT : '//' ~[\r\n]* -> skip;    // Ignora comentários de linha
BLOCK_COMMENT : '/*' .*? '*/' -> skip; // Ignora comentários de bloco
returnStatement : 'return' expression? ';';
function : type ID '(' parameters? ')' block;
parameters : type ID (',' type ID)*; // Permite múltiplos parâmetros
declaration : type ID ('[' NUMBER ']')? ('=' expression)? ';';
expression : logicalExpression;
assignmentExpression : ID ('[' expression ']')? '=' expression; 

breakStatement : 'break' ';';

