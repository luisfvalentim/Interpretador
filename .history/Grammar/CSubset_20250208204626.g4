grammar CSubset;

program: (function | statement)+ EOF;

// Declarations
function: type ID '(' (parameters | /* vazio */) ')' block;
parameters: type ID (',' type ID)*;
type: 'int' | 'float' | 'char';

// Statements
statement: declaration 
         | ifStatement 
         | whileStatement 
         | forStatement 
         | doWhileStatement 
         | returnStatement
         | expressionStatement
         | block;  

block: '{' statement* '}';  // Block definition

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
expressionStatement: expression? ';';  // Now allows ";" alone

// Expressions
expression: assignmentExpression;

assignmentExpression: ID '=' expression    // ✅ Now correctly processes `x = x + 1;`
                    | logicalExpression;   // ✅ Otherwise, it's a logical expression

logicalExpression: equalityExpression (('&&' | '||') equalityExpression)*;

equalityExpression: relationalExpression (('==' | '!=') relationalExpression)*;

relationalExpression: additiveExpression (('<' | '>' | '<=' | '>=') additiveExpression)*;

additiveExpression: multiplicativeExpression (('+' | '-') multiplicativeExpression)*;

multiplicativeExpression: unaryExpression (('*' | '/' | '%') unaryExpression)*;

// ✅ Supports negative numbers (-x) and logical negation (!x)
unaryExpression: ('-' | '!')? primary;

primary: ID 
       | NUMBER 
       | STRING 
       | '(' expression ')' 
       | functionCall;

// Function Calls
functionCall: ID '(' (expression (',' expression)* | /* vazio */) ')';

// Tokens
ID: [a-zA-Z_][a-zA-Z0-9_]*;
NUMBER: [0-9]+ ('.' [0-9]+)?;
STRING: '"' (ESC | ~["\\])* '"';
fragment ESC: '\\' (["\\/bfnrt] | 'u' [0-9a-fA-F]); 
WS: [ \t\r\n]+ -> skip;
COMMENT: '//' ~[\r\n]* -> skip;
