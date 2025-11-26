# calculator in C# und WPF

## contents

1. [what is this documentation about](#what-is-this-documentation-about?)
2. [concept](#concept)
3. [main & enum](#main-and-enum)
4. [token](#token)
5. [lexer](#lexer)
6. [parser](#parser)
7. [the whole program](#the-whole-program)

---

### what is this documentation about?

This documentation explains how the calculator implemented in C# works. It also describes how individual parts of the program function and how they interact with each other. The documentation attempts to explain as much of the code as possible, but first the concept of the calculator is introduced.

---

### concept

This section of the documentation explains the basic concept behind the calculator.
My calculator is capable of performing all simple arithmetic operations (+, -, *, /) as well as evaluating expressions with brackets ().
My idea was to use the formal grammar of arithmetic operations.

What follows is the EBNF (Extended Backus–Naur Form) notation of this grammar. Please note that this grammar uses terminal symbols as well as non-terminal symbols. The start symbol of the grammar is Expr

    Expr    = ["+" | "-"] Term {("+" | "-") Term}.
    Term    = Factor {("*" | "/") Factor}.
    Factor  = ident | number | "(" Expr ")".

Metasymbols of that grammer:

| symbol | usage |
|--------|-------|
| &#124; | seperates alternatives |
| (...)  | groups alternatives |
| [...]  | marks optional symbols |
| {...}  | repitition (0 to infiniy) |

An expression (Expr) begins with an optional sign (+ or -), followed by one or more terms (Term). These terms are separated by + or -.
A term consists of multiple factors (Factor). Factors are separated by * or /.
A factor consists of either a special function (ident) such as pow(), sin(), a number (number), or an expression enclosed in brackets. Note that the brackets in Factor are terminal symbols that will later appear as input in the program. They will be interpreted as tokens, which is why they are written using quotation marks "(" and ")".
Now we can implement a parser that processes a stream of tokens following this concept. The tokens are provided by the lexer. The lexer itself is a process that takes our input string (the expression) and converts it into tokens. Note that the algorithm to parse the expression later will be recursive

---

### main and enum

This section provides detailed information about the Main method as well as the enumeration that defines the tokens.

The enumeration:
- An enumeration defines a common type for a group of related values
- this enumeration defines the types of Tokens the calculator will use later

```C#
    public enum TokenType { Number, Plus, Minus, Star, Slash, LParen, RParen, Identifier, EndOfInput }
```

The Main method:
```C#
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ausdruck: ");
            string input = Console.ReadLine();
            Lexer lexer = new Lexer(input);
            List<Token> tokens = lexer.Tokenize();

            Parser parser = new Parser(tokens);
            double result = parser.Parse();

            Console.WriteLine($"\nErgebnis: {result}");
        }
    }
```

In the Main method, the first step of the calculator program is to read an expression, in this case an input string. This string is then processed by the lexer to produce a list of tokens. The next step is to use the parser to process these tokens. The result is stored in result and displayed to the user.

---

### token

The Token class defines what a token is. In this case, a token consists of two characteristics: a type and an optional value. Note that the type of a token can only be one of those previously defined in the enumeration. The value can be null. The token type only needs to be converted to a string for output purposes.

The class Token:
```C#
    public class Token
    {
        //defines characteristics of tokens
        public TokenType Type { get; }
        public string? Value { get; }

        //Constructor
        public Token(TokenType type, string? value = null)
        {
            Type = type;
            Value = value;
        }

        //used to turn token object into a string to display later
        //optional
        public override string ToString()
        {
            if(Value == null)
            {
                return Type.ToString();
            }
            else
            {
                return $"{Type}({Value})";
            }
        }
    }
```

---

### lexer

The Lexer class can be a bit hard to understand, which is why it is split into individual parts and then assembled together.
The lexer is the part of the program that converts the input string into tokens.

```C#
        //members of the Lexer-class
        //used to store a copy of the input string
        private string _input;
        //used to store the current position in the input string
        private int _pos = 0;

        //Constructor
        public Lexer(string inp)
        {
            _input = inp;
        }
```

the next part is the member 'Current'
- Current returns the current character in the input string
- When the end of the input string is reached, it returns \0 so the lexer knows when to stop

```C#
        private char Current
        {
            get
            {
                if(_pos < _input.Length)
                {
                    return _input[_pos];
                }
                else
                {
                    return '\0';
                }
            }
        }
```

The next part is the method Tokenize():

- It is the function that converts the input string into a list of tokens and returns that list.
- The function checks every character of the input string.
- It skips whitespaces.
- If the current character is a number, it adds a number token.
- If the current character is a letter, it adds an identifier token.
- If the current character is one of the terminal symbols (+, -, *, /, (, )), it adds the corresponding token.
- If the current character is not recognized, the program throws an exception for an unknown character.
- When the function reaches the end of the input string, it adds the end-of-input token.

```C#
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while(_pos < _input.Length)
            {
                char c = Current;
                //skip whitespaces
                if(char.IsWhiteSpace(c))
                {
                    _pos++;
                    continue;
                }
                //number
                if(char.IsDigit(c) || (c == '.' && char.IsDigit(Peek())))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }
                //identifier: sin, pow...
                if(char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                //terminal symbols "+" or "("...
                switch(c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus));
                        _pos++;
                        break;

                    case '-':
                        tokens.Add(new Token(TokenType.Minus));
                        _pos++;
                        break;

                    case '*':
                        tokens.Add(new Token(TokenType.Star));
                        _pos++;
                        break;

                    case '/':
                        tokens.Add(new Token(TokenType.Slash));
                        _pos++;
                        break;

                    case '(':
                        tokens.Add(new Token(TokenType.LParen));
                        _pos++;
                        break;

                    case ')':
                        tokens.Add(new Token(TokenType.RParen));
                        _pos++;
                        break;

                    default:
                        throw new Exception($"Unbekanntes Zeichen: {c}");
                }

            }
            tokens.Add(new Token(TokenType.EndOfInput));
            return tokens;
        }
```

Next, we define the methods that handle what happens when a number or an identifier is read.

- The first method is ReadNumber().
- It can read integers as well as decimal numbers.
- It creates a token of type Number and stores the value as a string.

```C#
        private Token ReadNumber()
        {
            int start = _pos;
            //read digits, and optional point and continuing digits
            while(char.IsDigit(Current) || Current == '.')
            {
                _pos++;
            }
            string numberString = _input.Substring(start, _pos - start);
            return new Token(TokenType.Number, numberString);
        }
```

- the next Method is ReadIdentifier()
- it reads identifiers and sets the token-type to identifier
- then it stores the value as a string

```C#
        private Token ReadIdentifier()
        {
            int start = _pos;
            //read letters as identifiers
            while(char.IsLetter(Current))
            {
                _pos++;
            }
            string s = _input.Substring(start, _pos - start);
            return new Token(TokenType.Identifier, s);
        }
```

- the last method of the Lexer is the method Peek().
- this this method is used as a helper Method for ReadNumber
- it can look at the next character of a string, without increasing th position value

```C#
        private char Peek()
        {
            if(_pos + 1 < _input.Length)
            {
                return _input[_pos + 1];
            }
            return '\0';
        }
```

---

### parser

The Parser class takes the list of tokens produced by the lexer and uses the grammar of arithmetic operations to turn them into a computable value.
The class Parser has three members:
- a List, that contains the tokens that will be processed
- a position of the current token
- a property Current that returns the token at the current position

in the constructor of Parser a list of tokens is passed as a parameter and then the member list of tokens is initialized with these tokens

```C#
    public class Parser
    {
        private List<Token> _tokens;
        private int _pos = 0;

        //Construktor
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        private Token Current
        {
            get
            {
                if(_pos < _tokens.Count)
                {
                    return _tokens[_pos];
                }
                else
                {
                    return _tokens[^1]; //last Token
                }
            }
        }
    }
```

the next part are two methods:
- The method Advance() returns the current token and moves the parser to the next one

```C#
        private Token Advance()
        {
            Token t = Current;
            if(_pos < _tokens.Count)
            {
                _pos++;
            }
            return t;
        }
```

- the next method is the method Match(), that returns true, when a token type matches a given type, then it moves on to the next token

```C#
        private bool Match(TokenType type)
        {
            if(Current.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }
```

- the next method is important, it's called Parse() and it returns the final value of the expression, so the result of the grammer of the arithmetic operations, unless there is an unexpected token

```C#
        public double Parse()
        {
            double value = ParseExpr();

            if(Current.Type != TokenType.EndOfInput)
            {
                throw new Exception($"Unerwartetes Token: {Current}");
            }
            return value;
        }
```

the next part is the mehtod: ParseExpr()
- it returns the expression, so the result of ["+" | "-"] Term { ("+" | "-") Term }

```C#
        //Expr = ["+" | "-"] Term { ("+" | "-") Term }
        private double ParseExpr()
        {
            double value;


            //optional prefixes: ["+" | "-"]
            if(Match(TokenType.Plus))
            {
                value = ParseTerm();
            }
            else if(Match(TokenType.Minus))
            {
                value = -ParseTerm();
            }
            else
            {
                value = ParseTerm();
            }



            //repitition of { "+" Term } und { "-" Term }
            while(true)
            {
                if(Match(TokenType.Plus))
                {
                    value += ParseTerm();
                }
                else if(Match(TokenType.Minus))
                {
                    value -= ParseTerm();
                }
                else
                {
                    break;
                }
            }
            return value;
        }
```

the next Part is the method ParseTerm()
- it takes care of the following rule of the grammer: Term = Factor { ("*" | "/") Factor }

```C#
        //Term = Factor { ("*" | "/") Factor }
        private double ParseTerm()
        {
            double value = ParseFactor();

            while(true)
            {
                if(Match(TokenType.Star))
                {
                    value *= ParseFactor();
                }
                else if(Match(TokenType.Slash))
                {
                    value /= ParseFactor();
                }
                else
                {
                    break;
                }
            }
            return value;
        }
```

the last part is the ParseFactor() method
- it takes care of of the last rule of the grammer: Factor = ident | number | "(" Expr ")"
- the program does not cover mathematical functions like sin(), cos() or pow() so far, those will be implemented later

```C#
        //Factor = ident | number | "(" Expr ")"
        private double ParseFactor()
        {
            //Number
            if(Current.Type == TokenType.Number)
            {
                string number = Current.Value;
                Advance();
                return double.Parse(number);
            }

            //Ident (e.g. sin, pow) -> implement later
            if(Current.Type == TokenType.Identifier)
            {
                string name = Current.Value;
                Advance();

                if(Match(TokenType.LParen))
                {
                    double inner = ParseExpr();
                    if(!Match(TokenType.RParen))
                    {
                        throw new Exception("')' erwartet");
                    }

                    throw new Exception($"Funktion '{name}' ist noch nicht implementiert.");
                }
                throw new Exception($"Funktion '{name}' muss in Klammern geschrieben werden, z.B. {name}(...)");
            }

            //"(" Expr ")"
            if(Match(TokenType.LParen))
            {
                double inner = ParseExpr();

                if(!Match(TokenType.RParen))
                {
                    throw new Exception("')' erwartet");
                }
                return inner;
            }
            throw new Exception($"Unerwartetes Token in Factor: {Current}");
        }
```

---

### the whole program

below you can see the whole program, take a look and see for yourself how everything works together:

```C#
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Main
{
    public enum TokenType
    { Number, Plus, Minus, Star, Slash, LParen, RParen, Identifier, EndOfInput }

    public class Token
    {
        public TokenType Type { get; }
        public string? Value { get; }

        //Konstruktor
        public Token(TokenType type, string? value = null)
        {
            Type = type;
            Value = value;
        }

        //umwandlung Tokens von  in string zum ausgeben
        /*public override string ToString()
        {
            if(Value == null)
            {
                return Type.ToString();
            }
            else
            {
                return $"{Type}({Value})";
            }
        }*/
    }

    public class Lexer
    {
        private string _input;
        private int _pos = 0;

        //Konstruktor
        public Lexer(string inp)
        {
            _input = inp;
        }

        private char Current
        {
            get
            {
                if(_pos < _input.Length)
                {
                    return _input[_pos];
                }
                else
                {
                    return '\0';
                }
            }
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while(_pos < _input.Length)
            {
                char c = Current;
                //whitespaces skippen
                if(char.IsWhiteSpace(c))
                {
                    _pos++;
                    continue;
                }
                //number
                if(char.IsDigit(c) || (c == '.' && char.IsDigit(Peek())))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }
                //identifier sin, pow...
                if(char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                //Terminalsymbole "+" oder "("...
                switch(c)
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus));
                        _pos++;
                        break;

                    case '-':
                        tokens.Add(new Token(TokenType.Minus));
                        _pos++;
                        break;

                    case '*':
                        tokens.Add(new Token(TokenType.Star));
                        _pos++;
                        break;

                    case '/':
                        tokens.Add(new Token(TokenType.Slash));
                        _pos++;
                        break;

                    case '(':
                        tokens.Add(new Token(TokenType.LParen));
                        _pos++;
                        break;

                    case ')':
                        tokens.Add(new Token(TokenType.RParen));
                        _pos++;
                        break;

                    default:
                        throw new Exception($"Unbekanntes Zeichen: {c}");
                }

            }
            tokens.Add(new Token(TokenType.EndOfInput));
            return tokens;
        }

        private Token ReadNumber()
        {
            int start = _pos;
            //read digits, and optional point and continuing digits
            while(char.IsDigit(Current) || Current == '.')
            {
                _pos++;
            }
            string numberString = _input.Substring(start, _pos - start);
            return new Token(TokenType.Number, numberString);
        }

        private Token ReadIdentifier()
        {
            int start = _pos;
            //read letters as identifiers
            while(char.IsLetter(Current))
            {
                _pos++;
            }
            string s = _input.Substring(start, _pos - start);
            return new Token(TokenType.Identifier, s);
        }

        private char Peek()
        {
            if(_pos + 1 < _input.Length)
            {
                return _input[_pos + 1];
            }
            return '\0';
        }

    }

    public class Parser
    {
        private List<Token> _tokens;
        private int _pos = 0;

        //Construktor
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        private Token Current
        {
            get
            {
                if(_pos < _tokens.Count)
                {
                    return _tokens[_pos];
                }
                else
                {
                    return _tokens[^1]; //last Token
                }
            }
        }

        private Token Advance()
        {
            Token t = Current;
            if(_pos < _tokens.Count)
            {
                _pos++;
            }
            return t;
        }

        private bool Match(TokenType type)
        {
            if(Current.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }


        public double Parse()
        {
            double value = ParseExpr();

            if(Current.Type != TokenType.EndOfInput)
            {
                throw new Exception($"Unerwartetes Token: {Current}");
            }
            return value;
        }


        //Expr = ["+" | "-"] Term { ("+" | "-") Term }
        private double ParseExpr()
        {
            double value;


            //optional prefixes: ["+" | "-"]
            if(Match(TokenType.Plus))
            {
                value = ParseTerm();
            }
            else if(Match(TokenType.Minus))
            {
                value = -ParseTerm();
            }
            else
            {
                value = ParseTerm();
            }



            //repitition { "+" Term } und { "-" Term }
            while(true)
            {
                if(Match(TokenType.Plus))
                {
                    value += ParseTerm();
                }
                else if(Match(TokenType.Minus))
                {
                    value -= ParseTerm();
                }
                else
                {
                    break;
                }
            }
            return value;
        }



        //Term = Factor { ("*" | "/") Factor }
        private double ParseTerm()
        {
            double value = ParseFactor();

            while(true)
            {
                if(Match(TokenType.Star))
                {
                    value *= ParseFactor();
                }
                else if(Match(TokenType.Slash))
                {
                    value /= ParseFactor();
                }
                else
                {
                    break;
                }
            }
            return value;
        }



        //Factor = ident | number | "(" Expr ")"
        private double ParseFactor()
        {
            //Number
            if(Current.Type == TokenType.Number)
            {
                string number = Current.Value;
                Advance();
                return double.Parse(number);
            }

            //Ident (e.g. sin, pow) -> implement later
            if(Current.Type == TokenType.Identifier)
            {
                string name = Current.Value;
                Advance();

                if(Match(TokenType.LParen))
                {
                    double inner = ParseExpr();
                    if(!Match(TokenType.RParen))
                    {
                        throw new Exception("')' erwartet");
                    }

                    throw new Exception($"Funktion '{name}' ist noch nicht implementiert.");
                }
                throw new Exception($"Funktion '{name}' muss in Klammern geschrieben werden, z.B. {name}(...)");
            }

            //"(" Expr ")"
            if(Match(TokenType.LParen))
            {
                double inner = ParseExpr();

                if(!Match(TokenType.RParen))
                {
                    throw new Exception("')' erwartet");
                }
                return inner;
            }
            throw new Exception($"Unerwartetes Token in Factor: {Current}");
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ausdruck: ");
            string input = Console.ReadLine();
            Lexer lexer = new Lexer(input);
            List<Token> tokens = lexer.Tokenize();

            //tokens ausgeben
            foreach(Token t in tokens)
            {
                Console.WriteLine(t);
            }

            Parser parser = new Parser(tokens);
            double result = parser.Parse();

            Console.WriteLine($"\nErgebnis: {result}");
        }
    }

}
```




