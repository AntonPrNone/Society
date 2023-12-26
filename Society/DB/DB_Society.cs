using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public static partial class DB_Interaction
{
    public static List<SocietyClass> GetSocieties()
    {
        List<SocietyClass> societies = new List<SocietyClass>();

        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Society_table";

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SocietyClass society = new SocietyClass
                        {
                            ID_Society = Convert.ToInt32(reader["ID_Society"]),
                            Name = reader["Name"].ToString(),
                            MaxStudent = Convert.ToInt32(reader["MaxStudent"]),
                            NumberHour = Convert.ToInt32(reader["NumberHour"])
                        };

                        societies.Add(society);
                    }
                }
            }

            return societies;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении данных о кружках: {ex.Message}");
            return null;
        }
    }

    public static int AddSociety(string name, int maxStudent, int numberHour)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Society_table (Name, MaxStudent, NumberHour) " +
                                  "VALUES (@Name, @MaxStudent, @NumberHour); " +
                                  "SELECT SCOPE_IDENTITY();";

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@MaxStudent", maxStudent);
                cmd.Parameters.AddWithValue("@NumberHour", numberHour);

                // Выполняем команду и возвращаем ID добавленного кружка
                int newSocietyID = Convert.ToInt32(cmd.ExecuteScalar());

                return newSocietyID;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении кружка: {ex.Message}");
            return -1; // Возвращаем -1 в случае ошибки
        }
    }

    public static bool UpdateSociety(int id, string name, int maxStudent, int numberHour)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Society_table SET Name = @Name, MaxStudent = @MaxStudent, NumberHour = @NumberHour WHERE ID_Society = @ID";

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@MaxStudent", maxStudent);
                cmd.Parameters.AddWithValue("@NumberHour", numberHour);

                // Выполняем команду
                int rowsAffected = cmd.ExecuteNonQuery();

                // Возвращаем true, если хотя бы одна строка была изменена
                return rowsAffected > 0;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении данных о кружке: {ex.Message}");
            return false;
        }
    }

    public static bool DeleteSociety(int id)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Society_table WHERE ID_Society = @ID";

                cmd.Parameters.AddWithValue("@ID", id);

                // Выполняем команду
                int rowsAffected = cmd.ExecuteNonQuery();

                // Возвращаем true, если хотя бы одна строка была удалена
                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении данных о кружке: {ex.Message}");
            return false;
        }
    }


    public static SocietyClass GetSocietyById(int societyId)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Society_table WHERE ID_Society = @ID";

                cmd.Parameters.AddWithValue("@ID", societyId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        SocietyClass society = new SocietyClass
                        {
                            ID_Society = Convert.ToInt32(reader["ID_Society"]),
                            Name = reader["Name"].ToString(),
                            MaxStudent = Convert.ToInt32(reader["MaxStudent"]),
                            NumberHour = Convert.ToInt32(reader["NumberHour"])
                        };

                        return society;
                    }
                }
            }

            return null; // Возвращаем null, если кружок с указанным ID не найден
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении данных о кружке: {ex.Message}");
            return null;
        }
    }
    public static List<SocietyClass> GetSocietiesByEmployeeId(int employeeId)
    {
        List<SocietyClass> societies = new List<SocietyClass>();

        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Society_table.* FROM Society_table " +
                                  "JOIN SocietyEmployee_table ON Society_table.ID_Society = SocietyEmployee_table.ID_Society " +
                                  "WHERE SocietyEmployee_table.ID_Employee = @EmployeeID";

                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SocietyClass society = new SocietyClass
                        {
                            ID_Society = Convert.ToInt32(reader["ID_Society"]),
                            Name = reader["Name"].ToString(),
                            MaxStudent = Convert.ToInt32(reader["MaxStudent"]),
                            NumberHour = Convert.ToInt32(reader["NumberHour"])
                        };

                        societies.Add(society);
                    }
                }
            }

            return societies;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении данных о кружках для сотрудника: {ex.Message}");
            return null;
        }
    }
}
