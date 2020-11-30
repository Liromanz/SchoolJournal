using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class StudentPage : Page
    {
        public List<string> login = new List<string>();
        public List<string> dayrasp = new List<string>();
        public List<List<string>> biglist = new List<List<string>>(new List<string>[6]);

        public StudentPage()
        {
            this.InitializeComponent();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            login = await De_Serialization.Deserialisation<List<string>>("LoginData.xml", CreationCollisionOption.OpenIfExists, login);

            biglist = await De_Serialization.Deserialisation<List<List<string>>>($"{login[5]}.xml", CreationCollisionOption.OpenIfExists, biglist);

        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Кнопки для групп

        private async void RaspListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RaspDatePicker.Date.ToString() != "" && (sender as ListBox).SelectedIndex != -1)
            {
                string[] para = dayrasp[(sender as ListBox).SelectedIndex].Split(" | ");
                FinalDispTextBlock.Text = para[1];

                List<List<string>> marks = new List<List<string>>();
                marks = await De_Serialization.Deserialisation<List<List<string>>>($"{para[1]}-{login[5]}.xml", CreationCollisionOption.OpenIfExists, marks);

                List<string> desiredday = new List<string>();
                foreach (List<string> day in marks)
                {
                    if (day[day.Count - 1] == RaspDatePicker.Date.ToString("dd.MM.yyyy"))
                        desiredday = day;
                }

                if (desiredday.Count != 0)
                {
                    foreach (string student in desiredday)
                    {
                        if (student.Contains(login[3]))
                        {
                            string[] getgrade = student.Split('|');
                            FinalGradeTextBlock.Text = getgrade[0];
                            break;
                        }
                        else
                        {
                            FinalGradeTextBlock.Text = "Не выставлено";
                        }
                    }
                }
                else
                {
                    FinalGradeTextBlock.Text = "Не выставлено";
                }
            }
        }

        private void RaspDatePicker_DataContextChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args)
        {
            {
                int indx = 0;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Monday") indx = 0;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Tuesday") indx = 1;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Wednesday") indx = 2;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Thursday") indx = 3;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Friday") indx = 4;
                if (RaspDatePicker.Date.DayOfWeek.ToString() == "Saturday") indx = 5;
                dayrasp = biglist[indx];
                RaspListBox.ItemsSource = new List<string>();
                RaspListBox.ItemsSource = dayrasp;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------------------Системные штучки
        private void CommandBar_Opening(object sender, object e) => (sender as CommandBar).IsOpen = false;
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack) this.Frame.GoBack();
        }
    }
}
