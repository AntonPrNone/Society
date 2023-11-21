using Society.Logic;
using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Society.View
{
    public partial class SocietyControl : UserControl
    {
        List<SocietyClass> societyClasses = new List<SocietyClass>();
        private bool isSearchBoxEmpty = true;
        AddSocietyView addSocietyView;
        InfoSocietyView infoSocietyView;

        public SocietyControl()
        {
            InitializeComponent();
            societyClasses = DB_Interaction.GetSocieties();

            if (User.ID_Role == 1)
            {
                societyClasses = DB_Interaction.GetSocietiesByEmployeeId(User.ID_Employee);
                Add_Button.IsEnabled = false;
            }

            Society_ItemControl.ItemsSource = societyClasses;
        }

        private void AddSocietyView_SocietyAdded(object sender, EventArgs e)
        {
            societyClasses = DB_Interaction.GetSocieties();
            if (User.ID_Role == 1)
            {
                societyClasses = DB_Interaction.GetSocietiesByEmployeeId(User.ID_Employee);
            }

            Society_ItemControl.ItemsSource = societyClasses;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            var filteredSocieties = societyClasses
                .Where(society => society.Name.ToLower().Contains(searchText))
                .ToList();

            // Проверяем, что Society_ItemControl не является null перед обновлением ItemsSource
            Society_ItemControl?.Dispatcher.Invoke(() => Society_ItemControl.ItemsSource = filteredSocieties);
        }


        private void SearchTextBox_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            SearchTextBox.Text = isSearchBoxEmpty ? "" : SearchTextBox.Text;
            isSearchBoxEmpty = false;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Получаем выбранный объект SocietyClass из DataContext Border'а
            SocietyClass selectedSociety = (SocietyClass)((Border)sender).DataContext;

            // Создаем новое окно
            infoSocietyView = new InfoSocietyView(selectedSociety);
            infoSocietyView.SocietySaved += AddSocietyView_SocietyAdded;

            // Открываем новое окно
            infoSocietyView.Show();
        }

        private void Add_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            addSocietyView = new AddSocietyView();
            addSocietyView.SocietyAdded += AddSocietyView_SocietyAdded;

            addSocietyView.Show();
        }
    }
}
