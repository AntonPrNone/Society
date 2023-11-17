using Society.Model;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private bool StateAuth_Log = true;
        private int StatePage = 0;

        private bool _isAnimationRunning = false;

        private string LoginText;
        private string PasswordText;
        private string NameText;
        private string SurnameText;
        private string PatronymicText;
        public LoginView()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Reg_TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StateAuth_Log = !StateAuth_Log;
            if (StatePage != 0)
            {
                SwitchForms((StatePage == 1 ? Form2_StackPanel : Form3_StackPanel), Form1_StackPanel);
                StatePage = 0;
            }

            if (StateAuth_Log)
            {
                ChangeLabel_TextBlock.Text = "Не зарегистрированы?";
                ChangeLabel2_TextBlock.Text = "Регистрация";
            }

            else
            {
                ChangeLabel_TextBlock.Text = "Зарегистрированы?";
                ChangeLabel2_TextBlock.Text = "Войти";
            }
        }

        private void Log_Button_Click(object sender, RoutedEventArgs e)
        {
            if (StateAuth_Log) Log();
            else Reg();
        }

        private void Log()
        {
            LoginText = Login_TextBox.Text;
            PasswordText = Password_TextBox.Password;

            if (Valid_LogPas(LoginText, PasswordText))
            {
                var result = DB_Connect.CheckLogin(LoginText, PasswordText);
                if (result.success)
                {
                    var result2 = DB_Connect.GetFillEmployeeByLogin(LoginText);
                    if (result2.success)
                    {

                        User.SaveEmployeeID();
                        HomeView homeView = new HomeView();
                        homeView.Show();
                    }

                    else
                    {
                        ShowErrorMessage(result2.errorMessage);
                    }
                }

                else
                {
                    ShowErrorMessage(result.errorMessage);
                }
            }
        }

        private void Reg()
        {
            ErrorLogin_TextBlock.Text = "";
            ErrorPassword_TextBlock.Text = "";
            ErrorName_TextBlock.Text = "";
            ErrorSurname_TextBlock.Text = "";
            ErrorPatronymic_TextBlock.Text = "";

            if (StatePage == 0)
            {
                LoginText = Login_TextBox.Text;
                PasswordText = Password_TextBox.Password;

                if (Valid_LogPas(LoginText, PasswordText))
                {
                    var result = DB_Connect.LoginNoExists(LoginText);
                    if (result.success)
                    {
                        StatePage = 1;
                        SwitchForms(Form1_StackPanel, Form2_StackPanel);
                    }

                    else
                    {
                        ShowErrorMessage(result.errorMessage);
                    }
                }
            }

            else if (StatePage == 1)
            {
                NameText = Name_TextBox.Text;
                SurnameText = Surname_TextBox.Text;
                if (ValidName(NameText, ErrorName_TextBlock) & 
                    ValidName(SurnameText, ErrorSurname_TextBlock))
                {
                    StatePage = 2;
                    SwitchForms(Form2_StackPanel, Form3_StackPanel);
                }
            }

            else if (StatePage == 2)
            {
                PatronymicText = Patronymic_TextBox.Text;
                if (ValidName(PatronymicText, ErrorPatronymic_TextBlock))
                {
                    var result = DB_Connect.AddEmployee(NameText, SurnameText, PatronymicText, LoginText, PasswordText);
                    if (result.success)
                    {
                        var result2 = DB_Connect.GetFillEmployeeByLogin(LoginText);
                        if (result2.success)
                        {
                            User.SaveEmployeeID();
                            HomeView homeView = new HomeView();
                            homeView.Show();
                        }

                        else
                        {
                            ShowErrorMessage(result2.errorMessage);
                        }

                    }

                    else
                    {
                        ShowErrorMessage(result.errorMessage);
                    }
                }
            }
        }

        private void ShowErrorMessage(string errorMessage)
        {
            Mess_TextBlock.Text = errorMessage;
            AnimateBorder();
        }

        private bool Valid_LogPas(string loginText, string passwordText)
        {
            bool correct = false;
            ErrorLogin_TextBlock.Text = "";
            ErrorPassword_TextBlock.Text = "";

            if ((!string.IsNullOrWhiteSpace(loginText) && !string.IsNullOrWhiteSpace(passwordText)) &&
        (System.Text.RegularExpressions.Regex.IsMatch(loginText, @"^[a-zA-Z0-9!@#$%^&*()_+-=]+$")) &&
        (System.Text.RegularExpressions.Regex.IsMatch(passwordText, @"^[a-zA-Z0-9!@#$%^&*()_+-=]+$")))
            {
                correct = true;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(loginText))
                {
                    ErrorLogin_TextBlock.Text = "Поле логина пусто";
                    correct = false;
                }

                else if (!(System.Text.RegularExpressions.Regex.IsMatch(loginText, @"^[a-zA-Z0-9!@#$%^&*()_+-=]+$")))
                {
                    ErrorLogin_TextBlock.Text = "Логин иожет содержать латиницу, цифры и спецсимволы";
                    correct = false;
                }

                if (string.IsNullOrWhiteSpace(passwordText))
                {
                    ErrorPassword_TextBlock.Text = "Поле пароля пусто";
                    correct = false;
                }

                else if (!(System.Text.RegularExpressions.Regex.IsMatch(passwordText, @"^[a-zA-Z0-9!@#$%^&*()_+-=]+$")))
                {
                    ErrorPassword_TextBlock.Text = "Пароль может содержать латиницу, цифры и спецсимволы";
                    correct = false;
                }
            }

            return correct;
        }

        private bool ValidName(string name, TextBlock textBlock)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                if (textBlock.Name == "ErrorPatronymic_TextBlock") return true;
                textBlock.Text = "Поле пусто";
                return false;
            }

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != '-')
                {
                    textBlock.Text = "Поле может содержать буквы и дефис";
                    return false;
                }
            }

            return true;
        }

        private void AnimateBorder()
        {
            if (_isAnimationRunning) return;

            _isAnimationRunning = true;

            // Очищаем все анимации перед добавлением новых
            BorderMess_Border.BeginAnimation(UIElement.OpacityProperty, null);
            BorderMess_Border.BeginAnimation(TranslateTransform.XProperty, null);

            BorderMess_Border.Opacity = 1;

            TranslateTransform translateTransform = new TranslateTransform();
            BorderMess_Border.RenderTransform = translateTransform;

            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                From = ActualWidth + 20,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.75)
            };

            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };

            moveAnimation.Completed += async (sender, e) =>
            {
                await Task.Delay(5000);

                opacityAnimation.Completed += (opacitySender, opacityE) =>
                {
                    _isAnimationRunning = false;
                };

                BorderMess_Border.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
            };

            translateTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
        }

        private async void SwitchForms(StackPanel formToFadeOut, StackPanel formToFadeIn)
        {
            // Очистка анимаций перед началом
            formToFadeOut.BeginAnimation(UIElement.OpacityProperty, null);
            formToFadeIn.BeginAnimation(UIElement.OpacityProperty, null);

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(1)
            };

            formToFadeOut.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            await Task.Delay(1000);

            formToFadeOut.Visibility = Visibility.Collapsed;
            formToFadeIn.Visibility = Visibility.Visible;

            formToFadeIn.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }
    }
}
