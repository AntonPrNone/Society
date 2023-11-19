using Society.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Society.View
{
    public partial class TeacherControl : UserControl
    {
        DataTable dataTable;
        private bool isSearchBoxEmpty = true;

        public TeacherControl()
        {
            InitializeComponent();
            LoadDataGrid();
            Teacher_DataGrid.Loaded += (sender, e) => CustomizationTables();
        }

        private void LoadDataGrid()
        {
            dataTable = new DataTable("Employees");
            List<Employee> employees = DB_Interaction.GetEmployees();

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("ID", typeof(int)),
                new DataColumn("Имя", typeof(string)),
                new DataColumn("Фамилия", typeof(string)),
                new DataColumn("Отчество", typeof(string)),
            });

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
            DB_Interaction.UpdateEmployees(dataTable);
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataTable == null)
            {
                return;
            }

            string searchText = SearchTextBox.Text;

            DataTable filteredDataTable = FilterDataTable(searchText);

            Teacher_DataGrid.ItemsSource = filteredDataTable.DefaultView;

            if (Teacher_DataGrid.Columns.Count > 0)
            {
                CustomizationTables();
            }
        }

        private DataTable FilterDataTable(string searchText)
        {
            DataTable filteredDataTable = dataTable.Clone();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var columnValue in row.ItemArray)
                {
                    if (columnValue.ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        filteredDataTable.ImportRow(row);
                        break;
                    }
                }
            }

            return filteredDataTable;
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = isSearchBoxEmpty ? "" : SearchTextBox.Text;
            isSearchBoxEmpty = false;
        }

        private void Reser_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            CustomizationTables();
        }
    }
}
