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
            if (Value == null)
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
                if (_pos < _input.Length)
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
        private bool radiant = true;

        //Construktor
        public Parser(List<Token> tokens, bool rad)
        {
            _tokens = tokens;
            radiant = rad;
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
                string name = Current.Value!;
                Advance();

                if(!Match(TokenType.LParen))
                {
                    throw new Exception("'(' erwartet nach Funktionsname");
                }

                double inner = ParseExpr();

                if (!Match(TokenType.RParen))
                {
                    throw new Exception("')' erwartet");
                }

                return EvaluateFunction(name, inner);
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


        //evaluate method for the special functiions
        private double EvaluateFunction(string name, double value)
        {
            if(radiant == true)
            {
                switch (name.ToLower())
                {
                    case "sin":
                        return Math.Sin(value);

                    case "cos":
                        return Math.Cos(value);

                    case "tan":
                        return Math.Tan(value);

                    case "cot":
                        return 1/Math.Tan(value);

                    default:
                        throw new Exception($"Unbekannte Funktion: {name}");
                }
            }
            else
            {
                switch (name.ToLower())
                {
                    case "sin":
                        return Math.Sin(value * 180/Math.PI);

                    case "cos":
                        return Math.Cos(value * 180 / Math.PI);

                    case "tan":
                        return Math.Tan(value * 180 / Math.PI);

                    case "cot":
                        return 1 / Math.Tan(value);

                    default:
                        throw new Exception($"Unbekannte Funktion: {name}");
                }
            }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            //run loop
            bool run = true;
            while(run)
            {
                Console.WriteLine("Einstellungen - \t1: deg\n\t\t\t2: rad\n");
                string radi = Console.ReadLine();
                bool r = true;
                if (radi == "1")
                {
                    r = true;
                }
                else if (radi == "2")
                {
                    r = false;
                }
                else
                {
                    Console.WriteLine("unerwartete Einstelung ->\nEinstellung rad als standard gesetzt.");
                    r = true;
                }
                Console.WriteLine("\nAusdruck: ");
                string input = Console.ReadLine();
                Lexer lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();

                //tokens ausgeben
                foreach (Token t in tokens)
                {
                    Console.WriteLine(t);
                }

                Parser parser = new Parser(tokens, r);
                double result = parser.Parse();

                Console.WriteLine($"\nErgebnis: {result}");
                Console.WriteLine("\nWeitermachen?\n\t1: ja\n\t2: nein\n");
                string cont = Console.ReadLine();
                if(cont == "1")
                {
                    run = true;
                    Console.Clear();
                }
                else if(cont == "2")
                {
                    run = false;
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("\nunwerwartete Eingabe\n");
                    run= true;
                    Console.ReadKey();
                    Console.Clear();
                }
            }

        }
    }

}