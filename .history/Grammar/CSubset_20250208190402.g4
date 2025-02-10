grammar CSubset;

program: (function | statement)+ EOF;

// Declarações
function: type ID '(' (parameters | /* vazio */) ')' '{' statement* '}';
parameters: type ID (',' type ID)*;
type: 'int' | 'float' | 'char';

// Comandos
statement: declaration 
         | ifStatement 
         | whileStatement 
         | forStatement 
         | doWhileStatement 
         | returnStatement
         | expressionStatement;

declaration: type ID ('=' expression)? ';';
ifStatement: 'if' '(' expression ')' statement ('else' statement)?;
whileStatement: 'while' '(' expression ')' statement;
forStatement: 'for' '(' 
    (declaration | expression | /* vazio */) ';' 
    (expression | /* vazio */) ';' 
    (expression | /* vazio */) ')' 
    statement;
doWhileStatement: 'do' statement 'while' '(' expression ')' ';';
returnStatement: 'return' (expression | /* vazio */) ';';
expressionStatement: expression ';';

// Expressões
expression: logicalExpression;
logicalExpression: equalityExpression (('&&' | '||') equalityExpression)*;
equalityExpression: relationalExpression (('==' | '!=') relationalExpression)*;
relationalExpression: additiveExpression (('<' | '>' | '<=' | '>=') additiveExpression)*;
additiveExpression: multiplicativeExpression (('+' | '-') multiplicativeExpression)*;
multiplicativeExpression: primary (('*' | '/' | '%') primary)*;
primary: ID | NUMBER | STRING | '(' expression ')' | functionCall;

// Chamada de função
functionCall: ID '(' (expression (',' expression)* | /* vazio */) ')';

// Tokens
ID: [a-zA-Z_][a-zA-Z0-9_]*;
NUMBER: [0-9]+ ('.' [0-9]+)?;
STRING: '"' (ESC | ~["\\])* '"';
fragment ESC: '\\' (["\\/bfnrt] | 'u' [0-9a-fA-F]); // Sem ações!
WS: [ \t\r\n]+ -> skip;
COMMENT: '//' ~[\r\n]* -> skip;