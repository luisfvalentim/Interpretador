grammar CSubset;

program: (function | statement)+ EOF;

// Declarações
function: type ID '(' (parameters | /* vazio */) ')' block;
parameters: type ID (',' type ID)*;
type: 'int' | 'float' | 'char';

// Comandos
statement: declaration 
         | ifStatement 
         | whileStatement 
         | forStatement 
         | doWhileStatement 
         | returnStatement
         | expressionStatement
         | block;  // Suporte a blocos de código

block: '{' statement* '}';  // Definição de bloco de código

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
expressionStatement: expression? ';';  // Agora permite ";" isolado

// Expressões
expression: logicalExpression;

logicalExpression: equalityExpression (('&&' | '||') equalityExpression)*;

equalityExpression: relationalExpression (('==' | '!=') relationalExpression)*;

relationalExpression: additiveExpression (('<' | '>' | '<=' | '>=') additiveExpression)*;

additiveExpression: multiplicativeExpression (('+' | '-') multiplicativeExpression)*;

multiplicativeExpression: unaryExpression (('*' | '/' | '%') unaryExpression)*;

// 📌 Suporte adequado a números negativos e negação lógica (!x)
unaryExpression: ('-' | '!')? primary;

primary: ID 
       | NUMBER 
       | STRING 
       | '(' expression ')' 
       | functionCall;


// Chamada de função
functionCall: ID '(' (expression (',' expression)* | /* vazio */) ')';

// Tokens
ID: [a-zA-Z_][a-zA-Z0-9_]*;
NUMBER: [0-9]+ ('.' [0-9]+)?;
STRING: '"' (ESC | ~["\\])* '"';
fragment ESC: '\\' (["\\/bfnrt] | 'u' [0-9a-fA-F]); // Sem ações!
WS: [ \t\r\n]+ -> skip;
COMMENT: '//' ~[\r\n]* -> skip;
