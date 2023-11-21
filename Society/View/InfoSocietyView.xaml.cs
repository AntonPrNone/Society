using Society.Model;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

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
        private List<Lesson> lessons;
        private List<Student> students;
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
                    MessageBox.Show("Ошибка при добавлении кружка.");
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

        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BorderItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Lesson lesson)
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
    }
}
