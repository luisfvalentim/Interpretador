grammar CSubset;

program : (statement | functionCall)* EOF;

statement 
    : declaration
    | assignmentExpression ';'
    | functionCall ';'
    ;

declaration : type ID ('=' expression)? ';';
assignmentExpression : ID '=' expression;

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
    | ID
    | STRING
    | functionCall
    | '!' primary
    | '(' expression ')'
    ;

functionCall : ID '(' (expression (',' expression)*)? ')';

type : 'int' | 'float' | 'char';

ID : [a-zA-Z_][a-zA-Z_0-9]*;
NUMBER : [0-9]+('.'[0-9]+)?;  // Aceita números inteiros e decimais
STRING : '"' (~["\\] | '\\' .)* '"';  // Permite strings com aspas
WS : [ \t\r\n]+ -> skip;




