using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.IO;

namespace Diary.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewTitleWindow.xaml
    /// </summary>
    public partial class NewTitleWindow : Window
    {
        TitlesListWindow TitlesListWindow;
        bool imageAdded = false;
        List<String> imagesList = new List<String>();
        int imageId = 0;
        int titleId;
        SqlConnection connection;

        public NewTitleWindow(TitlesListWindow titlesListWindow)
        {
            InitializeComponent();
            TitlesListWindow = titlesListWindow;
            TBdate.Text = DateTime.Now.ToString();
        }

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            WinForms.OpenFileDialog dialog = new WinForms.OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            dialog.Title = "Выберите изображение";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                try
                {
                    imagesList.AddRange(dialog.FileNames.Reverse());
                    if (imagesList.Count > 1)
                        SPcontrol.Visibility = Visibility.Visible;
                    setImgSource(ImgAdded, imagesList.First());
                    imageId = 0;
                    imageAdded = true;
                }
                catch (Exception ex){ MessageBox.Show(ex.Message); }
            }

            if (imageAdded)
            {
                this.Height = 670;
                GridImage.Visibility = Visibility.Visible;
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            TitlesListWindow.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TitlesListWindow.NewTitleWindow = null;
            TitlesListWindow.updateList();
            TitlesListWindow.updateTitlesCount();
            TitlesListWindow.Focus();
        }

        private void BtnDeleteImg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imagesList.RemoveAt(imageId);

                if (imagesList.Count <= 0)
                {
                    imagesList.Clear();
                    GridImage.Visibility = Visibility.Collapsed;
                    imageAdded = false;
                    this.Height = 450;
                    return;
                }

                if (imageId == imagesList.Count)
                {
                    imageId -= 1;
                    setImgSource(ImgAdded, imagesList[imageId]);
                }
                else { setImgSource(ImgAdded, imagesList[imageId]); }

                if (imagesList.Count == 1)
                {
                    SPcontrol.Visibility = Visibility.Collapsed;
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void setImgSource(Image image, String path)
        {
            try
            {
                image.Source = createBitmapFromPath(path);
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private BitmapImage createBitmapFromPath(String path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
        }

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                Button controlBtn = (Button)sender;
                switch (controlBtn.Name)
                {
                    case "BtnBackImg":
                        if (imageId == 0)
                        {
                            imageId = imagesList.Count - 1;
                            setImgSource(ImgAdded, imagesList[imageId]);
                        }
                        else
                        {
                            imageId -= 1;
                            setImgSource(ImgAdded, imagesList[imageId]);
                        }
                        break;

                    case "BtnNextImg":
                        if (imageId == imagesList.Count - 1)
                        {
                            imageId = 0;
                            setImgSource(ImgAdded, imagesList[imageId]);
                        }
                        else
                        {
                            imageId += 1;
                            setImgSource(ImgAdded, imagesList[imageId]);
                        }
                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void TBText_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Foreground = Brushes.Black;
            if (textBox.Text == "Заголовок записи..." || textBox.Text == "Текст записи...")
                textBox.Text = String.Empty;
        }

        private void TBText_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (String.IsNullOrEmpty(textBox.Text) || String.IsNullOrWhiteSpace(textBox.Text) ||  textBox.Text == "Текст записи...")
            {
                textBox.Text = "Текст записи...";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void TBTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (String.IsNullOrEmpty(textBox.Text) || String.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Заголовок записи...")
            {
                textBox.Text = "Заголовок записи...";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            // Если все текстовые поля заполнены
            if (!String.IsNullOrEmpty(TBText.Text) && !String.IsNullOrWhiteSpace(TBText.Text) && TBText.Text != "Текст записи..."
                && !String.IsNullOrEmpty(TBTitle.Text) && !String.IsNullOrWhiteSpace(TBTitle.Text) && TBTitle.Text != "Заголовок записи...")
            {
                // Если пользователь прикрепил фото
                if (imagesList.Count > 0)
                {
                    try
                    {
                        connection = new SqlConnection(MainWindow.connectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("INSERT INTO Titles VALUES (@userId, @title, @text, @date, @have_image)", connection);
                        command.Parameters.AddWithValue("@userId", MainWindow.id);
                        command.Parameters.AddWithValue("@title", TBTitle.Text);
                        command.Parameters.AddWithValue("@text", TBText.Text);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@have_image", 1);
                        command.ExecuteNonQuery();
                        command = null;
                        command = new SqlCommand("SELECT MAX(id) as id FROM Titles" , connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                            titleId = Convert.ToInt32(reader["id"]);
                        reader.Close();
                        foreach (var imagePath in imagesList)
                        {
                            command = new SqlCommand("INSERT INTO Images VALUES (@title_id, @image)", connection);
                            command.Parameters.AddWithValue("@title_id", titleId);
                            command.Parameters.AddWithValue("@image", File.ReadAllBytes(imagePath));
                            command.ExecuteNonQuery();
                        }
                        this.Close();
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);

                    }
                    finally { if (connection != null) connection.Close(); }
                }
                // Если фотографий нет
                else
                {
                    try {
                        connection = new SqlConnection(MainWindow.connectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("INSERT INTO Titles VALUES (@userId, @title, @text, @date, @have_image)", connection);
                        command.Parameters.AddWithValue("@userId", MainWindow.id);
                        command.Parameters.AddWithValue("@title", TBTitle.Text);
                        command.Parameters.AddWithValue("@text", TBText.Text);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@have_image", 0);
                        command.ExecuteNonQuery();
                        this.Close();
                    } catch (Exception ex) { MessageBox.Show(ex.Message); } finally { if (connection != null) connection.Close(); }
                }
            } // Если поля не заполнены
            else MessageBox.Show("Заполните текстовые поля!");
        }
    }
}
