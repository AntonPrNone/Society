using Society.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Linq;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для InfoSocietyView.xaml
    /// </summary>
    public partial class InfoSocietyView : Window
    {
        StudentsSocietyView studentsSocietyView;
        EditLessonView editLessonView;
        public event EventHandler SocietySaved;

        private SocietyClass _society;
        private List<Lesson> lessons = new List<Lesson>();
        private List<Student> students;
        private bool isSearchBoxEmpty = true;

        public InfoSocietyView(SocietyClass society)
        {
            InitializeComponent();

            _society = society;
            lessons = DB_Interaction.GetLessonsBySociety(_society.ID_Society);
            students = DB_Interaction.GetStudentsBySociety(_society.ID_Society);
            Name_TextBox.Text = society.Name;
            MaxStudent_TextBox.Text = society.MaxStudent.ToString();
            NumberHour_TextBox.Text = society.NumberHour.ToString();

            Lesson_ItemControl.ItemsSource = lessons;
            СountStudent_RunTextBlock.Text = $"{students.Count}/{_society.MaxStudent}";

            if (User.ID_Role == 1)
            {
                Save_Button.IsEnabled = false;
                Add_Button.IsEnabled = false;
            }
        }

        protected virtual void OnSocietySaved()
        {
            // Проверяем, есть ли подписчики на событие, и если есть, вызываем событие
            SocietySaved?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateLessons(object sender, EventArgs e)
        {
            lessons = DB_Interaction.GetLessonsBySociety(_society.ID_Society);
            Lesson_ItemControl.ItemsSource = lessons;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string name, out int maxStudent, out int numberHour))
            {
                bool newSocietyID = DB_Interaction.UpdateSociety(_society.ID_Society, name, maxStudent, numberHour);

                if (newSocietyID)
                {
                    OnSocietySaved();
                    _society = DB_Interaction.GetSocietyById(_society.ID_Society);
                    СountStudent_RunTextBlock.Text = $"{students.Count}/{_society.MaxStudent}";
                }

                else
                {
                    ErrorView errorView = new ErrorView("Ошибка", "Ошибка при добавлении кружка");
                    errorView.Show();
                }
            }
        }

        private bool ValidateInput(out string name, out int maxStudent, out int numberHour)
        {
            // Инициализация значений по умолчанию
            name = Name_TextBox.Text;
            maxStudent = 15; // Значение по умолчанию для MaxStudent
            numberHour = 0;

            // Очистка блоков с ошибками
            ErrorName_TextBlock.Text = "";
            ErrorMaxStudent_TextBlock.Text = "";
            ErrorNumberHour_TextBlock.Text = "";

            bool isValid = true;

            // Проверка наличия имени
            if (string.IsNullOrWhiteSpace(name))
            {
                ErrorName_TextBlock.Text = "Введите название кружка";
                isValid = false;
            }

            // Проверка корректности MaxStudent (если указано)
            if (!string.IsNullOrWhiteSpace(MaxStudent_TextBox.Text))
            {
                if (!int.TryParse(MaxStudent_TextBox.Text, out int maxStudentValue))
                {
                    ErrorMaxStudent_TextBlock.Text = "Максимальное количество учеников должно быть целым числом";
                    isValid = false;
                }

                else
                {
                    maxStudent = maxStudentValue;
                }
            }

            // Проверка корректности NumberHour
            if (!int.TryParse(NumberHour_TextBox.Text, out int numberHourValue))
            {
                ErrorNumberHour_TextBlock.Text = "Количество часов должно быть целым числом";
                isValid = false;
            }

            else
            {
                numberHour = numberHourValue;
            }

            return isValid;
        }


        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            studentsSocietyView = new StudentsSocietyView(_society.ID_Society);
            studentsSocietyView.StudentsInSocietyUpdated += StudentsView_DataUpdated;
            studentsSocietyView.Show();
        }

        private void StudentsView_DataUpdated(object sender, EventArgs e)
        {
            _society = DB_Interaction.GetSocietyById(_society.ID_Society);
            students = DB_Interaction.GetStudentsBySociety(_society.ID_Society);
            СountStudent_RunTextBlock.Text = $"{students.Count}/{_society.MaxStudent}";
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            editLessonView = new EditLessonView(-1, _society.ID_Society);
            editLessonView.LessonUpdate += UpdateLessons;
            editLessonView.Show();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = isSearchBoxEmpty ? "" : SearchTextBox.Text;
            isSearchBoxEmpty = false;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (lessons != null)
            {
                // Применяем фильтр к lessons
                List<Lesson> filteredLessons = lessons.Where(lesson =>
                    lesson.Date.ToLower().Contains(searchText) ||
                    lesson.StartTime.ToLower().Contains(searchText) ||
                    lesson.EndTime.ToLower().Contains(searchText) ||
                    lesson.CabinetNumber.ToString().Contains(searchText) ||
                    lesson.Teacher.Name.ToLower().Contains(searchText) ||
                    lesson.Teacher.Surname.ToLower().Contains(searchText) ||
                    lesson.Teacher.Patronymic.ToLower().Contains(searchText)
                ).ToList();

                // Обновляем отображение в ItemsControl
                Lesson_ItemControl?.Dispatcher.Invoke(() => Lesson_ItemControl.ItemsSource = filteredLessons);
            }
        }

        private void BorderItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Lesson lesson && User.ID_Role != 1)
            {
                editLessonView = new EditLessonView(lesson.ID_Lesson, _society.ID_Society);
                editLessonView.LessonUpdate += UpdateLessons;
                editLessonView.Show();
            }
        }

        //----------------------------------------------------------------------------------------------

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

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            // Создаем окно подтверждения
            ErrorView confirmationView = new ErrorView("Удаление", $"Подтвердите удаление кружка {_society.Name}");

            // Открываем окно и ждем подтверждения
            bool? result = confirmationView.ShowDialog();

            // Проверяем результат подтверждения
            if (result.HasValue && confirmationView.IsConfirmed)
            {
                // Если пользователь нажал "ОК" в окне подтверждения
                bool isDeleted = DB_Interaction.DeleteSociety(_society.ID_Society);

                if (isDeleted)
                {
                    OnSocietySaved();
                    this.Close();
                }
                else
                {
                    // Если произошла ошибка при удалении, выводим сообщение об ошибке
                    ErrorView errorView = new ErrorView("Ошибка", "Ошибка при удалении кружка");
                    errorView.Show();
                }
            }
        }

    }
}
