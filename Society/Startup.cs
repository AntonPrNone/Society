using Society.Model;
using Society.View;
using System;
using System.IO;

namespace Society
{
    internal static class Startup
    {
        private const string EmployeeIdFilePath = "employeeID.txt";

        public static void Start()
        {
            if (File.Exists(EmployeeIdFilePath))
            {
                LoadEmployeeDataFromFile();
            }

            if (User.ID_Employee == -1)
            {
                LoginView loginView = new LoginView();
                loginView.Show();
            }

            else
            {
                HomeView homeView = new HomeView();
                homeView.Show();
            }
        }

        private static void LoadEmployeeDataFromFile()
        {
            string content = File.ReadAllText(EmployeeIdFilePath);
            if (int.TryParse(content, out int id))
            {
                User.ID_Employee = id;
                User.LoadEmployeeData();
            }

            else
            {
                User.ClearEmployeeID();
            }
        }
    }
}
