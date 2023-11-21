using Society.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для AddStudentInSocietyView.xaml
    /// </summary>
    public partial class AddStudentInSocietyView : Window
    {
        public event EventHandler StudentSocietyLinkAdded;

        SocietyClass _society;
        List<Student> students;

        public AddStudentInSocietyView(int societyId)
        {
            InitializeComponent();

            _society = DB_Interaction.GetSocietyById(societyId);
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            _society = DB_Interaction.GetSocietyById(_society.ID_Society);
            students = DB_Interaction.GetStudentsBySociety(_society.ID_Society);

            if (ValidateInput(ID_TextBox.Text))
            {
                if (students.Count < _society.MaxStudent)
                {
                    var result = DB_Interaction.AddStudentSocietyLink(Int32.Parse(ID_TextBox.Text), _society.ID_Society);

                    if (result.success)
                    {
                        this.Close();
                        OnStudentSocietyLinkAdded();
                    }

                    else
                    {
                        ErrorId_TextBlock.Text = result.errorMessage;
                    }

                }

                else
                {
                    ErrorId_TextBlock.Text = "Количество учеников в кружке равно лимиту";
                }
            }
        }


        private bool ValidateInput(string id)
        {
            ErrorId_TextBlock.Text = "";
            if (!int.TryParse(id, out _))
            {
                ErrorId_TextBlock.Text = "Введите число";
                return false;
            }

            return true;
        }

        protected virtual void OnStudentSocietyLinkAdded()
        {
            // Проверяем, есть ли подписчики на событие, и если есть, вызываем событие
            StudentSocietyLinkAdded?.Invoke(this, EventArgs.Empty);
        }

        // ------------------------------------------------------------------------------------

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
