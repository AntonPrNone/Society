using System;
using System.Windows;
using System.Windows.Input;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для AddSocietyView.xaml
    /// </summary>
    public partial class AddSocietyView : Window
    {
        public event EventHandler SocietyAdded;

        public AddSocietyView()
        {
            InitializeComponent();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(out string name, out int maxStudent, out int numberHour))
            {
                // Вызываем метод добавления
                int newSocietyID = DB_Interaction.AddSociety(name, maxStudent, numberHour);

                if (newSocietyID != -1)
                {
                    // Закрываем окно после успешного добавления
                    this.Close();

                    OnSocietyAdded();

                }

                else
                {
                    MessageBox.Show("Ошибка при добавлении кружка.");
                }
            }
        }

        protected virtual void OnSocietyAdded()
        {
            // Проверяем, есть ли подписчики на событие, и если есть, вызываем событие
            SocietyAdded?.Invoke(this, EventArgs.Empty);
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
