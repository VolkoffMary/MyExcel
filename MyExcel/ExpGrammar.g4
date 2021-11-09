grammar ExpGrammar;

// Parcer rules

compileUnit : expression EOF;
expression 
    : LPAREN expression RPAREN #ParenthesizedExpr
    | operatorToken=(INC | DEC) LPAREN expression RPAREN #incdecExpr
    | expression EXPONENT expression #ExponentialExpr
    | operatorToken=(ADD|SUBTRACT) expression #UnaryExpr
    
    | expression operatorToken=(MULTIPLY | DIVIDE) expression #MultiplicativeExpr
    | expression operatorToken=(ADD | SUBTRACT) expression #AdditiveExpr
    | NUMBER #NumberExpr
    | IDENTIFIER #IdentifierExpr
;

// Lexer rules

NUMBER : ('0'..'9')+;
IDENTIFIER : [A-Z]+[1-9]?[0-9]+;
EXPONENT : '^';
MULTIPLY : '*';
DIVIDE : '/';
SUBTRACT: '-';
DEC: 'dec';
ADD: '+';
INC: 'inc';
LPAREN :'(';
RPAREN : ')';

WS : [ \r\t\n]+ -> skip ;