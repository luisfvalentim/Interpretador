grammar CSubset;

program : (statement | functionCall)* EOF;

statement 
    : declaration
    | assignmentExpression ';'
    | functionCall ';'
    ;

declaration : type ID ('=' expression)? ';';
assignmentExpression : ID '=' expression;

expression : additiveExpression;

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
    | functionCall
    | '(' expression ')'
    ;

functionCall : ID '(' (expression (',' expression)*)? ')';

type : 'int' | 'float' | 'char';

ID : [a-zA-Z_][a-zA-Z_0-9]*;
NUMBER : [0-9]+;
STRING : '"' ~["]* '"';
WS : [ \t\r\n]+ -> skip;

