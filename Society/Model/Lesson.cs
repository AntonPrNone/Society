using System;

namespace Society.Model
{
    public class Lesson
    {
        public int ID_Lesson { get; set; }
        public int CabinetNumber { get; set; }
        private DateTime _date;
        private TimeSpan _startTime;
        private TimeSpan _endTime;

        // Свойство для получения и установки только даты
        public string Date
        {
            get => _date.ToString("d");
            set => _date = DateTime.Parse(value);
        }

        // Преобразование TimeSpan в строку в формате "hh:mm"
        public string StartTime
        {
            get => _startTime.ToString("hh\\:mm");
            set => _startTime = TimeSpan.Parse(value);
        }

        // Преобразование TimeSpan в строку в формате "hh:mm"
        public string EndTime
        {
            get => _endTime.ToString("hh\\:mm");
            set => _endTime = TimeSpan.Parse(value);
        }

        public int ID_Employee { get; set; }
        public int ID_Society { get; set; }
    }
}
