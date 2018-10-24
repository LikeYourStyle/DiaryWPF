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
    /// Логика взаимодействия для TitlesListWindow.xaml
    /// </summary>
    public partial class TitlesListWindow : Window
    {
        public NewTitleWindow NewTitleWindow;
        public ShowTitleWindow ShowTitleWindow;
        public EditTitleWindow EditTitleWindow;
        SqlConnection connection;
        SortedDictionary<int, int> id_titleId_Map = new SortedDictionary<int, int>(); 

        public TitlesListWindow()
        {
            InitializeComponent();
            updateList();
            updateTitlesCount();
            setWelcomeText(TBwelcome);

        }

        private void BtnNewTitle_Click(object sender, RoutedEventArgs e)
        {
            if (NewTitleWindow != null && NewTitleWindow.Visibility == Visibility.Visible)
                NewTitleWindow.Focus();
            else
            {
                NewTitleWindow = new NewTitleWindow(this);
                NewTitleWindow.Show();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
            MainWindow.id = 0;
        }

        private void setWelcomeText(TextBlock textBlock)
        {
            try
            {
                connection = new SqlConnection(MainWindow.connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT login FROM Users WHERE id = @user_id", connection);
                command.Parameters.AddWithValue("@user_id", MainWindow.id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string login = Convert.ToString(reader["login"]);
                    textBlock.Text = "Привет, " + login + "!";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { if (connection != null) connection.Close(); }
        }

        public void updateTitlesCount()
        {
            try
            {
                connection = new SqlConnection(MainWindow.connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT COUNT(id) as titlesCount FROM Titles WHERE user_id = @user_id", connection);
                command.Parameters.AddWithValue("@user_id", MainWindow.id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int titlesCount = Convert.ToInt32(reader["titlesCount"]);
                    TBtitlesCount.Text = titlesCount.ToString();
                }                 
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { if (connection != null) connection.Close(); }
        }

        public void updateList()
        {
            try
            {
                id_titleId_Map.Clear();
                LBtitles.Items.Clear();
                connection = new SqlConnection(MainWindow.connectionString);
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM Titles WHERE user_id = @userId", connection);
                command.Parameters.AddWithValue("@userId", MainWindow.id);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Если запись без фотографий
                    if (Convert.ToInt32(reader["have_image"]) == 0)
                    {
                        int title_id = Convert.ToInt32(reader["id"]);
                        string title = Convert.ToString(reader["title"]);
                        string text = Convert.ToString(reader["text"]);
                        DateTime date = Convert.ToDateTime(reader["date"]);

                        LBtitles.Items.Add(createTitle(title, text, date));
                        id_titleId_Map.Add(LBtitles.Items.Count - 1, title_id);
                    }
                    else
                    {
                        int title_id = Convert.ToInt32(reader["id"]);
                        string title = Convert.ToString(reader["title"]);
                        string text = Convert.ToString(reader["text"]);
                        DateTime date = Convert.ToDateTime(reader["date"]);

                        SqlCommand command2 = new SqlCommand("SELECT image, (SELECT COUNT(id) FROM Images WHERE title_id = @title_id) as imageCount FROM Images WHERE title_id = @title_id", connection);
                        command2.Parameters.AddWithValue("@title_id", title_id);
                        SqlDataReader imageReader = command2.ExecuteReader();

                        if (imageReader.Read())
                        {
                            var byteArray = (byte[])imageReader["image"];
                            int imagesCount = Convert.ToInt32(imageReader["imageCount"]);
                            imageReader.Close();

                            Image image = new Image();
                            image.Source = setImgSourceFromArray(byteArray);

                            LBtitles.Items.Add(createTitleWithImage(title, text, date, image, imagesCount));
                            id_titleId_Map.Add(LBtitles.Items.Count - 1, title_id);
                        }
                    }
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { if (connection != null) connection.Close();}
        }

        private Border createTitle (string title, string text, DateTime date) {
            Border border = new Border();
            border.Padding = new Thickness(5);
            border.Margin = new Thickness(10, 10, 10, 0);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;

            Grid mainGrid = new Grid();
            mainGrid.Width = 485;
            ColumnDefinition dateColumnDefinition = new ColumnDefinition();
            ColumnDefinition textColumnDefinition = new ColumnDefinition();

            dateColumnDefinition.Width = new GridLength(3, GridUnitType.Star);
            textColumnDefinition.Width = new GridLength(20, GridUnitType.Star);

            mainGrid.ColumnDefinitions.Add(dateColumnDefinition);
            mainGrid.ColumnDefinitions.Add(textColumnDefinition);
            
            StackPanel dateStackPanel = new StackPanel();
            dateStackPanel.Orientation = Orientation.Vertical;
            
            TextBlock dayOfWeekTextBlock = new TextBlock();
            dayOfWeekTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock dayTextBlock = new TextBlock();
            dayTextBlock.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock timeTextBlock = new TextBlock();
            timeTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            
            StackPanel textStackPanel = new StackPanel();
            textStackPanel.Margin = new Thickness(15,0,0,0);
            textStackPanel.Orientation = Orientation.Vertical;
            
            TextBlock titleTextBlock = new TextBlock();
            titleTextBlock.Height = 20;
            titleTextBlock.FontWeight = FontWeights.Black;
            
            TextBlock textTextBlock = new TextBlock();
            textTextBlock.MaxWidth = 490;
            textTextBlock.Height = 35;
            textTextBlock.TextWrapping = TextWrapping.Wrap;

            dayOfWeekTextBlock.Text = date.DayOfWeek.ToString();
            dayTextBlock.Text = date.Day.ToString();
            timeTextBlock.Text = date.TimeOfDay.Hours.ToString() + ":" + date.TimeOfDay.Minutes.ToString();

            titleTextBlock.Text = title;
            textTextBlock.Text = text;

            mainGrid.Children.Add(dateStackPanel);
            mainGrid.Children.Add(textStackPanel);

            Grid.SetColumn(dateStackPanel, 0);
            Grid.SetColumn(textStackPanel, 1);

            dateStackPanel.Children.Add(dayOfWeekTextBlock);
            dateStackPanel.Children.Add(dayTextBlock);
            dateStackPanel.Children.Add(timeTextBlock);

            textStackPanel.Children.Add(titleTextBlock);
            textStackPanel.Children.Add(textTextBlock);

            border.Child = mainGrid;

            //<Border Padding="5" Margin="10 10 10 0" BorderThickness="1" BorderBrush="Black">
            //        <Grid Width="485">
            //            <Grid.ColumnDefinitions>
            //                <ColumnDefinition Width="2*"/>
            //                <ColumnDefinition Width="21*"/>

            //            </Grid.ColumnDefinitions>
            //            <StackPanel Grid.Column="0" Orientation="Vertical">
            //                <TextBlock HorizontalAlignment="Center">Пт</TextBlock>
            //                <TextBlock HorizontalAlignment="Center">30</TextBlock>
            //                <TextBlock HorizontalAlignment="Center">9:30</TextBlock>
            //            </StackPanel>

            //            <StackPanel Grid.Column="1" Margin="15 0 0 0" Orientation="Vertical">
            //                <TextBlock Height="20" FontWeight="Black">Заголовофцкото фклоцолфкиц флкоцоли</TextBlock>
            //                <TextBlock Height="35" MaxWidth="490" TextWrapping="Wrap">ЦВфцашруы ыгпргшркп рыпг шквпгшр ыугшар MMMMMM MMM adwkawudh dawhdawua dwawdh dawh dawwad</TextBlock>
            //            </StackPanel>
            //        </Grid>
            //    </Border>


            return border;
        }

        private Border createTitleWithImage(string title, string text, DateTime date, Image image, int imagesCount)
        {
            Border border = new Border();
            border.Padding = new Thickness(5);
            border.Margin = new Thickness(10,10,10,0);
            border.BorderThickness = new Thickness(1);
            border.BorderBrush = Brushes.Black;

            Grid mainGrid = new Grid();
            mainGrid.Width = 485;
            ColumnDefinition dateColumnDefinition = new ColumnDefinition();
            ColumnDefinition textColumnDefinition = new ColumnDefinition();
            ColumnDefinition imageColumnDefinition = new ColumnDefinition();
            ColumnDefinition countColumnDefinition = new ColumnDefinition();

            dateColumnDefinition.Width = new GridLength(3,GridUnitType.Star);
            textColumnDefinition.Width = new GridLength(15, GridUnitType.Star);
            imageColumnDefinition.Width = new GridLength(4, GridUnitType.Star);
            countColumnDefinition.Width = new GridLength(1, GridUnitType.Star);


            mainGrid.ColumnDefinitions.Add(dateColumnDefinition);
            mainGrid.ColumnDefinitions.Add(textColumnDefinition);
            mainGrid.ColumnDefinitions.Add(imageColumnDefinition);
            mainGrid.ColumnDefinitions.Add(countColumnDefinition);


            StackPanel dateStack = new StackPanel();
            dateStack.Orientation = Orientation.Vertical;

            TextBlock dayOfWeekTB = new TextBlock();
            dayOfWeekTB.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock dayTB = new TextBlock();
            dayTB.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock timeTB = new TextBlock();
            timeTB.HorizontalAlignment = HorizontalAlignment.Center;

            dayOfWeekTB.Text = date.DayOfWeek.ToString();
            dayTB.Text = date.Day.ToString();
            timeTB.Text = date.TimeOfDay.Hours.ToString() + ":" + date.TimeOfDay.Minutes.ToString();

            StackPanel textStack = new StackPanel();
            textStack.Margin = new Thickness(15,0,0,0);
            textStack.Orientation = Orientation.Vertical;

            TextBlock titleTB = new TextBlock();
            titleTB.Height = 20;
            titleTB.FontWeight = FontWeights.Black;
            titleTB.Text = title;

            TextBlock textTB = new TextBlock();
            textTB.MaxWidth = 350;
            textTB.Height = 35;
            textTB.Text = text;

            Image imageUI = new Image();
            image.Height = 56;
            image.Width = 70;
            image.Stretch = Stretch.Fill;
            imageUI = image;

            StackPanel imgCountStack = new StackPanel();
            imgCountStack.Orientation = Orientation.Vertical;
            imgCountStack.VerticalAlignment = VerticalAlignment.Center;

            Image cameraImgUI = new Image();
            cameraImgUI.Height = 20;
            cameraImgUI.Source = createBitmapFromPath("/Diary;component/Resources/camera.png");

            TextBlock imgCountTB = new TextBlock();
            imgCountTB.HorizontalAlignment = HorizontalAlignment.Center;
            imgCountTB.Text = imagesCount.ToString();

            border.Child = mainGrid;

            mainGrid.Children.Add(dateStack);
            mainGrid.Children.Add(textStack);
            mainGrid.Children.Add(imageUI);
            mainGrid.Children.Add(imgCountStack);

            Grid.SetColumn(dateStack, 0);
            Grid.SetColumn(textStack, 1);
            Grid.SetColumn(imageUI, 2);
            Grid.SetColumn(imgCountStack, 3);

            dateStack.Children.Add(dayOfWeekTB);
            dateStack.Children.Add(dayTB);
            dateStack.Children.Add(timeTB);

            textStack.Children.Add(titleTB);
            textStack.Children.Add(textTB);

            imgCountStack.Children.Add(cameraImgUI);
            imgCountStack.Children.Add(imgCountTB);

            return border;

//<Border Padding="5" Margin="10 10 10 0" BorderThickness="1" BorderBrush="Black">
//                    <Grid Width="485">
//                        <Grid.ColumnDefinitions>
//                            <ColumnDefinition Width="2*"/>
//                            <ColumnDefinition Width="16*"/>
//                            <ColumnDefinition Width="4*"/>
//                            <ColumnDefinition Width="1*"/>
//                        </Grid.ColumnDefinitions>
//                        <StackPanel Grid.Column="0" Orientation="Vertical">
//                            <TextBlock HorizontalAlignment="Center">Пт</TextBlock>
//                            <TextBlock HorizontalAlignment="Center">30</TextBlock>
//                            <TextBlock HorizontalAlignment="Center">9:30</TextBlock>
//                        </StackPanel>

//                        <StackPanel Grid.Column="1" Margin="15 0 0 0" Orientation="Vertical">
//                            <TextBlock Height="20" FontWeight="Black">Заголовофцкото фклоцолфкиц флкоцоли</TextBlock>
//                            <TextBlock Height="35" MaxWidth="350" TextWrapping="Wrap">ЦВфцашруы ыгпргшркп рыпг шквпгшр ыугшар MMMMMM MMM adwkawudh dawhdawua dwawdh dawh dawwad</TextBlock>
//                        </StackPanel>

//                        <Image Grid.Column="2" Height="56" Stretch="Fill" Width="70" Source="C:\Users\User\Pictures\1920x1080_Neon.jpg"></Image>

//                        <StackPanel Grid.Column="3" Orientation="Vertical" VerticalAlignment="Center">
//                            <Image Source="/Resources/camera.png" Height="20"></Image>
//                            <TextBlock HorizontalAlignment="Center">7</TextBlock>
//                        </StackPanel>
//                    </Grid>
//                </Border>
        }

        private BitmapImage createBitmapFromPath(String path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Relative);
            bitmap.EndInit();
            return bitmap;
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

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var element = LBtitles.SelectedIndex;
            deleteElement(element);
        }

        private void deleteElement(int index)
        {
            int title_id = id_titleId_Map.ElementAt(index).Value;
            connection = new SqlConnection(MainWindow.connectionString);
            connection.Open();

            SqlCommand deleteImageCommand = new SqlCommand("DELETE FROM Images WHERE title_id = @title_id", connection);
            deleteImageCommand.Parameters.AddWithValue("title_id", title_id);
            deleteImageCommand.ExecuteNonQuery();

            SqlCommand command = new SqlCommand("DELETE FROM Titles WHERE id = @title_id", connection);
            command.Parameters.AddWithValue("@title_id", title_id);
            command.ExecuteNonQuery();
           
            connection.Close();
            id_titleId_Map.Remove(index);
            updateList();
            updateTitlesCount();
            MessageBox.Show("Запись удалена!");
        }

        private void LBtitles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = LBtitles.SelectedIndex;
            int title_id = id_titleId_Map.ElementAt(element).Value;
            if (ShowTitleWindow != null && ShowTitleWindow.Visibility == Visibility.Visible)
            {
                ShowTitleWindow.Close();
                ShowTitleWindow = new ShowTitleWindow(this, title_id);
                ShowTitleWindow.Show();
                ShowTitleWindow.Focus();
            }
            else
            {
                ShowTitleWindow = new ShowTitleWindow(this, title_id);
                ShowTitleWindow.Show();
            }
        }

        private void MenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            var element = LBtitles.SelectedIndex;
            int title_id = id_titleId_Map.ElementAt(element).Value;
            if (EditTitleWindow != null && EditTitleWindow.Visibility == Visibility.Visible)
            {
                EditTitleWindow.Close();
                EditTitleWindow = new EditTitleWindow(this, title_id);
                EditTitleWindow.Show();
                EditTitleWindow.Focus();
            }
            else
            {
                EditTitleWindow = new EditTitleWindow(this, title_id);
                EditTitleWindow.Show();
            }
        }
    }
}
