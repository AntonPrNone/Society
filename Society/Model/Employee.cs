namespace Society.Model
{
    public class Employee
    {
        public int ID_Employee { get; set; } = -1;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int ID_Role { get; set; } = 1;
    }
}
