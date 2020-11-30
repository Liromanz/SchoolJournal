using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace laba10uwp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static int count = 0;
        public List<string> groups = new List<string>();
        public List<UserData> users = new List<UserData>();
        public MainPage()
        {
            this.InitializeComponent();
        }
        
        //----------------------------------------------------------------------------------------------------------------Общее
        private async void Page_Loading(FrameworkElement sender, object args)
        {
            users = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            groups = await De_Serialization.Deserialisation<List<string>>("Groups.xml", CreationCollisionOption.OpenIfExists, groups);
            RegDiscBox.ItemsSource = groups;
        }
        private void Button_Click(object sender, RoutedEventArgs e) //Кнопка для экрана входа
        {
            RegGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
            RegLogButton.Content = "Войти";

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)// Кнопка для жкрана регистрации
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegGrid.Visibility = Visibility.Visible;
            RegLogButton.Content = "Зарегистрироваться";
        }
        private async void RegLogButton_Click(object sender, RoutedEventArgs e) //Кнопка входа/регистрации
        {
            if (RegGrid.Visibility == Visibility.Visible) //Действия для регистрации
            {
                if (RegTextBox.Text != "")
                    if (RegPasswordbx.Password != "")
                    {
                        string passw = Cypher.GetMd5Hash(RegPasswordbx.Password);
                        if (RegNameBox.Text != "")
                            if (RegSurNameBox.Text != "")
                                if (RegTypeBox.SelectedIndex != -1)
                                {
                                    UserData us = new UserData(RegTextBox.Text, passw, RegNameBox.Text, RegSurNameBox.Text, RegTypeBox.SelectedIndex, RegTextBlock.Text);
                                    users.Add(us);
                                    De_Serialization.Serialization<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);

                                    RegGrid.Visibility = Visibility.Collapsed;
                                    LoginGrid.Visibility = Visibility.Visible;
                                    RegLogButton.Content = "Войти";
                                }
                    }
                //Ошибки
                if (RegTextBox.Text == "" || RegPasswordbx.Password == "" || RegNameBox.Text == "" || RegSurNameBox.Text == "" || RegTypeBox.SelectedIndex == -1)
                {
                    ContentDialog prog = new ContentDialog() 
                    {
                        Title = "Ошибка",
                        Content = "Одно или несколько полей пустые",
                        PrimaryButtonText = "OK"
                    };
                    await prog.ShowAsync();
                }
            }
            else //Действия для входа
            {
                if (users.Count != 0 && LoginTextBox.Text != "" && Passwordbx.Password != "")
                {
                    string passw = Cypher.GetMd5Hash(Passwordbx.Password);
                    bool confirmed = true;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (LoginTextBox.Text == users[i].Login && passw == users[i].Password)
                        {
                            List<string> us = new List<string>() { users[i].Login, passw, users[i].Name, users[i].Surname, users[i].AccountType.ToString(), users[i].DisciplineType };
                            De_Serialization.Serialization<List<string>>("LoginData.xml", CreationCollisionOption.ReplaceExisting, us);

                            confirmed = true;

                            if (users[i].AccountType == 0) this.Frame.Navigate(typeof(StudentPage));
                            if (users[i].AccountType == 1) this.Frame.Navigate(typeof(TeacherPage));
                            if (users[i].AccountType == 2) this.Frame.Navigate(typeof(AdminPage));

                            break;
                        }
                        else
                        {
                            confirmed = false;
                        }
                    }
                    if (!confirmed)
                    {
                        ContentDialog prog = new ContentDialog()
                        {
                            Title = "Ошибка",
                            Content = "Неверный логин или пароль",
                            PrimaryButtonText = "OK"
                        };
                        await prog.ShowAsync();
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------------------Регистрация
        private void TextBox_LostFocus(object sender, RoutedEventArgs e) //Большие буквы в имени
        {
            string newText = Convert.ToString((sender as TextBox).Text);
            if (newText.Length > 0)
                if (newText[0] == char.ToLower(newText[0])) newText = char.ToUpper(newText[0]) + newText.Substring(1);
            (sender as TextBox).Text = newText;
        }
        private void RegTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Отображение группы
        {
            if (RegTypeBox.SelectedIndex == 0) RegDiscBox.Visibility = Visibility.Visible;
            else
            {
                RegDiscBox.Visibility = Visibility.Collapsed;
                RegDiscBox.SelectedIndex = -1;
            }
        }
        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)//ПКМ по регистрации чтобы добавить администратора
        {
            count++;
            if (count == 8)
            {
                ComboBoxItem adm = new ComboBoxItem();
                adm.Content = "Администратор";
                RegTypeBox.Items.Add(adm);
            }
        }
    }
}