# Parser
The makings of an LL(1) compiler made in C# for a made up language

# Language
```
Start symbol: <PROGRAM>

Semantics: With the exception of <NEGATIVE NUMBER>, white space, tabs, carriage returns and such are meaningless to this grammer.  Although in practice statements will generally be written on different lines, your scanner shouldn't care.

Special note: "e" stands for the empty string.
Language Definition

<PROGRAM> ::= PROGRAM <VARIABLE NAME>; BEGIN <STATEMENT> END.

<STATEMENT> ::= <READ STATEMENT><STATEMENT> | <WRITE STATEMENT><STATEMENT> | <FOR STATEMENT><STATEMENT> | <IF STATEMENT><STATEMENT> | <CASE STATEMENT><STATEMENT> | <NUM ASSIGNMENT STATEMENT><STATEMENT> | <STRING ASSIGNMENT STATEMENT><STATEMENT> | <ARRAY STATEMENT><STATEMENT> | <PROCEDURE DECLARATION STATEMENT><STATEMENT> | <PROCEDURE CALL STATEMENT><STATEMENT> | <RETURN STATEMENT><STATEMENT> | <NUM DECLARATION STATEMENT><STATEMENT> | <STRING DECLARATION STATEMENT><STATEMENT> | <STRING CONCATENATION><STATEMENT> | <ARRAY ASSIGNMENT STATEMENT><STATEMENT> | e

<DIGIT> ::= 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9

<NUMBER> ::= <DIGIT><BETA>

<BETA> ::= e | <NUMBER>

<NEGATIVE NUMBER> ::= -<NUMBER>

<POS OR NEG> ::= <NUMBER> | <NEGATIVE NUMBER>

<LETTER> ::= A  | B  | C | D | E | F | G | H | I | J | K | L | M | N | O | P | Q |  R | S | T | U | V | W | X | Y | Z

<CHARACTER> ::= <LETTER> | <DIGIT> | . | , | < | > | ? | { | } | _  | ! | @ | # | $ | & | "space" (i.e a space: " ") | ~ | ` | :

<VAR CHARACTER> ::= <LETTER> | <DIGIT> | _

<WORD> ::= <LETTER><GAMMA>

<GAMMA> ::= e | <CHARACTER><GAMMA>

<STRING CONSTANT> ::= "<WORD>"

<VARIABLE NAME> ::= <LETTER><VARIABLE NAME PRIME>

<VARIABLE NAME PRIME> ::= <VAR CHARACTER><VARIABLE NAME PRIME> | e

<CASE STATEMENT> ::= SWITCH (<VARIABLE NAME>) { <CASE PART> }

<CASE PART> ::= CASE <EXP>:{<STATEMENT>} <CASE PART> | DEFAULT:{<STATEMENT>}

<WRITE STATEMENT> ::= WRITE <ALPHA>

<ALPHA> ::= <EXP>; | <STRING CONSTANT>; | <VARIABLE NAME>

<READ STATEMENT> ::= READ <VARIABLE NAME>;
//Note that for simplicity we only read positive or negative integers.

<STRING CONCATENATION> ::= <VARIABLE NAME> = <VARIABLE NAME> + <VARIABLE NAME>;
//The semantics on "string concatenation" is that all three <variable name>'s have to be strings.

<FOR STATEMENT> ::= FOR <VARIABLE NAME> = <EXP> TO <EXP> STEP <EXP> DO { <STATEMENT> }

//    For example:
//        FOR i = bob TO ralph STEP num3 DO { <STATEMENT> }
//        FOR i = 0 TO 10 STEP 1 DO { <STATEMENT> }
//        FOR i = bob TO 10 STEP num3 DO { <STATEMENT> }
    
<IF STATEMENT> ::= IF <CONDITION> THEN { <STATEMENT>} <THE FORCE>

<THE FORCE> ::= e | ELSE { <STATEMENT> }

<NUM ASSIGNMENT STATEMENT> ::= <VARIABLE NAME> = <EXP>;

<STRING ASSIGNMENT STATEMENT> ::= <VARIABLE NAME> = <DEATH STAR>

<DEATH STAR> ::= <VARIABLE NAME><ALDERAAN> | <STRING CONSTANT>;

