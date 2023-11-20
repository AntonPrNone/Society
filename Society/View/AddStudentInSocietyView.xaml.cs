using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для AddStudentInSocietyView.xaml
    /// </summary>
    public partial class AddStudentInSocietyView : Window
    {
        public event EventHandler SocietyAdded;

        public AddStudentInSocietyView()
        {
            InitializeComponent();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput(ID_TextBox.Text))
            {
                // Вызываем метод добавления
                int newSocietyID = DB_Interaction.AddSociety(name, maxStudent, numberHour);
                DB_Interaction.AddStudentSocietyLink(ID_TextBox.Text, );

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


        private bool ValidateInput(string id)
        {
            ErrorId_TextBlock.Text = "";
            if (!int.TryParse(id, out _))
            {
                ErrorId_TextBlock.Text = "Введите число";
                return false;
            }

            // Валидация успешна
            return true;
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
