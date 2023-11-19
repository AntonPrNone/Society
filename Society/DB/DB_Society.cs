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

}
