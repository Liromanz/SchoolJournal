using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace laba10uwp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        public static List<ListBox> listBoxes = new List<ListBox>();
        public List<List<string>> biglist = new List<List<string>>(new List<string>[6]);
        public List<string> disciplines = new List<string>();
        public List<string> groups = new List<string>();
        public List<UserData> users = new List<UserData>();
        public List<UserData> usersclone = new List<UserData>();

        public object SelectionSender;
        public DateTime date;
        public AdminPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loading(FrameworkElement sender, object args) //Начало работы
        {
            listBoxes = new List<ListBox>();

            users = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            dataGrid.ItemsSource = users;
            usersclone = users;

            disciplines = await De_Serialization.Deserialisation<List<string>>("Disciplines.xml", CreationCollisionOption.OpenIfExists, disciplines);
            AddingItems(disciplines, DisciplinesListBox);
            ExcelDiscComboBox.ItemsSource = disciplines;

            groups = await De_Serialization.Deserialisation<List<string>>("Groups.xml", CreationCollisionOption.OpenIfExists, groups);
            AddingItems(groups, GroupListBox);
            RaspComboBox.ItemsSource = groups;
            ExcelGroupComboBox.ItemsSource = groups;

            listBoxes.Add(RaspMonListBox);
            listBoxes.Add(RaspTueListBox);
            listBoxes.Add(RaspWenListBox);
            listBoxes.Add(RaspThuListBox);
            listBoxes.Add(RaspFriListBox);
            listBoxes.Add(RaspSatListBox);

        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для дисциплин/групп
        private void CommandBar_Opening(object sender, object e) => (sender as CommandBar).IsOpen = false; //Держать CommandBar закрытым
        private async void AddButton_Click(object sender, RoutedEventArgs e) //Добавление
        {
            if ((sender as AppBarButton).Name == "AddButton")
            {
                string discName = await InputDialog("Введите название дисциплины", "Добавить");
                if (discName != "")
                {
                    disciplines.Add(discName);
                    Refreshing(DisciplinesListBox);
                }
            }
            else
            {
                string groupName = await InputDialog("Введите название группы", "Добавить");
                if (groupName != "")
                {
                    groups.Add(groupName);
                    Refreshing(GroupListBox);
                }
            }
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e) //Изменение
        {
            if (GroupListBox.SelectedIndex != -1 || DisciplinesListBox.SelectedIndex != -1)
            {
                string Name = await InputDialog("Введите новое название", "Изменить");
                if (Name != "")
                {
                    if ((sender as AppBarButton).Name == "EditButton")
                    {
                        disciplines[DisciplinesListBox.SelectedIndex] = Name;
                        Refreshing(DisciplinesListBox);
                    }
                    else
                    {
                        groups[GroupListBox.SelectedIndex] = Name;
                        Refreshing(GroupListBox);
                    }
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e) //Удаление
        {
            if (GroupListBox.SelectedIndex != -1 || DisciplinesListBox.SelectedIndex != -1)
            {
                if ((sender as AppBarButton).Name == "DeleteButton")
                {
                    disciplines.RemoveAt(DisciplinesListBox.SelectedIndex);
                    Refreshing(DisciplinesListBox);
                }
                else
                {
                    groups.RemoveAt(GroupListBox.SelectedIndex);
                    Refreshing(GroupListBox);
                }
            }
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e) //Возвращение
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для расписания
        private async void AddAndEditRaspButton_Click(object sender, RoutedEventArgs e) //Добававление и изменение
        {
            if (((sender as AppBarButton).Name == "AddRaspButton" && RaspComboBox.SelectedIndex != -1) || ((sender as AppBarButton).Name == "EditRaspButton" && SelectionSender != null && (SelectionSender as ListBox).SelectedIndex != -1))
            {
                ContentDialog content = new ContentDialog();
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Vertical;

                TextBox disciptextBox = new TextBox();
                disciptextBox.PlaceholderText = "Дисциплина";
                disciptextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                stack.Children.Add(disciptextBox);

                TextBox preptextBox = new TextBox();
                preptextBox.PlaceholderText = "Преподаватель";
                preptextBox.Margin = new Thickness(0, 10, 0, 10);
                preptextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                stack.Children.Add(preptextBox);

                ComboBox box = new ComboBox();
                if ((sender as AppBarButton).Name == "AddRaspButton")
                {
                    box.Items.Add("Понедельник");
                    box.Items.Add("Вторник");
                    box.Items.Add("Среда");
                    box.Items.Add("Четверг");
                    box.Items.Add("Пятница");
                    box.Items.Add("Суббота");
                    box.MinWidth = 150;
                    box.HorizontalAlignment = HorizontalAlignment.Center;
                    stack.Children.Add(box);
                }

                content.Content = stack;
                content.Title = "Дисциплина в расписании";
                content.CloseButtonText = "Отмена";
                content.PrimaryButtonText = "Добавить";

                content.PrimaryButtonClick += (s, ev) =>
                {
                    string str = $"{disciptextBox.Text} | {preptextBox.Text}";

                    if (disciptextBox.Text != "" && preptextBox.Text != "")
                    {
                        bool confirmed = false;
                        foreach (var item in disciplines)
                        {
                            if (disciptextBox.Text == item)
                            {
                                confirmed = true;
                                break;
                            }
                        }
                        if (confirmed)
                        {
                            if ((sender as AppBarButton).Name == "AddRaspButton")
                                AddingAndEditingRasp(box.SelectedIndex, str, sender);
                            else
                            {
                                int indx = 0;
                                if ((SelectionSender as ListBox).Name == "RaspMonListBox") indx = 0;
                                if ((SelectionSender as ListBox).Name == "RaspTueListBox") indx = 1;
                                if ((SelectionSender as ListBox).Name == "RaspWenListBox") indx = 2;
                                if ((SelectionSender as ListBox).Name == "RaspThuListBox") indx = 3;
                                if ((SelectionSender as ListBox).Name == "RaspFriListBox") indx = 4;
                                if ((SelectionSender as ListBox).Name == "RaspSatListBox") indx = 5;
                                AddingAndEditingRasp(indx, str, sender);
                            }

                        }
                    }
                };
                await content.ShowAsync();
            }
        }
        private void DeleteRaspButton_Click(object sender, RoutedEventArgs e) //Удаление
        {
            if (SelectionSender != null && (SelectionSender as ListBox).SelectedIndex != -1)
            {
                int indx = 0;
                if ((SelectionSender as ListBox).Name == "RaspMonListBox") indx = 0;
                if ((SelectionSender as ListBox).Name == "RaspTueListBox") indx = 1;
                if ((SelectionSender as ListBox).Name == "RaspWenListBox") indx = 2;
                if ((SelectionSender as ListBox).Name == "RaspThuListBox") indx = 3;
                if ((SelectionSender as ListBox).Name == "RaspFriListBox") indx = 4;
                if ((SelectionSender as ListBox).Name == "RaspSatListBox") indx = 5;

                List<string> smollist = new List<string>();
                for (int i = 0; i < listBoxes[indx].Items.Count; i++)
                {
                    string str = listBoxes[indx].Items[i].ToString();
                    smollist.Add(str);
                }

                smollist.RemoveAt((SelectionSender as ListBox).SelectedIndex);

                biglist[indx] = smollist;
                for (int i = 0; i < biglist.Count; i++)
                {
                    listBoxes[i].ItemsSource = biglist[i];
                }
            }
        }
        private void SaveRaspButton_Click(object sender, RoutedEventArgs e) { SavingRasp(); } //Сохранение
        private void RaspComboBox_DropDownOpened(object sender, object e) { SavingRasp(); } //Автосохранение
        private async void RaspComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Загрузка расписания группы
        {
            biglist = new List<List<string>>(new List<string>[6]);
            biglist = await De_Serialization.Deserialisation<List<List<string>>>($"{RaspComboBox.SelectedItem.ToString()}.xml", CreationCollisionOption.OpenIfExists, biglist);
            for (int i = 0; i < biglist.Count; i++)
            {
                listBoxes[i].ItemsSource = biglist[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для пользователей
        private async void SaveUserButton_Click(object sender, RoutedEventArgs e) //Сохранение пользователей
        {
            usersclone = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Password != usersclone[i].Password)
                    users[i].Password = Cypher.GetMd5Hash(users[i].Password);
            }
            De_Serialization.Serialization<List<UserData>>("UserData.xml", CreationCollisionOption.ReplaceExisting, users);
            dataGrid.ItemsSource = new List<UserData>();
            dataGrid.ItemsSource = users;
            date = DateTime.Now;
            SaveTextBlock.Text = $"Последнее сохранение: {date}";
        }
        private async void LoadUserButton_Click(object sender, RoutedEventArgs e)//Загрузка старых данных
        {
            usersclone = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            string confirm = await YesNoDialog("Подтвердите действие", $"Вы уверены, что хотите загрузить последние сохраненные данные, не сохранив нынешние? Последнее сохранение было в {date}");
            if (confirm == "yes")
            {
                users = usersclone;
                dataGrid.ItemsSource = new List<UserData>();
                dataGrid.ItemsSource = users;
            }
        }
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)//Удаление
        {
            if (dataGrid.SelectedIndex != -1)
            {
                users.RemoveAt(dataGrid.SelectedIndex);
                dataGrid.ItemsSource = new List<UserData>();
                dataGrid.ItemsSource = users;
            }
        }
        private void AddUserButton_Click(object sender, RoutedEventArgs e)//Добавление
        {
            UserData us = new UserData("", "", "", "", 0, "");
            users.Add(us);

            De_Serialization.Serialization<List<UserData>>("UserData.xml", CreationCollisionOption.ReplaceExisting, users);
            date = DateTime.Now;
            SaveTextBlock.Text = $"Последнее сохранение: {date}";

            dataGrid.ItemsSource = new List<UserData>();
            dataGrid.ItemsSource = users;
        }
        private async void dataGrid_CellEditEnded(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridCellEditEndedEventArgs e)//Проверка ячеек после изменения
        {
            usersclone = await De_Serialization.Deserialisation<List<UserData>>("UserData.xml", CreationCollisionOption.OpenIfExists, users);
            if (e.Column.Header.ToString() == "DisciplineType")
            {
                var item = (UserData)dataGrid.SelectedItem;
                bool confirm = false;
                if (item.AccountType == 0)
                    foreach (string group in groups)
                    {
                        if (group == item.DisciplineType)
                        {
                            confirm = true;
                            break;
                        }
                    }
                if (!confirm)
                {
                    users[dataGrid.SelectedIndex].DisciplineType = usersclone[dataGrid.SelectedIndex].DisciplineType;
                    dataGrid.ItemsSource = new List<UserData>();
                    dataGrid.ItemsSource = users;
                }
            }

            if (e.Column.Header.ToString() == "AccountType")
            {
                if (users[dataGrid.SelectedIndex].AccountType > 2 || users[dataGrid.SelectedIndex].AccountType < 0)
                    users[dataGrid.SelectedIndex].AccountType = usersclone[dataGrid.SelectedIndex].AccountType;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для генерации
        private async void CreateExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExcelGroupComboBox.SelectedIndex != -1 && ExcelDiscComboBox.SelectedIndex != -1)
            {
                FileInfo fileex = new FileInfo(ApplicationData.Current.LocalFolder.Path + $@"\{groups[ExcelGroupComboBox.SelectedIndex]}.xlsx");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                ToastNotifier toastNote = ToastNotificationManager.CreateToastNotifier();
                Windows.Data.Xml.Dom.XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                Windows.Data.Xml.Dom.XmlNodeList nodes = xml.GetElementsByTagName("text");
                nodes.Item(0).AppendChild(xml.CreateTextNode("Генерация Excel файла"));

                using (ExcelPackage excelPackage = new ExcelPackage(fileex))
                {
                    try
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"{disciplines[ExcelDiscComboBox.SelectedIndex]}");

                        List<List<string>> marks = new List<List<string>>();
                        marks = await De_Serialization.Deserialisation<List<List<string>>>($"{disciplines[ExcelDiscComboBox.SelectedIndex]}-{groups[ExcelGroupComboBox.SelectedIndex]}.xml", CreationCollisionOption.OpenIfExists, marks);
                        for (int i = 1; i < marks.Count + 1; i++)
                        {
                            List<string> day = marks[i - 1];
                            for (int g = 1; g < day.Count + 1; g++)
                            {
                                string[] splitted = day[g - 1].Split('|');
                                if (i == 1 && g != day.Count)
                                {
                                    worksheet.Cells[g + 1, i].Value = splitted[1];
                                    worksheet.Cells[g + 1, i + 1].Value = splitted[0];
                                }
                                else if (g == day.Count)
                                    worksheet.Cells[1, i+1].Value = day[g - 1];
                                else
                                    worksheet.Cells[g + 1, i+1].Value = splitted[0];
                            }
                        }

                        excelPackage.Save();

                        nodes.Item(1).AppendChild(xml.CreateTextNode("Excel файл успешно сохранен!"));
                        
                    }
                    catch
                    {
                        nodes.Item(1).AppendChild(xml.CreateTextNode("Произошла ошибка в сохранении. Такое бывает, если в Excel уже есть лист с таким именем"));
                    }
                    ToastNotification toast = new ToastNotification(xml);
                    toast.ExpirationTime = DateTime.Now.AddSeconds(4);
                    toastNote.Show(toast);
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Системные фичи
        private void AddingItems(List<string> vs, ListBox listBox)//Добавление элементов в лист
        {
            foreach (var str in vs)
            {
                ListBoxItem boxItem = new ListBoxItem();
                boxItem.Content = str;
                listBox.Items.Add(boxItem);
            }
        }
        public async void Refreshing(ListBox listBox)//Обновить список
        {
            if (listBox.Name == "DisciplinesListBox")
            {
                De_Serialization.Serialization<List<string>>("Disciplines.xml", CreationCollisionOption.ReplaceExisting, disciplines);
                DisciplinesListBox.Items.Clear();
                disciplines = await De_Serialization.Deserialisation<List<string>>("Disciplines.xml", CreationCollisionOption.OpenIfExists, disciplines);
                AddingItems(disciplines, DisciplinesListBox);
            }
            else
            {
                De_Serialization.Serialization<List<string>>("Groups.xml", CreationCollisionOption.ReplaceExisting, groups);
                GroupListBox.Items.Clear();
                groups = await De_Serialization.Deserialisation<List<string>>("Groups.xml", CreationCollisionOption.OpenIfExists, groups);
                AddingItems(groups, GroupListBox);
            }
        }
        private void AddingAndEditingRasp(int listindex, string raspstr, object sender) //Добавить и изменить расписание
        {
            List<string> smollist = new List<string>();
            for (int i = 0; i < listBoxes[listindex].Items.Count; i++)
            {
                string str = listBoxes[listindex].Items[i].ToString();
                smollist.Add(str);
            }

            if ((sender as AppBarButton).Name == "AddRaspButton")
            {
                raspstr = $"{listBoxes[listindex].Items.Count + 1} | {raspstr}";
                smollist.Add(raspstr);
            }
            else
            {
                string first = smollist[(SelectionSender as ListBox).SelectedIndex].Substring(0, 4);
                smollist[(SelectionSender as ListBox).SelectedIndex] = first + raspstr;
            }

            biglist[listindex] = smollist;
            for (int i = 0; i < biglist.Count; i++)
            {
                listBoxes[i].ItemsSource = biglist[i];
            }
        }
        private void SavingRasp() //Сохранить расписание
        {
            if (RaspComboBox.SelectedItem != null)
            {
                List<string> smollist;
                biglist = new List<List<string>>(new List<string>[6]);

                for (int i = 0; i < listBoxes.Count; i++)
                {
                    smollist = new List<string>();
                    for (int g = 0; g < listBoxes[i].Items.Count; g++)
                    {
                        string str = listBoxes[i].Items[g].ToString();
                        smollist.Add(str);
                    }
                    biglist[i] = smollist;
                }
                De_Serialization.Serialization<List<List<string>>>($"{RaspComboBox.SelectedItem.ToString()}.xml", CreationCollisionOption.ReplaceExisting, biglist);
            }
        }
        private void GetSelection(object sender, SelectionChangedEventArgs e) //Получить выбранный элемент
        {
            SelectionSender = sender;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Диалоговые окна
        private async Task<string> InputDialog(string title, string button) //Диалоговое окно с созданием
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;

            ContentDialog dialog = new ContentDialog()
            {
                Content = inputTextBox,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = button,
                SecondaryButtonText = "Отмена"
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }
        private async Task<string> YesNoDialog(string title, string content) //Диалоговое окно с выбором да/нет
        {
            ContentDialog dialog = new ContentDialog()
            {
                Content = content,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Да",
                SecondaryButtonText = "Нет"
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return "yes";
            else
                return "no";
        }


    }
}