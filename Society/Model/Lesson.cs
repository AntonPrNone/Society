using System;

namespace Society.Model
{
    public class Lesson
    {
        public int ID_Lesson { get; set; }

        public int CabinetNumber { get; set; }

        public string Date { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int ID_Employee { get; set; }

        public int ID_Society { get; set; }

        public Employee Teacher { get; set; }
    }
}
