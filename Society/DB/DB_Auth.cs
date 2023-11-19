using Society.Model;
using System;
using System.Data.SqlClient;
using System.Data;

public static partial class DB_Interaction
{
    // ------------------------------------------------ #Аутентификация ------------------------------------------------

    public static (bool success, string errorMessage) CheckLogin(string login, string password)
    {
        OpenConnection();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT Password FROM Employee_table WHERE Login = @Login";
            cmd.Parameters.AddWithValue("@Login", login);

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedPassword = reader["Password"].ToString();

                        if (storedPassword == password)
                        {
                            Console.WriteLine("Вход выполнен успешно");
                            return (true, null);
                        }

                        else
                        {
                            Console.WriteLine("Неверный пароль");
                            return (false, "Неверный пароль");
                        }
                    }

                    else
                    {
                        Console.WriteLine("Пользователь не найден");
                        return (false, "Пользователь не найден");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке входа: {ex.Message}");
                return (false, "Произошла ошибка");
            }
        }
    }

    public static (bool success, string errorMessage) LoginNoExists(string login)
    {
        OpenConnection();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "IF EXISTS (SELECT 1 FROM Employee_table WHERE Login = @Login) SELECT 1 ELSE SELECT 0";
            cmd.Parameters.AddWithValue("@Login", login);

            try
            {
                bool exists = (int)cmd.ExecuteScalar() == 1;
                if (exists)
                {
                    return (false, "Логин уже существует");
                }

                else
                {
                    return (true, null);
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке наличия логина: {ex.Message}");
                return (false, "Произошла ошибка при проверке логина");
            }
        }
    }

    public static (bool success, string errorMessage) AddEmployee(string name, string surname, string patronymic, string login, string password)
    {
        OpenConnection();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            var loginExistsResult = LoginNoExists(login);
            if (!loginExistsResult.success)
            {
                return loginExistsResult;
            }

            cmd.CommandText = "INSERT INTO Employee_table (Name, Surname, Patronymic, Login, Password, ID_Role) " +
                              "VALUES (@Name, @Surname, @Patronymic, @Login, @Password, 1)";
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Surname", surname);
            cmd.Parameters.AddWithValue("@Patronymic", patronymic);
            cmd.Parameters.AddWithValue("@Login", login);
            cmd.Parameters.AddWithValue("@Password", password);

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("Сотрудник успешно добавлен в базу данных");
                return (true, null);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении сотрудника: {ex.Message}");
                return (false, "Произошла ошибка при добавлении сотрудника");
            }
        }
    }

    public static (bool success, string errorMessage) GetFillEmployeeByLogin(string login)
    {
        OpenConnection();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM Employee_table WHERE Login = @Login";
            cmd.Parameters.AddWithValue("@Login", login);

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Заполняем объект Employee данными из базы данных
                        User.ID_Employee = Convert.ToInt32(reader["ID_Employee"]);
                        User.Name = reader["Name"].ToString();
                        User.Surname = reader["Surname"].ToString();
                        User.Patronymic = reader["Patronymic"].ToString();
                        User.Login = reader["Login"].ToString();
                        User.Password = reader["Password"].ToString();
                        User.ID_Role = Convert.ToInt32(reader["ID_Role"]);

                        return (true, null);
                    }

                    else
                    {
                        return (false, "Пользователь с указанным логином не найден");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных сотрудника: {ex.Message}");
                return (false, "Произошла ошибка. Проверьте соединение с БД");
            }
        }
    }

    public static bool LoadEmployeeData()
    {
        if (User.ID_Employee == -1)
        {
            return false;
        }

        OpenConnection();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM Employee_table WHERE ID_Employee = @ID_Employee";
            cmd.Parameters.AddWithValue("@ID_Employee", User.ID_Employee);

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Заполняем объект Employee данными из базы данных
                        User.Name = reader["Name"].ToString();
                        User.Surname = reader["Surname"].ToString();
                        User.Patronymic = reader["Patronymic"].ToString();
                        User.Login = reader["Login"].ToString();
                        User.Password = reader["Password"].ToString();
                        User.ID_Role = Convert.ToInt32(reader["ID_Role"]);

                        return true;
                    }

                    else
                    {
                        return false;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных сотрудника: {ex.Message}");
                return false;
            }
        }
    }
}
