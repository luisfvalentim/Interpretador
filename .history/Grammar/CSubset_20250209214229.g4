grammar CSubset;

program : (statement | functionCall)* EOF;

statement 
    : declaration
    | assignmentExpression ';'
    | functionCall ';'
    | ifStatement
    | whileStatement
    | forStatement
    | doWhileStatement
    | switchStatement
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

declaration 
    : type ID ('[' NUMBER ']')? ('=' expression)? ';'  // Suporte a arrays
    ;

assignmentExpression 
    : ID ('[' expression ']')? '=' expression  // Suporte a atribuição de elementos do array
    ;

expression : logicalExpression;

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
    | ID ('[' expression ']')?  // Suporte ao acesso de elementos do array
    | STRING
    | functionCall
    | CHAR 
    | '!' primary
    | '(' expression ')'
    ;

functionCall : ID '(' (expression (',' expression)*)? ')';

type : 'int' | 'float' | 'char' | 'string'; 

ID : [a-zA-Z_][a-zA-Z_0-9]*;
NUMBER : [0-9]+('.'[0-9]+)?;  // Aceita números inteiros e decimais
CHAR : '\'' (~['\\] | '\\' .)* '\'';
STRING : '"' (~["\\] | '\\' .)* '"';
WS : [ \t\r\n]+ -> skip;
