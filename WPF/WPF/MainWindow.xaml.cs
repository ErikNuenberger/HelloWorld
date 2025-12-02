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
        bool rad = true;

        private void Button_Number_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content.ToString() == "=")
            {
                string val = InputBox.Text;
                string output = Program.Calculate(val, rad);
                if (output.Contains("Fehlerhafte Eingabe"))
                {
                    OutputLabel.Text = "Fehlerhafte Eingabe";
                    string errorMsg = output.Substring(20);
                    int index = 0;
                    if (errorMsg.Length > 40)
                    {
                        for (int i = errorMsg.Length - 1; i > 40; i--)
                        {
                            if (errorMsg[i] == ' ')
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
                else
                {
                    OutputLabel.Text = output;
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    ErrorText.Text = "";
                }

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

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string val = InputBox.Text;
                string output = Program.Calculate(val, rad);
                if (output.Contains("Fehlerhafte Eingabe"))
                {
                    OutputLabel.Text = "Fehlerhafte Eingabe";
                    string errorMsg = output.Substring(20);
                    int index = 0;
                    if (errorMsg.Length > 40)
                    {
                        for (int i = errorMsg.Length - 1; i > 40; i--)
                        {
                            if (errorMsg[i] == ' ')
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
                else
                {
                    OutputLabel.Text = output;
                    ErrorLabel.Visibility = Visibility.Collapsed;
                    ErrorText.Text = "";
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

    }
}