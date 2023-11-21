using Society.Logic;
using Society.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для EditLessonView.xaml
    /// </summary>
    public partial class EditLessonView : Window
    {
        public event EventHandler LessonUpdate;

        Lesson lesson;
        int _id_lesson;
        int _id_society;
        public EditLessonView(int id_lesson, int id_society)
        {
            InitializeComponent();

            _id_lesson = id_lesson;
            _id_society = id_society;

            if (id_lesson != -1)
            {
                lesson = DB_Interaction.GetLessonById(id_lesson);

                Date_TextBox.Text = lesson.Date;
                StartTime_TextBox.Text = lesson.StartTime;
                EndTime_TextBox.Text = lesson.EndTime;
                CabinetNumber_TextBox.Text = lesson.CabinetNumber.ToString();
                ID_Employee_TextBox.Text = lesson.ID_Employee.ToString();
            }
        }

        protected virtual void OnLessonUpdate()
        {
            // Проверяем, есть ли подписчики на событие, и если есть, вызываем событие
            LessonUpdate?.Invoke(this, EventArgs.Empty);
        }

        private bool ValidateData()
        {
            // Очищаем текст блоки ошибок перед новой валидацией
            ClearErrorTextBlocks();

            // Переменная для хранения статуса валидации
            bool isValid = true;

            // Проверка валидности даты
            if (!PsevdoDateTime.IsValidPseudoDate(Date_TextBox.Text))
            {
                ErrorDate_TextBlock.Text = "Неверный формат даты";
                isValid = false;
            }

            // Проверка валидности времени начала
            if (!PsevdoDateTime.IsValidPseudoTime(StartTime_TextBox.Text))
            {
                ErrorStartTime_TextBlock.Text = "Неверный формат времени начала";
                isValid = false;
            }

            // Проверка валидности времени конца
            if (!PsevdoDateTime.IsValidPseudoTime(EndTime_TextBox.Text))
            {
                ErrorEndTime_TextBlock.Text = "Неверный формат времени конца";
                isValid = false;
            }

            // Проверка, является ли строка числом для номера кабинета
            if (!int.TryParse(CabinetNumber_TextBox.Text, out _))
            {
                ErrorCabinetNumber_TextBlock.Text = "Неверный формат номера кабинета";
                isValid = false;
            }

            // Проверка, существует ли сотрудник с указанным ID
            int employeeId;
            if (!int.TryParse(ID_Employee_TextBox.Text, out employeeId) || !DB_Interaction.EmployeeExists(employeeId))
            {
                ErrorID_Employee_TextBlock.Text = "Неверный ID сотрудника";
                isValid = false;
            }

            return isValid;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            // Перед сохранением проверяем валидность данных
            if (ValidateData())
            {
                if (_id_lesson != -1)
                {
                    DB_Interaction.UpdateLesson(new Lesson {
                        ID_Lesson = lesson.ID_Lesson, CabinetNumber = int.Parse(CabinetNumber_TextBox.Text), 
                        Date = Date_TextBox.Text, StartTime = StartTime_TextBox.Text, 
                        EndTime = EndTime_TextBox.Text, ID_Employee = int.Parse(ID_Employee_TextBox.Text), 
                        ID_Society = lesson.ID_Society 
                    });

                    OnLessonUpdate();
                    Close();
                }

                else
                {
                    DB_Interaction.SaveLesson(new Lesson
                    {
                        CabinetNumber = int.Parse(CabinetNumber_TextBox.Text),
                        Date = Date_TextBox.Text,
                        StartTime = StartTime_TextBox.Text,
                        EndTime = EndTime_TextBox.Text,
                        ID_Employee = int.Parse(ID_Employee_TextBox.Text),
                        ID_Society = _id_society
                    });

                    OnLessonUpdate();
                    Close();
                }
            }
        }

        private void ClearErrorTextBlocks()
        {
            // Очищаем текст блоки ошибок
            ErrorDate_TextBlock.Text = "";
            ErrorStartTime_TextBlock.Text = "";
            ErrorEndTime_TextBlock.Text = "";
            ErrorCabinetNumber_TextBlock.Text = "";
            ErrorID_Employee_TextBlock.Text = "";
        }

        //----------------------------------------------------------------------------------------------

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void pnlControlBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
