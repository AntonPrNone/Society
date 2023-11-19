using System;

namespace Society.Model
{
    public class Lesson
    {
        public int ID_Lesson { get; set; }
        public int CabinetNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ID_Employee { get; set; }
        public int ID_Society { get; set; }
    }
}
