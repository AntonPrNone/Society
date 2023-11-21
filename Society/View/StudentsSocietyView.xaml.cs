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
        AddStudentInSocietyView addStudentInSociety;
        public event EventHandler StudentsInSocietyUpdated;

        List<Student> students;
        SocietyClass _society;
        private bool isSearchBoxEmpty = true;


        public StudentsSocietyView(int societyId)
        {
            InitializeComponent();

            _society = DB_Interaction.GetSocietyById(societyId);
            students = DB_Interaction.GetStudentsBySociety(societyId);

            Student_ItemControl.ItemsSource = students;
        }

        private void AddSocietyView_StudentSocietyLinkAdded(object sender, EventArgs e)
        {
            // Обновляем данные после успешного добавления связи
            UpdateStudentsData();
        }

        private void UpdateStudentsData()
        {
            _society = DB_Interaction.GetSocietyById(_society.ID_Society);
            students = DB_Interaction.GetStudentsBySociety(_society.ID_Society);

            Student_ItemControl.ItemsSource = null;
            Student_ItemControl.ItemsSource = students;

            OnStudentsInSocietyUpdated();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            var filteredStudents = students?
                .Where(student =>
                    student != null &&
                    (
                        student.ID_Student.ToString().Contains(searchText) ||
                        student.Name.ToLower().Contains(searchText) ||
                        student.Surname.ToLower().Contains(searchText) ||
                        student.Patronymic.ToLower().Contains(searchText) ||
                        student.DateOfBirth.ToString().Contains(searchText)
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

        private void Add_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            addStudentInSociety = new AddStudentInSocietyView(_society.ID_Society);
            addStudentInSociety.StudentSocietyLinkAdded += AddSocietyView_StudentSocietyLinkAdded;

            addStudentInSociety.Show();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Button deleteButton = (Button)sender;
            Student student = (Student)deleteButton.DataContext;

            if (student != null)
            {
                var result = DB_Interaction.DeleteStudentSocietyLink(student.ID_Student, _society.ID_Society);

                if (result.success)
                {
                    students.Remove(student);

                    Student_ItemControl.ItemsSource = null;
                    Student_ItemControl.ItemsSource = students;

                    OnStudentsInSocietyUpdated();

                }

                else
                {
                    MessageBox.Show($"Ошибка при удалении: {result.errorMessage}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected virtual void OnStudentsInSocietyUpdated()
        {
            StudentsInSocietyUpdated?.Invoke(this, EventArgs.Empty);
        }

        //--------------------------------------------------------------------------------------------

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
    }
}
