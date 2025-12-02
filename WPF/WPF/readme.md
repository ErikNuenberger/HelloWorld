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


here is the complete code for Program.cs:
```C#
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
```

---

The MainWindow.xaml.cs file contains the C# code that connects the WPF user interface with the calculator logic implemented in Program.cs.
It performs three main tasks:
1. Receiving user input from the UI (button presses, text input, key events).
2. Passing the input expression to Program.Calculate() for evaluation.
3. Displaying either the result or an error message back in the UI.

Below is the complete code-behind file:
```C#
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Taschenrechner_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //to set the calculator rad or deg mode
        bool rad = true;


        //event of a button click
        private void Button_Number_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            //when the = button was clicked:
            if (btn.Content.ToString() == "=")
            {
                //get input from input label and perform calculation
                string val = InputBox.Text;
                string output = Program.Calculate(val, rad);
                //check if output is an error
                if(output.Contains("Fehlerhafte Eingabe"))
                {
                    OutputLabel.Text = "Fehlerhafte Eingabe";
                    string errorMsg = output.Substring(20);
                    int index = 0;
                    if (errorMsg.Length > 40)
                    {
                        for(int i = errorMsg.Length - 1; i > 40; i--)
                        {
                            if(errorMsg[i] == ' ')
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    if (index > 0)
                    {
                        errorMsg = errorMsg.Insert(index, "\n");
                    }

                    ErrorText.Text = errorMsg;
                    ErrorLabel.Visibility = Visibility.Visible;
                }
                //if output is not an error -> normal output = result of calculation
                else
                {
                    OutputLabel.Text = output;
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    ErrorText.Text = "";
                }

            }
            //if the C-button (Clear) is pressed, clear everything
            else if(btn.Content.ToString() == "C")
            {
                ErrorLabel.Visibility = Visibility.Collapsed;
                InputBox.Text = "";
                OutputLabel.Text = "";
                ErrorText.Text = "";
            }
            //if any other button is pressed, it is an input button, that adds a new symbol to the equation
            //e.g. + is pressed, add + symbol to the end of the equation
            else
            {
                string value = InputBox.Text;
                value = value + btn.Content.ToString();
                InputBox.Text = value;
            }
        }

        //instead of pressing the = button, you can just hit enter to perform a calculation
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string val = InputBox.Text;
                string output = Program.Calculate(val, rad);
                if (output.Contains("Fehlerhafte Eingabe"))
                {
                    OutputLabel.Text = "Fehlerhafte Eingabe";
                    string errorMsg = output.Substring(20);
                    int index = 0;
                    if(errorMsg.Length > 40)
                    {
                        for(int i = errorMsg.Length - 1; i > 40 ; i--)
                        {
                            if(errorMsg[i] == ' ')
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    if(index > 0)
                    {
                        errorMsg = errorMsg.Insert(index, "\n");
                    }

                    ErrorText.Text = errorMsg;
                    ErrorLabel.Visibility = Visibility.Visible;
                }
                else
                {
                    OutputLabel.Text = output;
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    ErrorText.Text = "";
                }
            }
        }

        //for the button that sets rad/deg mode
        private void AngleToggle_Checked(object sender, RoutedEventArgs e)
        {
            rad = false;
            AngleToggle.Content = "DEG";
        }

        private void AngleToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            rad = true;
            AngleToggle.Content = "RAD";
        }

    }
}
```

---

The MainWindow.xaml file defines the full user interface, including:
- The layout of input fields, output area, and buttons
- Styling for buttons, toggle buttons, text boxes, and labels
- Color animations for user feedback
- A two-column layout (input on the left, output on the right)

