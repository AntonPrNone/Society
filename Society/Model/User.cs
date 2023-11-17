using System.IO;

namespace Society.Model
{
    public static class User
    {
        public static int ID_Employee { get; set; } = -1;
        public static string Name { get; set; }
        public static string Surname { get; set; }
        public static string Patronymic { get; set; }
        public static string Login { get; set; }
        public static string Password { get; set; }
        public static int ID_Role { get; set; } = 1;

        public static void SaveEmployeeID()
        {
            File.WriteAllText("employeeID.txt", ID_Employee.ToString());
        }

        public static void LoadEmployeeData()
        {
            if (File.Exists("employeeID.txt"))
            {
                string content = File.ReadAllText("employeeID.txt");
                if (int.TryParse(content, out int id))
                {
                    ID_Employee = id;
                    if (!DB_Connect.LoadEmployeeData())
                    {
                        ClearEmployeeID();
                    }

                    return;
                }

                else
                {
                    ClearEmployeeID();
                }
            }
        }

        public static void ClearEmployeeID()
        {
            ID_Employee = -1;
            SaveEmployeeID();
        }
    }
}
