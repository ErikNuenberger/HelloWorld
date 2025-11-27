# documentation for the GUI section of the calculator

The WPF-based calculator project is composed of three main files:
- Program.cs – Contains the core calculation logic of the calculator.
- MainWindow.xaml – Contains the XAML code for the UI layout and styling.
- MainWindow.xaml.cs – The code-behind file that connects the UI with the calculation logic.

## Program.cs

The Program.cs file contains the same calculator logic used in the separate Calculator project (see Calculator/Program.cs)
It provides the basic functionality required to parse and evaluate mathematical expressions. For detailed information about the internal logic of
the calculator, please refer to the documentation of that project.
Within this WPF version, one part differs from the original Program.cs file:
- There is no Main() method. Instead, there is a method named Calculate(), which is called from the code-behind to evaluate the user input.
- The method is implemented as follows:

```C#
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
```

### Explanation

Parameters
Calculate() takes two parameters:
- input: a string representing the mathematical expression to evaluate.
- rad: a boolean indicating whether the calculator should operate in radian mode (true) or degree mode (false).

Return Value
- The result of the calculation (a double) is converted into a string so it can be passed back to the code-behind and displayed.

Error Handling
- If an error occurs during the calculation (e.g., invalid syntax), the method does not return a numeric result.
- Instead, it returns an error message as a string, which is then displayed by the code-behind.
