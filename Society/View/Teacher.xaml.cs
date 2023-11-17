using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для Teacher.xaml
    /// </summary>
    public partial class Teacher : System.Windows.Controls.UserControl
    {
        DataTable dataTable;
        private bool isSearchBoxEmpty = true;

        public Teacher()
        {
            InitializeComponent();

            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            dataTable = new DataTable("Employees");
            List<Employee> employees = DB_Connect.GetEmployees();

            dataTable.Columns.Add("ID", typeof(int));
            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("Фамилия", typeof(string));
            dataTable.Columns.Add("Отчество", typeof(string));

            foreach (Employee employee in employees)
            {
                dataTable.Rows.Add(
                    employee.ID_Employee,
                    employee.Name,
                    employee.Surname,
                    employee.Patronymic
                );
            }

            Teacher_DataGrid.ItemsSource = dataTable.DefaultView;

            Teacher_DataGrid.Loaded += (sender, e) => CustomizationTables();
        }

        private void CustomizationTables()
        {
            var columns = Teacher_DataGrid.Columns;
            DataGridColumn columnID = columns.FirstOrDefault(c => c.Header?.ToString() == "ID");
            if (columnID != null)
            {
                columnID.Width = 50;
                columnID.IsReadOnly = true;
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            DB_Connect.UpdateEmployees(dataTable);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем, что dataTable не является null
            if (dataTable == null)
            {
                // Можно выполнить какие-то дополнительные действия или просто выйти из метода
                return;
            }

            string searchText = SearchTextBox.Text;

            // Создаем новый DataTable для хранения совпадающих строк
            DataTable filteredDataTable = dataTable.Clone();

            // Фильтруем строки в оригинальном DataTable и добавляем совпадающие в новый DataTable
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var columnValue in row.ItemArray)
                {
                    if (columnValue.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filteredDataTable.ImportRow(row);
                        break; // Добавляем строку только один раз
                    }
                }
            }

            // Устанавливаем новый DataTable в качестве источника данных для DataGrid
            Teacher_DataGrid.ItemsSource = filteredDataTable.DefaultView;

            CustomizationTables();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isSearchBoxEmpty)
            {
                SearchTextBox.Text = "";
                isSearchBoxEmpty = false;
            }
        }

        private void Reser_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            CustomizationTables();
        }
    }
}
