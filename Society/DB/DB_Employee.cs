using Society.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

public static partial class DB_Interaction
{
    // ------------------------------------------------ Учитель ------------------------------------------------
    public static List<Employee> GetEmployees()
    {
        OpenConnection();

        List<Employee> employees = new List<Employee>();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM Employee_table";

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Employee employee = new Employee
                        {
                            ID_Employee = Convert.ToInt32(reader["ID_Employee"]),
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Patronymic = reader["Patronymic"].ToString(),
                            Login = reader["Login"].ToString(),
                            Password = reader["Password"].ToString(),
                            ID_Role = Convert.ToInt32(reader["ID_Role"])
                        };

                        employees.Add(employee);
                    }

                    return employees;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных сотрудников: {ex.Message}");
                return null;
            }
        }
    }

    public static (bool success, string errorMessage) UpdateEmployees(DataTable dataTable)
    {
        OpenConnection();

        try
        {
            foreach (DataRow row in dataTable.Rows)
            {
                int id = (int)row["ID"];
                string name = (string)row["Имя"];
                string surname = (string)row["Фамилия"];
                string patronymic = (string)row["Отчество"];

                // Если сотрудник существует, обновляем его данные
                if (EmployeeExists(id))
                {
                    UpdateEmployee(id, name, surname, patronymic);
                }
                else
                {
                    // В противном случае, сотрудника можно добавить, если требуется
                    // AddEmployee(name, surname, patronymic, "новый логин", "новый пароль");
                }
            }

            // Удаляем сотрудников, которых нет в переданном DataTable
            DeleteNonExistingEmployees(dataTable);

            return (true, null);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении данных сотрудников: {ex.Message}");
            return (false, "Произошла ошибка при обновлении данных сотрудников");
        }
    }

    public static (bool success, string errorMessage) UpdateEmployee(int id, string name, string surname, string patronymic)
    {
        OpenConnection();

        using (SqlTransaction transaction = _connection.BeginTransaction())
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;

                try
                {
                    // Проверяем, существует ли запись с указанным ID в базе данных
                    cmd.CommandText = "SELECT COUNT(*) FROM Employee_table WHERE ID_Employee = @ID";
                    cmd.Parameters.AddWithValue("@ID", id);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Если запись существует, обновляем ее данные
                        cmd.Parameters.Clear();
                        cmd.CommandText = "UPDATE Employee_table SET Name = @Name, Surname = @Surname, Patronymic = @Patronymic WHERE ID_Employee = @ID";
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Surname", surname);
                        cmd.Parameters.AddWithValue("@Patronymic", patronymic);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        return (true, null);
                    }
                    else
                    {
                        transaction.Rollback();
                        return (false, "Запись с указанным ID не найдена");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при обновлении данных сотрудника: {ex.Message}");
                    return (false, "Произошла ошибка при обновлении данных сотрудника");
                }
            }
        }
    }

    private static bool EmployeeExists(int id)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT COUNT(*) FROM Employee_table WHERE ID_Employee = @ID";
            cmd.Parameters.AddWithValue("@ID", id);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    private static void DeleteNonExistingEmployees(DataTable dataTable)
    {
        List<int> existingEmployeeIds = GetEmployeeIds();

        // Определяем сотрудников, которых нет в переданном DataTable
        List<int> nonExistingEmployeeIds = existingEmployeeIds.Except(dataTable.AsEnumerable().Select(row => row.Field<int>("ID"))).ToList();

        foreach (int id in nonExistingEmployeeIds)
        {
            // Удаляем сотрудника из базы данных
            DeleteEmployee(id);
        }
    }

    private static List<int> GetEmployeeIds()
    {
        List<int> employeeIds = new List<int>();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT ID_Employee FROM Employee_table";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    employeeIds.Add(Convert.ToInt32(reader["ID_Employee"]));
                }
            }
        }

        return employeeIds;
    }

    private static void DeleteEmployee(int id)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "DELETE FROM Employee_table WHERE ID_Employee = @ID";
            cmd.Parameters.AddWithValue("@ID", id);

            cmd.ExecuteNonQuery();
        }
    }
}
