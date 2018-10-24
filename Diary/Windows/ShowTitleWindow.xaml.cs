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

namespace Diary.Windows
{
    /// <summary>
    /// Логика взаимодействия для ShowTitleWindow.xaml
    /// </summary>
    public partial class ShowTitleWindow : Window
    {
        private int titleId;
        SqlConnection connection;
        List<byte[]> imagesList = new List<byte[]>();
        int imageId = 0;
        TitlesListWindow TitlesListWindow;

        public ShowTitleWindow(TitlesListWindow listWindow, int title_Id)
        {
            InitializeComponent();
            this.titleId = title_Id;
            TitlesListWindow = listWindow;
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

                            imagesList.Add(byteArray);
                        }
                    if (imagesList.Count > 1)
                        SPcontrol.Visibility = Visibility.Visible;
                        imageReader.Close();
                        setImgSourceFromArray(imagesList.First());
                    }
                }
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
            //finally { if (connection != null) connection.Close(); }
        }

        private void setImgSourceFromArray(byte[] imageData)
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
            ImgAdded.Source = bitmapImage;
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
                            setImgSourceFromArray(imagesList[imageId]);
                        }
                        else
                        {
                            imageId -= 1;
                            setImgSourceFromArray(imagesList[imageId]);
                        }
                        break;

                    case "BtnNextImg":
                        if (imageId == imagesList.Count - 1)
                        {
                            imageId = 0;
                            setImgSourceFromArray(imagesList[imageId]);
                        }
                        else
                        {
                            imageId += 1;
                            setImgSourceFromArray(imagesList[imageId]);
                        }
                        break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            TitlesListWindow.ShowTitleWindow = null;
        }
    }
}
