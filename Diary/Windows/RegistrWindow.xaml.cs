using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        SqlConnection connection;

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
            String pass;
            String passAgain;
            String login;
            try
            {
                login = TBmail.Text;
                pass = TBpass.Password;
                passAgain = TBpass.Password;
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
                return;
            }
            if (pass == passAgain && MainWindow.isValidEmail(login))
            {
                CreateUser(login, pass);
            }
        }

        private async void CreateUser(String login, String pass)
        {
            try
            {
                connection = new SqlConnection(MainWindow.connectionString);
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand("INSERT INTO Users Values(@login, @password)", connection);
                command.Parameters.AddWithValue("login", login);
                command.Parameters.AddWithValue("password", pass);
                await command.ExecuteNonQueryAsync();
                MessageBox.Show("Успешная регистрация!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                new MainWindow().Show();
                this.Close();
            }
            catch (System.Data.Common.DbException)
            {
                MessageBox.Show("Такой логин уже используется", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
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
                BorderMail.BorderBrush = Brushes.White;
                textBox.Text = "Имя пользователя";
            }
            else
            {
                if (MainWindow.isValidEmail(TBmail.Text)){ BorderMail.BorderBrush = Brushes.White; }
                    else { BorderMail.BorderBrush = Brushes.Red; }
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
    }
}
