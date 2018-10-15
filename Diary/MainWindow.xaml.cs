﻿using Diary.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        private bool isValidEmail(string email)
        {
            var addr = new EmailAddressAttribute();
            return addr.IsValid(email);
        }
    }
}
