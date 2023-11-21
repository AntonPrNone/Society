using Society.Logic;
using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Society.View
{
    /// <summary>
    /// Логика взаимодействия для Student.xaml
    /// </summary>
    public partial class StudentControl : UserControl
    {
        DataTable dataTable;
        private bool isSearchBoxEmpty = true;

        public StudentControl()
        {
            InitializeComponent();
            LoadDataGrid();
            Student_DataGrid.Loaded += (sender, e) => CustomizationTables();

            if (User.ID_Role == 1)
            {
                Save_Button.IsEnabled = false;
            }
        }

        private void LoadDataGrid()
        {
            dataTable = new DataTable("Students");
            List<Student> students = DB_Interaction.GetStudents();

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("ID", typeof(int)),
                new DataColumn("Имя", typeof(string)),
                new DataColumn("Фамилия", typeof(string)),
                new DataColumn("Отчество", typeof(string)),
                new DataColumn("Дата рождения", typeof(string)) // Добавлено поле DateOfBirth
            });

            foreach (Student student in students)
            {
                dataTable.Rows.Add(
                    student.ID_Student,
                    student.Name,
                    student.Surname,
                    student.Patronymic,
                    student.DateOfBirth // Добавлено поле DateOfBirth
                );
            }

            Student_DataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void CustomizationTables()
        {
            var columns = Student_DataGrid.Columns;
            DataGridColumn columnID = columns.FirstOrDefault(c => c.Header?.ToString() == "ID");
            if (columnID != null)
            {
                columnID.Width = 50;
                columnID.IsReadOnly = true;
            }
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                DB_Interaction.UpdateStudents(dataTable);
            }
        }

        private bool ValidateData()
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string pseudoDate = row["Дата рождения"].ToString();

                // Проверка валидности псевдодаты
                if (!PsevdoDateTime.IsValidPseudoDate(pseudoDate))
                {
                    ErrorView errorView = new ErrorView("Ошибка", "Неверный формат записи даты");
                    return false;
                }
            }

            return true;
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataTable == null)
            {
                return;
            }

            string searchText = SearchTextBox.Text;

            DataTable filteredDataTable = FilterDataTable(searchText);

            Student_DataGrid.ItemsSource = filteredDataTable.DefaultView;

            if (Student_DataGrid.Columns.Count > 0)
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
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
            CustomizationTables();
        }

        private void Student_DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            // Получаем максимальное значение ID в текущем DataTable
            int maxId = dataTable.AsEnumerable().Max(row => row.Field<int>("ID"));

            // Увеличиваем на 1 и присваиваем новое значение для следующей строки
            int newId = maxId + 1;

            // Присваиваем новое значение полю ID в текущей добавляемой строке
            var newRow = e.NewItem as DataRowView;
            if (newRow != null)
            {
                newRow["ID"] = newId;
            }
        }
    }
}
