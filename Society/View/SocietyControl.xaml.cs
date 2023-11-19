using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Society.Model;

namespace Society.View
{
    public partial class SocietyControl : UserControl
    {
        List<SocietyClass> societyClasses = new List<SocietyClass>();
        private bool isSearchBoxEmpty = true;
        AddSocietyView addSocietyView;

        public SocietyControl()
        {
            InitializeComponent();
            societyClasses = DB_Interaction.GetSocieties();
            Society_ItemControl.ItemsSource = societyClasses;



        }

        private void AddSocietyView_SocietyAdded(object sender, EventArgs e)
        {
            // Обновите данные в вашем окне после добавления кружка
            societyClasses = DB_Interaction.GetSocieties();
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
            InfoSocietyView infoSocietyView = new InfoSocietyView(selectedSociety);

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
