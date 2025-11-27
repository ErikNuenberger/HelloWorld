using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Taschenrechner_WPF
{
    public enum TokenType
    { Number, Plus, Minus, Star, Slash, LParen, RParen, Identifier, EndOfInput, Semicolon }

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
            while (_pos < _input.Length)
            {
                char c = Current;
                //whitespaces skippen
                if (char.IsWhiteSpace(c))
                {
                    _pos++;
                    continue;
                }
                //number
                if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek())))
                {
                    tokens.Add(ReadNumber());
                    continue;
                }
                //identifier sin, pow...
                if (char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifier());
                    continue;
                }

                //Terminalsymbole "+" oder "("...
                switch (c)
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

                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon));
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
            bool hasDecimal = false;

            while (true)
            {
                if (char.IsDigit(Current))
                {
                    _pos++;
                }
                else if (Current == '.' && !hasDecimal)
                {
                    hasDecimal = true;
                    _pos++;
                }
                else
                {
                    break;
                }
            }

            string number = _input.Substring(start, _pos - start);

            return new Token(TokenType.Number, number);
        }


        private Token ReadIdentifier()
        {
            int start = _pos;
            //read letters as identifiers
            while (char.IsLetter(Current))
            {
                _pos++;
            }
            string s = _input.Substring(start, _pos - start);
            return new Token(TokenType.Identifier, s);
        }

        private char Peek()
        {
            if (_pos + 1 < _input.Length)
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
                if (_pos < _tokens.Count)
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
            if (_pos < _tokens.Count)
            {
                _pos++;
            }
            return t;
        }

        private bool Match(TokenType type)
        {
            if (Current.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }


        public double Parse()
        {
            double value = ParseExpr();

            if (Current.Type != TokenType.EndOfInput)
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
            if (Match(TokenType.Plus))
            {
                value = ParseTerm();
            }
            else if (Match(TokenType.Minus))
            {
                value = -ParseTerm();
            }
            else
            {
                value = ParseTerm();
            }



            //repitition { "+" Term } und { "-" Term }
            while (true)
            {
                if (Match(TokenType.Plus))
                {
                    value += ParseTerm();
                }
                else if (Match(TokenType.Minus))
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

            while (true)
            {
                if (Match(TokenType.Star))
                {
                    value *= ParseFactor();
                }
                else if (Match(TokenType.Slash))
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
            if (Current.Type == TokenType.Number)
            {
                string number = Current.Value;
                Advance();
                return double.Parse(number, System.Globalization.CultureInfo.InvariantCulture);
            }

            //Ident (e.g. sin, pow) -> implement later
            if (Current.Type == TokenType.Identifier)
            {
                string name = Current.Value;
                Advance();

                if (name.ToLower() == "pi")
                {
                    return Math.PI;
                }
                if (name.ToLower() == "e")
                {
                    return Math.E;
                }

                if (!Match(TokenType.LParen))
                {
                    throw new Exception("'(' erwartet nach Funktionsname");
                }

                List<double> args = new List<double>();
                args.Add(ParseExpr());
                while (Match(TokenType.Semicolon))
                {
                    args.Add(ParseExpr());
                }

                if (!Match(TokenType.RParen))
                {
                    throw new Exception("')' erwartet");
                }

                return EvaluateFunction(name, args.ToArray());
            }
            //"(" Expr ")"
            if (Match(TokenType.LParen))
            {
                double inner = ParseExpr();

                if (!Match(TokenType.RParen))
                {
                    throw new Exception("')' erwartet");
                }
                return inner;
            }
            throw new Exception($"Unerwartetes Token in Factor: {Current}");
        }


        //evaluate method for the special functiions
        private double EvaluateFunction(string name, params double[] args)
        {
            if (radiant == true)
            {
                switch (name.ToLower())
                {
                    case "sin":
                        if (args.Length != 1)
                        {
                            throw new Exception("sin() erwartet 1 Argument");
                        }
                        return Math.Sin(args[0]);

                    case "cos":
                        if (args.Length != 1)
                        {
                            throw new Exception("cos() erwartet 1 Argument");
                        }
                        return Math.Cos(args[0]);

                    case "tan":
                        if (args.Length != 1)
                        {
                            throw new Exception("tan() erwartet 1 Argument");
                        }
                        return Math.Tan(args[0]);

                    case "cot":
                        if (args.Length != 1)
                        {
                            throw new Exception("cot() erwartet 1 Argument");
                        }
                        return 1 / Math.Tan(args[0]);

                    case "sqrt":
                        if (args.Length != 1)
                        {
                            throw new Exception("sqrt() erwartet 1 Argument");
                        }
                        if (args[0] < 0)
                        {
                            throw new Exception("negative Wurzel ist nicht möglich!");
                        }
                        else
                        {
                            return Math.Sqrt(args[0]);
                        }
                    case "pow":
                        if (args.Length != 2)
                        {
                            throw new Exception("pow() erwartet 2 Argumente");
                        }
                        return Math.Pow(args[0], args[1]);

                    case "fac":
                        if (args.Length != 1)
                        {
                            throw new Exception("fac() erwartet 1 Argument");
                        }
                        return Fakul(args[0]);

                    default:
                        throw new Exception($"Unbekannte Funktion: {name}");
                }
            }
            else
            {
                switch (name.ToLower())
                {
                    case "sin":
                        if (args.Length != 1)
                        {
                            throw new Exception("sin() erwartet 1 Argument");
                        }
                        return Math.Sin(args[0] * Math.PI / 180);

                    case "cos":
                        if (args.Length != 1)
                        {
                            throw new Exception("cos() erwartet 1 Argument");
                        }
                        return Math.Cos(args[0] * Math.PI / 180);

                    case "tan":
                        if (args.Length != 1)
                        {
                            throw new Exception("tan() erwartet 1 Argument");
                        }
                        if (args[0] == 90)
                        {
                            throw new Exception("tan(90) in deg ist undefiniert");
                        }
                        return Math.Tan(args[0] * Math.PI / 180);

                    case "cot":
                        if (args.Length != 1)
                        {
                            throw new Exception("cos() erwartet 1 Argument");
                        }
                        return 1 / Math.Tan(args[0] * Math.PI / 180);

                    case "sqrt":
                        if (args.Length != 1)
                        {
                            throw new Exception("sqrt() erwartet 1 Argument");
                        }
                        if (args[0] < 0)
                        {
                            throw new Exception("negative Wurzel ist nicht möglich!");
                        }
                        else
                        {
                            return Math.Sqrt(args[0]);
                        }
                    case "pow":
                        if (args.Length != 2)
                        {
                            throw new Exception("pow() erwartet 2 Argumente");
                        }
                        return Math.Pow(args[0], args[1]);

                    case "fac":
                        if (args.Length != 1)
                        {
                            throw new Exception("fac() erwartet 1 Argument");
                        }
                        return Fakul(args[0]);
                    default:
                        throw new Exception($"Unbekannte Funktion: {name}");
                }
            }
        }
        public static double Fakul(double value)
        {
            //is int
            if (value % 1 != 0)
            {
                throw new Exception("Fakultät funktioniert nur bei ganze Zahlen.");
            }

            if (value < 0)
            {
                throw new Exception("Fakultät funktioniert nur bei positiven Zahlen.");
            }

            double result = 1;
            for (int i = 1; i <= (int)value; i++)
            {
                result *= i;
            }

            return result;
        }
    }

    class Program
    {
        public static string Calculate(string input, bool rad)
        {
            bool r = rad;
            input = input.Replace(',', '.');
            if(input == "")
            {
                return "";
            }
            try
            {
                Lexer lexer = new Lexer(input);
                List<Token> tokens = lexer.Tokenize();
                Parser parser = new Parser(tokens, r);
                double result = parser.Parse();
                return result.ToString();
            }
            catch(Exception e)
            {
                return "Fehlerhafte Eingabe\n" + e.Message.ToString();
            }

        }
    }
}