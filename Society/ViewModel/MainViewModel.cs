using Society.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Society.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        Teacher teacher = new Teacher();
        Student student = new Student();

        private UserControl _currentChildView;
        public UserControl CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        private bool _isPage1Selected;
        public bool IsPage1Selected
        {
            get => _isPage1Selected;
            set
            {
                _isPage1Selected = value;
                if (value)
                {
                    // При выборе страницы 1 устанавливаем соответствующий UserControl
                    CurrentChildView = teacher;
                }
                OnPropertyChanged(nameof(IsPage1Selected));
            }
        }

        private bool _isPage2Selected;
        public bool IsPage2Selected
        {
            get => _isPage2Selected;
            set
            {
                _isPage2Selected = value;
                if (value)
                {
                    // При выборе страницы 2 устанавливаем соответствующий UserControl
                    CurrentChildView = student;
                }
                OnPropertyChanged(nameof(IsPage2Selected));
            }
        }

        // Добавьте аналогичные свойства для каждой страницы

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
