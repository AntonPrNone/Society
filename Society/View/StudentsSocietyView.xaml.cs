using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для StudentsSocietyView.xaml
    /// </summary>
    public partial class StudentsSocietyView : Window
    {
        public event EventHandler DataUpdated;

        List<Student> _students;
        int _id_Society;

        private bool isSearchBoxEmpty = true;
        AddSocietyView addSocietyView;
        InfoSocietyView infoSocietyView;
        public StudentsSocietyView(List<Student> students, int id_Society)
        {
            InitializeComponent();

            _id_Society = id_Society;
            _students = students ?? new List<Student>(); // Гарантируем, что _students не является null

            Student_ItemControl.ItemsSource = _students;
        }

        private void AddSocietyView_SocietyAdded(object sender, EventArgs e)
        {
            //// Обновите данные в вашем окне после добавления кружка
            //societyClasses = DB_Interaction.GetSocieties();
            //Society_ItemControl.ItemsSource = societyClasses;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            var filteredStudents = _students?
                .Where(student =>
                    student != null &&
                    (
                        student.ID_Student.ToString().Contains(searchText) ||
                        student.Name.ToLower().Contains(searchText) ||
                        student.Surname.ToLower().Contains(searchText) ||
                        student.Patronymic.ToLower().Contains(searchText) ||
                        student.DateOfBirth.ToString("dd.MM.yyyy").Contains(searchText) // Пример для поиска по дате рождения в формате "dd.MM.yyyy"
                                                                                        // Добавьте другие поля, если необходимо
                    )
                )
                .ToList();

            // Проверяем, что Society_ItemControl не является null перед обновлением ItemsSource
            Student_ItemControl?.Dispatcher.Invoke(() => Student_ItemControl.ItemsSource = filteredStudents);
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


        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void pnlControlBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }

        private void pnlControlBar_MouseEnter(object sender, MouseEventArgs e)
        {

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else this.WindowState = WindowState.Normal;
        }

        private void Reser_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, на которую было произведено нажатие
            Button deleteButton = (Button)sender;

            // Получаем контекст данных (элемент, к которому привязана кнопка)
            Student student = (Student)deleteButton.DataContext;

            if (student != null)
            {
                // Попытка удаления связи из базы данных
                var result = DB_Interaction.DeleteStudentSocietyLink(student.ID_Student, _id_Society);

                if (result.success)
                {
                    // Успешное удаление из базы данных
                    // Удаляем элемент из коллекции
                    _students.Remove(student);

                    // Обновляем источник данных для ItemsControl
                    Student_ItemControl.ItemsSource = null;
                    Student_ItemControl.ItemsSource = _students;

                    OnDataUpdated();

                }

                else
                {
                    // Обработка ошибки (result.errorMessage содержит текст ошибки)
                    MessageBox.Show($"Ошибка при удалении: {result.errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected virtual void OnDataUpdated()
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }

    }
}
