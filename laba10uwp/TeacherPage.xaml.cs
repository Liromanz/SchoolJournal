using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace laba10uwp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class TeacherPage : Page
    {
        public static List<ListBox> listBoxes = new List<ListBox>();
        public List<List<string>> biglist = new List<List<string>>(new List<string>[6]);
        public List<string> disciplines = new List<string>();
        public List<string> groups = new List<string>();
        public List<string> login = new List<string>();
        public List<UserData> students = new List<UserData>();

        public List<List<string>> marks = new List<List<string>>();
        public List<string> savingmarks;
        public List<string> loadedmarks;

        public string groupstring = "";
        public string discipstring = "";
        public string date = "";
        public int dateint = 0;

        public TeacherPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            List<UserData> users = new List<UserData>();
            users = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            foreach (UserData user in users)
            {
                if (user.AccountType == 0) students.Add(user);
            }

            disciplines = await De_Serialization.Deserialisation<List<string>>("Disciplines.xml", CreationCollisionOption.OpenIfExists, disciplines);

            groups = await De_Serialization.Deserialisation<List<string>>("Groups.xml", CreationCollisionOption.OpenIfExists, groups);
            AddingItems(groups, groupsListBox);
            RaspComboBox.ItemsSource = groups;

            listBoxes = new List<ListBox>();
            listBoxes.Add(RaspMonListBox);
            listBoxes.Add(RaspTueListBox);
            listBoxes.Add(RaspWenListBox);
            listBoxes.Add(RaspThuListBox);
            listBoxes.Add(RaspFriListBox);
            listBoxes.Add(RaspSatListBox);

            for (int i = 0; i < biglist.Count; i++)
            {
                listBoxes[i].ItemsSource = biglist[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для групп
        private void groupsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Выбор группы
        {
            if (groupsListBox.SelectedIndex != -1)
            {
                groupstring = groups[groupsListBox.SelectedIndex];
                ListBoxItems();
                AddingDiscItems();
            }
        }
        private void disciplinesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Выбор дисциплины
        {
            if (disciplinesListBox.SelectedIndex != -1)
            {
                List<string> list = biglist[dateint];
                string[] splitted = list[disciplinesListBox.SelectedIndex].Split(" | ");
                discipstring = splitted[1];
                dispTextBlock.Text = discipstring;
                JournalDispTextBox.Text = discipstring;
            }
            else dispTextBlock.Text = "";
        }
        private void JournalDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args) //Выбор даты
        {
            if (JournalDatePicker.Date.Year > 2010)
            {
                date = JournalDatePicker.Date.DayOfWeek.ToString();
                if (date != "Sunday")
                {
                    AddingDiscItems();
                    ListBoxItems();
                }
                else
                {
                    listbox.Items.Clear();
                }
            }
        }
        private async void AddingDiscItems() //Добавление дисциплин
        {
            if ((date != "Sunday" && date != "") && groupstring != "")
            {
                if (date == "Monday") dateint = 0;
                if (date == "Tuesday") dateint = 1;
                if (date == "Wednesday") dateint = 2;
                if (date == "Thursday") dateint = 3;
                if (date == "Friday") dateint = 4;
                if (date == "Saturday") dateint = 5;

                login = new List<string>();
                login = await De_Serialization.Deserialisation<List<string>>("LoginData.xml", CreationCollisionOption.OpenIfExists, login);

                biglist = new List<List<string>>(new List<string>[6]);
                biglist = await De_Serialization.Deserialisation<List<List<string>>>($"{groupstring}.xml", CreationCollisionOption.OpenIfExists, biglist);
                for (int i = 0; i < biglist.Count; i++)
                {
                    List<string> smollist = biglist[i];
                    List<string> cleanlist = new List<string>();
                    for (int g = 0; g < smollist.Count; g++)
                    {
                        if (smollist[g].Contains(login[3]))
                            cleanlist.Add(smollist[g]);
                    }

                    biglist[i] = cleanlist;
                }
                disciplinesListBox.ItemsSource = new List<string>();
                disciplinesListBox.ItemsSource = biglist[dateint];

            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для расписания
        private async void RaspComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Выбор группы
        {
            login = new List<string>();
            login = await De_Serialization.Deserialisation<List<string>>("LoginData.xml", CreationCollisionOption.OpenIfExists, login);

            biglist = new List<List<string>>(new List<string>[6]);
            biglist = await De_Serialization.Deserialisation<List<List<string>>>($"{RaspComboBox.SelectedItem.ToString()}.xml", CreationCollisionOption.OpenIfExists, biglist);
            for (int i = 0; i < biglist.Count; i++)
            {
                List<string> smollist = biglist[i];
                List<string> cleanlist = new List<string>();
                for (int g = 0; g < smollist.Count; g++)
                {
                    if (smollist[g].Contains(login[3]))
                        cleanlist.Add(smollist[g]);
                }

                biglist[i] = cleanlist;
                listBoxes[i].ItemsSource = biglist[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для журнала
        private async void ListBoxItems() //Лист айтемы студентов + оценки
        {
            listbox.Items.Clear();
            List<UserData> groupstudent = new List<UserData>();
            foreach (UserData groupguy in students)
            {
                if (groupguy.DisciplineType == groupstring) groupstudent.Add(groupguy);
            }
            savingmarks = new List<string>(new string[groupstudent.Count]);

            //Десериализация - начало
            if (discipstring != "" && groupstring != "")
            {
                marks = await De_Serialization.Deserialisation<List<List<string>>>($"{discipstring}-{groupstring}.xml", CreationCollisionOption.OpenIfExists, marks);
            }
            bool confirm = false;
            List<string> data = new List<string>();
            for (int i = 0; i < marks.Count; i++)
            {
                data = marks[i];
                if (data[data.Count - 1] == JournalDatePicker.Date.ToString("dd.MM.yyyy"))
                {
                    confirm = true;
                    break;
                }
            }
            //Десериализация - конец

            for (int i = 0; i < groupstudent.Count; i++)
            {
                ListBoxItem boxItem = new ListBoxItem();
                Grid grid = new Grid();

                boxItem.SizeChanged += (s, e) =>
                {
                    grid.Width = boxItem.ActualWidth - 25;
                };
                ColumnDefinition c1 = new ColumnDefinition();
                c1.Width = new GridLength(1, GridUnitType.Star);
                grid.ColumnDefinitions.Add(c1);
                ColumnDefinition c2 = new ColumnDefinition();
                c2.Width = new GridLength(1, GridUnitType.Auto);
                grid.ColumnDefinitions.Add(c2);

                TextBlock disciptextBox = new TextBlock();
                disciptextBox.FontSize = 18;
                disciptextBox.Text = $"{groupstudent[i].Surname} {groupstudent[i].Name}";
                disciptextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                Grid.SetColumn(disciptextBox, 0);
                grid.Children.Add(disciptextBox);

                ComboBox box = new ComboBox();
                box.Items.Add("5");
                box.Items.Add("4");
                box.Items.Add("3");
                box.Items.Add("2");
                box.Items.Add("НБ");
                if (confirm)
                {
                    string[] dapasplit = data[i].Split('|');
                    if (dapasplit[0] == "5") box.SelectedIndex = 0;
                    if (dapasplit[0] == "4") box.SelectedIndex = 1;
                    if (dapasplit[0] == "3") box.SelectedIndex = 2;
                    if (dapasplit[0] == "2") box.SelectedIndex = 3;
                    if (dapasplit[0] == "НБ") box.SelectedIndex = 4;
                }
                box.HorizontalAlignment = HorizontalAlignment.Right;
                box.SelectionChanged += (s, e) =>
                {
                    if (box.SelectedIndex != -1 && listbox.SelectedIndex != -1)
                        savingmarks[listbox.SelectedIndex] = $"{box.SelectedItem.ToString()}|{groupstudent[listbox.SelectedIndex].Surname}";
                };
                Grid.SetColumn(box, 1);
                grid.Children.Add(box);

                boxItem.Content = grid;
                listbox.Items.Add(boxItem);
            }
        }
        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e) //Сохранение оценок
        {
            bool confirm = true;
            foreach (var mark in savingmarks)
            {
                if (mark == null)
                {
                    confirm = false;
                    break;
                }
            }
            if (JournalDispTextBox.Text != "" && JournalGroupTextBox.Text != "" && confirm)
            {
                savingmarks.Add(JournalDatePicker.Date.ToString("dd.MM.yyyy"));
                bool checkdate = false;
                for (int i = 0; i < marks.Count; i++)
                {
                    List<string> data = marks[i];
                    if (data[data.Count - 1] == savingmarks[savingmarks.Count - 1])
                    {
                        checkdate = true;
                        marks[i] = savingmarks;
                        break;
                    }
                }
                if (!checkdate)
                    marks.Add(savingmarks);
                De_Serialization.Serialization<List<List<string>>>($"{JournalDispTextBox.Text}-{JournalGroupTextBox.Text}.xml", CreationCollisionOption.ReplaceExisting, marks);
            }
            else
            {
                ContentDialog prog = new ContentDialog()
                {
                    Title = "Ошибка",
                    Content = "Повторите заполнение оценок. Правильное заполнение: нажимаете на студента, затем в списке справа выбираете его оценку.",
                    PrimaryButtonText = "OK"
                };
                await prog.ShowAsync();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Системные фичи
        private void CommandBar_Opening(object sender, object e) => (sender as CommandBar).IsOpen = false; //Держать CommandBar закрытым
        private void AddingItems(List<string> vs, ListBox listBox)//Добавление элементов в лист
        {
            foreach (var str in vs)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = str;
                listBox.Items.Add(boxItem);
            }
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)//Возврат
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }
    }
}