Below is the XAML code:
```xaml
<Window x:Class="Taschenrechner_WPF.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Taschenrechner_WPF"
        mc:Ignorable="d"
        Title="Taschenrechner"
		Height="450"
		Width="950"
		Background="#1D2129"
		FontFamily="Helvetica Neue"
		Icon="C:\Users\nuernberger\source\repos\Bild1.png">
	<!-- BorderBrush="#5B677F" Background="#2E3440" Foreground="#FFFFFF" BorderBrush="#5B677F" Padding="20" -->
	<Window.Resources>
		<Style TargetType="Button">
			<Setter Property="Background" Value="#424B5C"/>
			<Setter Property="BorderBrush" Value="#424B5C"/>
			<Setter Property="Foreground" Value="#FFFFFF"/>
			<Setter Property="Padding" Value="6"/>
			<Setter Property="Margin" Value="6"/>
			
			<Setter Property="FontSize" Value="22"/>
			<Setter Property="Template">
				<Setter.Value>
					<ControlTemplate TargetType="Button">
						<Border x:Name="border"
								Background="{TemplateBinding Background}"
								BorderBrush="{TemplateBinding BorderBrush}"
								BorderThickness="{TemplateBinding BorderThickness}"
								CornerRadius="8">
							<ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"  Margin="{TemplateBinding Padding}"/>
						</Border>
						<ControlTemplate.Triggers>
							<Trigger Property="IsMouseOver" Value="True">
								<Setter TargetName="border" Property="Background" Value="#5B677F"/>
								<Setter TargetName="border" Property="BorderBrush" Value="#5B677F"/>
								<Setter Property="Margin" Value="1"/>
								<Setter  Property="Padding" Value="9"/>
							</Trigger>
						<EventTrigger RoutedEvent="Button.Click">
								<BeginStoryboard>
									<Storyboard>
										<ColorAnimation
											Storyboard.TargetName="border"
											Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)"
											To="#E6968E"
											Duration="0:0:0.10"
											AutoReverse="True"/>
									</Storyboard>
								</BeginStoryboard>
							</EventTrigger>
                </ControlTemplate.Triggers>
						
					</ControlTemplate>
				</Setter.Value>
			</Setter>
		</Style>

		<Style TargetType="ToggleButton">
			<!-- Standard -->
			<Setter Property="Background" Value="#C0392B"/>
			<Setter Property="BorderBrush" Value="#C0392B"/>
			<Setter Property="Foreground" Value="#FFFFFF"/>
			<Setter Property="Padding" Value="5"/>

			<Setter Property="FontSize" Value="22"/>
			<Setter Property="Template">
				<Setter.Value>
					<ControlTemplate TargetType="ToggleButton">
						<Border x:Name="border"
								Background="{TemplateBinding Background}"
								BorderBrush="{TemplateBinding BorderBrush}"
								BorderThickness="{TemplateBinding BorderThickness}"
								CornerRadius="8">
							<ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center" Margin="{TemplateBinding Padding}"/>
						</Border>
						<ControlTemplate.Triggers>
							<Trigger Property="IsMouseOver" Value="True">
								<Setter TargetName="border" Property="Background" Value="#DB675B"/>
								<Setter TargetName="border" Property="BorderBrush" Value="#DB675B"/>
							</Trigger>

							<EventTrigger RoutedEvent="Button.Click">
								<BeginStoryboard>
									<Storyboard>
										<ColorAnimation
											Storyboard.TargetName="border"
											Storyboard.TargetProperty="(Border.Background).(SolidColorBrush.Color)"
											To="#E6968E"
											Duration="0:0:0.10"
											AutoReverse="True"/>
									</Storyboard>
								</BeginStoryboard>
							</EventTrigger>
						</ControlTemplate.Triggers>
					</ControlTemplate>
				</Setter.Value>
			</Setter>
		</Style>
	</Window.Resources>



	<Grid Background="#1D2129" Margin="20">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="5*" />
            <ColumnDefinition Width="4*" />
        </Grid.ColumnDefinitions>

        <!-- Left side (Input + Buttons) -->
        <Grid Grid.Column="0">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <!-- INPUT label -->
            <TextBlock Background="#1D2129" Foreground="#FFFFFF" FontSize="25" Text="Eingabe" FontWeight="Bold" Margin="0,0,0,5"/>

            <!-- Input TextBox -->
            <TextBox Background="#5B677F" Padding="5" Foreground="#FFFFFF" BorderBrush="#5B677F" Grid.Row="1" Name="InputBox" Height="42" FontSize="26" KeyDown="InputBox_KeyDown"/>

            <!-- Button Grid -->
            <Grid Grid.Row="2" Margin="0, 10, 0, 0">
                
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>

                <Grid.ColumnDefinitions>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                    <ColumnDefinition/>
                </Grid.ColumnDefinitions>

                <!-- Row 1 -->
				<Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="0" Content="7"/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="1" Content="8"/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="2" Content="9"/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="3" Content="+"/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="4" Content="PI"/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="5" Content="sin("/>
                <Button Click="Button_Number_Click" Grid.Row="0" Grid.Column="6" Content="fac("/>

                <!-- Row 2 -->
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="0" Content="4"/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="1" Content="5"/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="2" Content="6"/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="3" Content="-"/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="4" Content="e"/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="5" Content="cos("/>
				<Button Click="Button_Number_Click" Grid.Row="1" Grid.Column="6" Content="sqrt("/>
                
				<!-- Row 3 -->
                <Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="0" Content="1"/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="1" Content="2"/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="2" Content="3"/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="3" Content="*"/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="4" Content="("/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="5" Content="tan("/>
				<Button Click="Button_Number_Click" Grid.Row="2" Grid.Column="6" Content="pow("/>

				<!-- Row 4 -->
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="0" Content="0"/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="1" Content=","/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="2" Content=";"/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="3" Content="/"/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="4" Content=")"/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="5" Content="cot("/>
				<Button Click="Button_Number_Click" Grid.Row="3" Grid.Column="6" Content="C"/>
				
            </Grid>

            <!-- Toggle row -->
            <StackPanel Grid.Row="3" Margin="0,10,0,0" Orientation="Horizontal">
				<ToggleButton x:Name="AngleToggle" Content="RAD" Checked="AngleToggle_Checked" Unchecked="AngleToggle_Unchecked" Width="100" Height="45">
				</ToggleButton>
            </StackPanel>
        </Grid>

        <!-- Right side (Output) -->
        <StackPanel Grid.Column="1" Margin="10,0,0,0">
            <TextBlock Foreground="#FFFFFF" FontSize="25" Text="Ergebnis:" FontWeight="Bold" Margin="0,0,0,5"/>
			<TextBox Name="OutputLabel" BorderBrush="#5B677F" Height="42" FontSize="26" Background="#5B677F" Foreground="#FFFFFF" Padding="5"/>
			<Label x:Name="ErrorLabel"
			   Background="#5B677F"
			   Foreground="#FFFFFF"
			   FontSize="16"
			   Padding="5"
			   Margin="0,0,0,0"
			   Height="64"
			   HorizontalContentAlignment="Left"
			   VerticalContentAlignment="Top"
			Visibility="Collapsed">
				<TextBlock x:Name="ErrorText" TextWrapping="Wrap"/>
			</Label>
            <Button Click="Button_Number_Click" FontSize="24" FontWeight="Bold" Content="=" Height="45" Margin="0,15,0,0"/>
			
        </StackPanel>
	

    </Grid>
</Window>

```