<ALDERAAN> ::= ; | + <VARIABLE NAME>;
<ARRAY STATEMENT> ::= ARRAY <VARIABLE NAME>[<POS OR NEG>..<POS OR NEG><ARRAY LIST>

<ARRAY LIST> ::= ,<POS OR NEG>..<POS OR NEG><ARRAY LIST> | ];

//    Array Statement example:
//        ARRAY bob1[0..10,-5..200];
//        ARRAY bob5[-3..12,0..5,0..20];

<ARRAY ASSIGNMENT STATEMENT> ::= <VARIABLE NAME>[<EXP><ARRAY ASSIGNMENT LIST>

<ARRAY ASSIGNMENT LIST> ::= ,<EXP><ARRAY ASSIGNMENT LIST> | ] = <EXP>;

//    Array Assignment Statement example:
//        First assignment:
//            bob1[3,4] = 5;
//            bob5[-2,0,20] = -999;
//        Second assignment (array subscripting):
//            bob1[(3+5)*num2,55*8^9] = 0;
//            bob5[88/2,bob1[num2,num3],bob1[9*9,num3^4]] = 9;

<PROCEDURE DECLARATION STATEMENT> ::= PROCEDURE <VARIABLE NAME> ( <VARIABLE PASSING>

//    Example:
//        PROCEDURE myProcedure(num *variable, string variable2)  { <STATEMENT> }
//        procedure myProcedure2() { <STATEMENT> }
//    Semantics: when the procedure is declared as pass by reference (the *) then it doesn't matter how the procedure is called as that variable will be passed by reference, not value.

<VARIABLE PASSING> ::= ) { <STATEMENT> } | <VARIABLE LIST>

<VARIABLE LIST> ::= <PASS BY VALUE> | <PASS BY REFERENCE>

<PASS BY REFERENCE> ::= NUM *<VARIABLE NAME><VARIABLE LISTING> | STRING *<VARIABLE NAME><VARIABLE LISTING>

<PASS BY VALUE> ::= NUM <VARIABLE NAME><VARIABLE LISTING> | STRING <VARIABLE NAME><VARIABLE LISTING>

<VARIABLE LISTING> ::= ,<VARIABLE LIST> | ) { <STATEMENT> }

<PROCEDURE CALL STATEMENT> ::= <VARIABLE NAME> ( <VARIABLE PASS>

//    Example:
//        myProcedure(num1, str1);   or...
//        myProcedure2();

<VARIABLE PASS> ::= ); | <VARIABLES PASSED IN>

<VARIABLES PASSED IN> ::= <VARIABLE NAME><VARIABLES LISTED>

<VARIABLES LISTED> ::= ,<VARIABLES PASSED IN> | );

<RETURN STATEMENT> ::= RETURN;

<NUM DECLARATION STATEMENT> ::= NUM <VARIABLE NAME><DELTA>

<DELTA> ::= ; | = <EXP>;

<STRING DECLARATION STATEMENT> ::= STRING <VARIABLE NAME><DARTH VADER>

<DARTH VADER> ::= = <LUKE> | ;

<LUKE> ::= <VARIABLE NAME>; | <STRING CONSTANT>;

//Sematics: note that "string <variable name> = <variable name>; that the
//second variable name has to be of type "string" or it should throw an error.

<CONDITION> ::= <EXP> <RELATION OPERATOR> <EXP>

<RELATION OPERATOR> ::= <<YODA> | ><YODA> | == | !=

<YODA> ::= e | =

<EXP> ::= <LI><AE>
//The semantics on this is that a <variable name> has to be a num or an array. So, this could have positive or negative numbers, or nums, or arrays, or arithmetic expressions that deal with them.


<AE> ::= +<LI><AE> | -<LI><AE> | /<LI><AE> | *<LI><AE> | ^<LI><AE> | e

<LI> ::= <OP> | <PAREN>

<PAREN> ::= (<EXP>)

<OP> ::= <POS OR NEG> | <VARIABLE NAME><REF>

<REF> ::= <AP> | e

<AP> ::= [<EXP><ST>]

<ST> ::= ,<EXP><ST> | e
```

Current Log: <br/>
Nov 3, 2021: Full LL(1) parser functioning based off of simple math without assignment
* Add (x + y)
* Subtract (x - y)
* Multiply (x * y)
* Divide (x / y)
* Exponents (x ^ y)

*negative numbers are supported* <br/>
Parser prints `valid` or `invalid` to console based off of results
