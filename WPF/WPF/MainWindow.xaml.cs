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
using System.IO;
using System.Diagnostics;
using System.Linq;

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
            MainUI.Visibility = Visibility.Visible;
            KidsUI.Visibility = Visibility.Collapsed;
            HistoryLabel.Visibility = Visibility.Collapsed;
            SettingsUI.Visibility = Visibility.Collapsed;
            NextButton.IsEnabled = false;
            ErrorHandlerMethod();


        }

        

        bool rad = true;
        bool kidsmode = false;
        string[] HistoryRes = new string[4];
        string[] HistoryCal = new string[4];
        int HistPos = 0;
        int ShowPos = 0;
        bool next = false;
        int errorCounter = 0;

        private void Button_Number_Click(object sender, RoutedEventArgs e)
        {
            if(!kidsmode)
            {
                Button btn = sender as Button;
                if(btn.Content.ToString() == "=")
                {
                    string val = InputBox.Text;
                    HistoryLabel.Visibility = Visibility.Visible;
                    if (HistPos >= 4)
                    {
                        HistPos = 0;
                    }
                    HistoryCal[HistPos] = val;
                    string output = Program.Calculate(val, rad);
                    if(output.Contains("Fehlerhafte Eingabe"))
                    {
                        OutputLabel.Text = "Fehlerhafte Eingabe";
                        string errorMsg = output.Substring(20);

                        string path = System.IO.Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "errorlog.log"
                        );
                        try
                        {
                            File.AppendAllText(path, "$" + errorCounter.ToString() + "\t\t"  + errorMsg + "\n");
                            errorCounter++;
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        int index = 0;
                        HistoryRes[HistPos] = "Fehler";
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
                        HistoryRes[HistPos] = output;
                        ErrorLabel.Visibility = Visibility.Collapsed;
                        ErrorText.Text = "";
                    }
                    HistPos++;
                    if(ShowPos < 4)
                    {
                        ShowPos++;
                    }
                    string histstr = "";
                    for(int i = 0; i < ShowPos; i++)
                    {
                        histstr = histstr + HistoryCal[i] + " =\n" + HistoryRes[i] + "\n\n";
                    }
                    HistoryLabel.Content = histstr;
                    HistoryLabel.Height = 60 * ShowPos + 10;
                    histstr = "";

                }
                else if (btn.Content.ToString() == "C")
                {
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    InputBox.Text = "";
                    OutputLabel.Text = "";
                    ErrorText.Text = "";
                }
                else
                {
                    string value = InputBox.Text;
                    value = value + btn.Content.ToString();
                    InputBox.Text = value;
                }
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(!kidsmode)
            {
                if(e.Key == Key.Enter)
                {
                    string val = InputBox.Text;
                    HistoryLabel.Visibility = Visibility.Visible;
                    if (HistPos >= 4)
                    {
                        HistPos = 0;
                    }
                    HistoryCal[HistPos] = val;
                    string output = Program.Calculate(val, rad);
                    if(output.Contains("Fehlerhafte Eingabe"))
                    {
                        OutputLabel.Text = "Fehlerhafte Eingabe";
                        string errorMsg = output.Substring(20);

                        string path2 = System.IO.Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "errorlog.log"
                        );
                        try
                        {
                            File.AppendAllText(path2, "$" + errorCounter.ToString() + "\t" + errorMsg + "\n");
                            errorCounter++;
                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                        HistoryRes[HistPos] = "Fehler";
                        int index = 0;
                        if(errorMsg.Length > 40)
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
                        HistoryRes[HistPos] = output;
                        ErrorLabel.Visibility = Visibility.Collapsed;
                        ErrorText.Text = "";
                    }
                    HistPos++;
                    if(ShowPos < 4)
                    {
                        ShowPos++;
                    }
                    string histstr = "";
                    for(int i = 0; i < ShowPos; i++)
                    {
                        histstr = histstr + HistoryCal[i] + " =\n" + HistoryRes[i] + "\n\n";
                    }
                    HistoryLabel.Content = histstr;
                    HistoryLabel.Height = 60 * ShowPos + 10;
                    histstr = "";
                }
            }
            else
            {
                if (int.TryParse(AnswerBox.Text, out int userAnswer))
                {
                    if (userAnswer == currentAnswer)
                    {
                        FeedbackText.Text = "🎉 Glückwunsch!";
                        FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(0, 180, 0));
                        next = true;
                        NextButton.IsEnabled = true;
                    }
                    else
                    {
                        FeedbackText.Text = "❌ Versuch es nochmal!";
                        FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                    }
                }
                else
                {
                    FeedbackText.Text = "Bitte eine Zahl eingeben!";
                    FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                    NextButton.IsEnabled = false;
                }
            }
        }


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

        private void KidsModeToggle_Unchecked(Object sender, RoutedEventArgs e)
        {
            KidsModeToggle.Content = "Kindermodus";
            kidsmode = false;
            MainUI.Visibility = Visibility.Visible;
            KidsUI.Visibility = Visibility.Collapsed;
        }

        private void KidsModeToggle_Checked(Object sender, RoutedEventArgs e)
        {
            KidsModeToggle.Content = "normaler Modus";
            kidsmode = true;
            MainUI.Visibility = Visibility.Collapsed;
            KidsUI.Visibility = Visibility.Visible;
            GenerateTask();
        }

        Random rnd = new Random();
        int currentAnswer = 0;

        private void GenerateTask()
        {
            // Operation wählen
            string[] operations = { "+", "-", "*" };
            string op = operations[rnd.Next(operations.Length)];

            int a = rnd.Next(1, 10);
            int b = rnd.Next(1, 10);
    

            TaskText.Text = $"{a} {op} {b} = ?";
            currentAnswer = op switch
            {
                "+" => a + b,
                "-" => a - b,
                "*" => a * b,
                _ => 0
            };

            GenerateSymbols(a, b, op);
            FeedbackText.Text = "";
            AnswerBox.Text = "";
        }

        private void GenerateSymbols(int a, int b, string op)
        {
            SymbolPanel.Children.Clear();

            // Pfad zum Bild (relative Pfadangabe innerhalb deines Projekts)
            string appleImagePath = "Images/Apple.png";

            for (int i = 0; i < a; i++)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(appleImagePath, UriKind.Relative));
                img.Width = 32;   // Größe des Symbols
                img.Height = 32;
                img.Margin = new Thickness(2);
                SymbolPanel.Children.Add(img);
            }

            // Operator symbol als TextBlock
            SymbolPanel.Children.Add(new TextBlock { Text = $" {op} ", FontSize = 32, Margin = new Thickness(5, 0, 5, 0), Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43)) });

            for (int i = 0; i < b; i++)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(appleImagePath, UriKind.Relative));
                img.Width = 32;
                img.Height = 32;
                img.Margin = new Thickness(2);
                SymbolPanel.Children.Add(img);
            }
        }



        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if(int.TryParse(AnswerBox.Text, out int userAnswer))
            {
                if(userAnswer == currentAnswer)
                {
                    FeedbackText.Text = "🎉 Glückwunsch!";
                    FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(0, 180, 0));
                    next = true;
                    NextButton.IsEnabled = true;
                }
                else
                {
                    FeedbackText.Text = "❌ Versuch es nochmal!";
                    FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                }
            }
            else
            {
                FeedbackText.Text = "Bitte eine Zahl eingeben!";
                FeedbackText.Foreground = new SolidColorBrush(Color.FromRgb(192, 57, 43));
                NextButton.IsEnabled= false;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if(next)
            {
                GenerateTask();
                next = false;
                NextButton.IsEnabled = false;
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Kleines 1x1:\n");

            for(int i = 1; i <= 10; i++)
            {
                for(int j = 1; j <= 10; j++)
                {
                    sb.Append($"{i}×{j}={i * j}\t");
                }
                sb.AppendLine();
            }

            MessageBox.Show(sb.ToString(), "Hilfe: Kleines 1x1", MessageBoxButton.OK);
        }


        private void Settings_Button(object sender, RoutedEventArgs e)
        {
            MainUI.Visibility = Visibility.Collapsed;
            KidsUI.Visibility = Visibility.Collapsed;
            SettingsUI.Visibility = Visibility.Visible;
        }

        private void ApplyFontSettings_Click(object sender, RoutedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem is ComboBoxItem fontItem)
            {
                string fontName = fontItem.Content.ToString();
                InputBox.FontFamily = new FontFamily(fontName);
                OutputLabel.FontFamily = new FontFamily(fontName);
            }

            if (FontSizeComboBox.SelectedItem is ComboBoxItem sizeItem)
            {
                if (double.TryParse(sizeItem.Content.ToString(), out double size))
                {
                    InputBox.FontSize = size;
                    OutputLabel.FontSize = size;
                }
            }
        }


        private void Back(object sender, RoutedEventArgs e)
        {
            MainUI.Visibility = Visibility.Visible;
            KidsUI.Visibility = Visibility.Collapsed;
            SettingsUI.Visibility = Visibility.Collapsed;
        }

        public void ErrorHandlerMethod()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorlog.log");

            errorCounter = 0;

            try
            {
                string lastLine = File.ReadLines(path).Reverse().FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

                if(lastLine == null)
                {
                    errorCounter = 0;
                    return;
                }

                string str_sub = lastLine.Substring(1);
                int indexing = 0;
                while(str_sub[indexing] != '\t')
                {
                    indexing++;
                }
                Debug.WriteLine(str_sub.Substring(0, indexing));
                errorCounter = int.Parse(str_sub.Substring(0, indexing));
                errorCounter++;
            }

            catch(Exception ex)
            {
                Debug.WriteLine("Fehler beim Initialisieren des ErrorCounters:");
                Debug.WriteLine(ex.ToString());
                errorCounter = 0;
            }

            //delete log when over 100 messages
            if(errorCounter > 100)
            {
                File.WriteAllText(path, string.Empty);
                errorCounter = 0;
            }
        }


    }
    

}