using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Diary.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrWindow.xaml
    /// </summary>
    public partial class RegistrWindow : Window
    {
        MainWindow MainWindow;
        public RegistrWindow()
        {
            InitializeComponent();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TBmail_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Имя пользователя")
            {
                textBox.Foreground = Brushes.Black;
                textBox.Text = String.Empty;
            }
        }

        private void TBmail_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Имя пользователя" || String.IsNullOrEmpty(textBox.Text) || String.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;
                textBox.Text = "Имя пользователя";
            }
        }

        private void TBhint_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Visibility = Visibility.Collapsed;
            TBpass.Visibility = Visibility.Visible;
            TBpass.Focus();
        }

        private void TBpass_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (String.IsNullOrEmpty(passwordBox.Password) || String.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Visibility = Visibility.Collapsed;
                TBhint.Visibility = Visibility.Visible;
            }

        }

        private void TBhintAgain_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Visibility = Visibility.Collapsed;
            TBpassAgain.Visibility = Visibility.Visible;
            TBpassAgain.Focus();
        }

        private void TBpassAgain_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (String.IsNullOrEmpty(passwordBox.Password) || String.IsNullOrWhiteSpace(passwordBox.Password))
            {
                passwordBox.Visibility = Visibility.Collapsed;
                TBhintAgain.Visibility = Visibility.Visible;
            }
        }
    }
}
