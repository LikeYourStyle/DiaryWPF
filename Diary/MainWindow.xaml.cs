using Diary.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diary
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection connection;
        private static string projectDir = Environment.CurrentDirectory;
        //public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + projectDir + @"\Database.mdf;Integrated Security=True";
        public static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB; 
            AttachDbFilename=C:\Users\User\OneDrive\Учебная фигня\op\Diary\Diary\Database.mdf;
            Integrated Security=True; MultipleActiveResultSets=True";
        public static int id;

        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegistrWindow().Show();
            this.Close();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            String login;
            String pass;
            try
            {
                if (isValidEmail(TBmail.Text))
                {
                    login = TBmail.Text;
                    if (TBpass.Visibility == Visibility.Visible)
                        pass = TBpass.Password;
                    else
                        pass = TBpassShowed.Text;


                    if (LogInUser(login, pass))
                    {
                        new TitlesListWindow().Show();
                        this.Close();
                    }
                }
                else
                {
                    BorderMail.BorderBrush = Brushes.Red;
                    TBmail.Focus();
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool LogInUser(String login, String password)
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT id FROM Users WHERE login = @login AND password = @pass", connection);
                command.Parameters.AddWithValue("login", login);
                command.Parameters.AddWithValue("pass", password);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    id = Convert.ToInt32(reader["id"]);
                    return true;
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void BtnShowPass_Click(object sender, RoutedEventArgs e)
        {
            if ((String)BtnShowPass.Content == "ПОКАЗАТЬ")
            {
                TBpass.Visibility = Visibility.Collapsed;
                TBpassShowed.Visibility = Visibility.Visible;
                TBpassShowed.Text = TBpass.Password;
                BtnShowPass.Content = "СКРЫТЬ";
            }
            else
            {
                TBpassShowed.Visibility = Visibility.Collapsed;
                TBpass.Visibility = Visibility.Visible;
                TBpass.Password = TBpassShowed.Text;
                BtnShowPass.Content = "ПОКАЗАТЬ";
            }
                
        }

        static public bool isValidEmail(string email)
        {
            var addr = new EmailAddressAttribute();
            return addr.IsValid(email);
        }

        private void TBmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (MainWindow.isValidEmail(TBmail.Text)) { BorderMail.BorderBrush = Brushes.White; }
            else { BorderMail.BorderBrush = Brushes.Red; }
        }
    }
}
