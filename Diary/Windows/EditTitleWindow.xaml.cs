using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
using WinForms = System.Windows.Forms;

namespace Diary.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditTitleWindow.xaml
    /// </summary>
    public partial class EditTitleWindow : Window
    {
        private int titleId;
        SqlConnection connection;
        List<Image> imagesList = new List<Image>();
        int imageId = 0;
        bool imageAdded;
        TitlesListWindow TitlesListWindow;

        public EditTitleWindow(TitlesListWindow titlesList, int title_id)
        {
            InitializeComponent();
            this.titleId = title_id;
            TitlesListWindow = titlesList;
            fillWindow();
        }

        private void fillWindow()
        {
            //try
            //{
            connection = new SqlConnection(MainWindow.connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT title, text, date, have_image FROM Titles WHERE id = @title_id", connection);
            command.Parameters.AddWithValue("@title_id", titleId);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                // Если запись без фотографий
                if (Convert.ToInt32(reader["have_image"]) == 0)
                {
                    string title = Convert.ToString(reader["title"]);
                    string text = Convert.ToString(reader["text"]);
                    DateTime date = Convert.ToDateTime(reader["date"]);
                    TBdate.Text = date.ToString();
                    TBTitle.Text = title;
                    TBText.Text = text;
                }
                else
                {
                    string title = Convert.ToString(reader["title"]);
                    string text = Convert.ToString(reader["text"]);
                    DateTime date = Convert.ToDateTime(reader["date"]);

                    SqlCommand command2 = new SqlCommand("SELECT image FROM Images WHERE title_id = @title_id", connection);
                    command2.Parameters.AddWithValue("@title_id", titleId);
                    SqlDataReader imageReader = command2.ExecuteReader();

                    TBdate.Text = date.ToString();
                    TBTitle.Text = title;
                    TBText.Text = text;

                    this.Height = 670;
                    GridImage.Visibility = Visibility.Visible;

                    while (imageReader.Read())
                    {
                        var byteArray = (byte[])imageReader["image"];
                        Image image = new Image();
                        image.Source = setImgSourceFromArray(byteArray);
                        imagesList.Add(image);
                    }
                    if (imagesList.Count > 1)
                        SPcontrol.Visibility = Visibility.Visible;
                    imageReader.Close();
                    ImgAdded.Source = imagesList.First().Source;
                }
            }
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
            //finally { if (connection != null) connection.Close(); }
        }

        private BitmapImage setImgSourceFromArray(byte[] imageData)
        {
            var bitmapImage = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.UriSource = null;
                bitmapImage.StreamSource = mem;
                bitmapImage.EndInit();
            }
            bitmapImage.Freeze();
            return bitmapImage;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
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
                        SqlCommand command = new SqlCommand("UPDATE Titles SET title = @title, text = @text, have_image = 1 WHERE id = @title_id", connection);
                        command.Parameters.AddWithValue("@title_id", titleId);
                        command.Parameters.AddWithValue("@title", TBTitle.Text);
                        command.Parameters.AddWithValue("@text", TBText.Text);
                        command.ExecuteNonQuery();
                        command = null;
                        command = new SqlCommand("DELETE FROM Images WHERE title_id = @title_id", connection);
                    command.Parameters.AddWithValue("@title_id",titleId);
                        command.ExecuteNonQuery();

                        foreach (var image in imagesList)
                        {
                            command = new SqlCommand("INSERT INTO Images VALUES (@title_id, @image)", connection);
                            command.Parameters.AddWithValue("@title_id", titleId);
                            command.Parameters.AddWithValue("@image", ImageSourceToBytes(image.Source));
                            command.ExecuteNonQuery();
                        }
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    finally { if (connection != null) connection.Close(); }
                }
                // Если фотографий нет
                else
                {
                    try
                    {
                        connection = new SqlConnection(MainWindow.connectionString);
                        connection.Open();
                        SqlCommand command = new SqlCommand("UPDATE Titles SET title = @title, text = @text, have_image = 0 WHERE id = @title_id", connection);
                        command.Parameters.AddWithValue("@title_id", titleId);
                        command.Parameters.AddWithValue("@title", TBTitle.Text);
                        command.Parameters.AddWithValue("@text", TBText.Text);
                        command.ExecuteNonQuery();
                        command = null;
                        command = new SqlCommand("DELETE FROM Images WHERE title_id = @title_id", connection);
                        command.Parameters.AddWithValue("@title_id", titleId);
                        command.ExecuteNonQuery();
                        this.Close();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { if (connection != null) connection.Close(); }
                }
            } // Если поля не заполнены
            else MessageBox.Show("Заполните текстовые поля!");
        }

        public byte[] ImageSourceToBytes(ImageSource imageSource)
        {
            byte[] bytes = null;
            var bitmapSource = imageSource as BitmapSource;

            if (bitmapSource != null)
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        private void controlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button controlBtn = (Button)sender;
                switch (controlBtn.Name)
                {
                    case "BtnBackImg":
                        if (imageId == 0)
                        {
                            imageId = imagesList.Count - 1;
                            ImgAdded.Source = imagesList[imageId].Source;
                        }
                        else
                        {
                            imageId -= 1;
                            ImgAdded.Source = imagesList[imageId].Source;
                        }
                        break;

                    case "BtnNextImg":
                        if (imageId == imagesList.Count - 1)
                        {
                            imageId = 0;
                            ImgAdded.Source = imagesList[imageId].Source;
                        }
                        else
                        {
                            imageId += 1;
                            ImgAdded.Source = imagesList[imageId].Source;
                        }
                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                    ImgAdded.Source = imagesList[imageId].Source;
                }
                else { ImgAdded.Source = imagesList[imageId].Source; }

                if (imagesList.Count == 1)
                {
                    SPcontrol.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void setImgSource(Image image, String path)
        {
            try
            {
                image.Source = createBitmapFromPath(path);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private BitmapImage createBitmapFromPath(String path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path);
            bitmap.EndInit();
            return bitmap;
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
                    foreach (var path in dialog.FileNames.Reverse())
                    {
                        Image image = new Image(); 
                        image.Source = createBitmapFromPath(path);
                        imagesList.Add(image);
                    }
                    
                    if (imagesList.Count > 1)
                        SPcontrol.Visibility = Visibility.Visible;
                    ImgAdded = imagesList.First();
                    imageId = 0;
                    imageAdded = true;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }

            if (imageAdded)
            {
                this.Height = 670;
                GridImage.Visibility = Visibility.Visible;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TitlesListWindow.EditTitleWindow = null;
            TitlesListWindow.updateList();
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
            if (String.IsNullOrEmpty(textBox.Text) || String.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Текст записи...")
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
    }
}
