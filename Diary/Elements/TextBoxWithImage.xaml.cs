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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diary.Elements
{
    /// <summary>
    /// Логика взаимодействия для TextBoxWithImage.xaml
    /// </summary>
    public partial class TextBoxWithImage : UserControl
    {
        public TextBoxWithImage()
        {
            InitializeComponent();
        }

        public ImageSource textBoxImage
        {
            get { return TextBoxImage.Source; }
            set { TextBoxImage.Source = value; }
        }

        public String GetTextBox
        {
            get { return TextBox.Text; }
            set { TextBox.Text = value; }
        }

        public Brush brush
        {
           get { return MailTextBoxBrush.BorderBrush; }
            set { MailTextBoxBrush.BorderBrush = value; }
        }
    }
}
