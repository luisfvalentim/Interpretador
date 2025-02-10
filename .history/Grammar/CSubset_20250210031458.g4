grammar CSubset;

program : preprocessorDirective* (statement | functionCall)* EOF; // ✅ Agora permite diretivas de pré-processador

preprocessorDirective
    : '#' 'include' ('<' ID '.' ID '>' | STRING) // ✅ `#include <stdio.h>` ou `#include "arquivo.h"`
    | '#' 'define' ID expression // ✅ `#define TAMANHO 10`
    | '#' 'error' STRING // ✅ `#error "Mensagem"`
    ;

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
    | structDeclaration
    | unionDeclaration
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
    | ID ('[' expression ']')?  
    | STRING
    | functionCall
    | CHAR 
    | structInstance  
    | unionInstance  
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
    : ID ID ';' 
    | ID ID '=' ID ';' 
    ;

unionInstance
    : ID '.' ID  
    ;

expression
    : logicalExpression
    | structInstance
    | unionInstance
    | expression '->' ID
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
