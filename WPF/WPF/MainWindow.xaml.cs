using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
            if(btn.Content.ToString() == "=")
            {
                string val = InputBox.Text;
                OutputLabel.Content = Program.Calculate(val, rad);

            }
            else if(btn.Content.ToString() == "C")
            {
                InputBox.Text = "";
                OutputLabel.Content = "";
            }
            else
            {
                string value = InputBox.Text;
                value = value + btn.Content.ToString();
                InputBox.Text = value;
            }
        }
        private void AngleToggle_Checked(object sender, RoutedEventArgs e)
        {
            rad = false;
        }

        private void AngleToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            rad = true;
        }

    }
}