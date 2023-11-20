using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public static partial class DB_Interaction
{
    public static List<Lesson> GetLessonsBySociety(int societyID)
    {
        List<Lesson> lessons = new List<Lesson>();

        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Lesson_table WHERE ID_Society = @SocietyID";

                cmd.Parameters.AddWithValue("@SocietyID", societyID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Lesson lesson = new Lesson
                        {
                            ID_Lesson = Convert.ToInt32(reader["ID_Lesson"]),
                            CabinetNumber = Convert.ToInt32(reader["CabinetNumber"]),
                            Date = (reader["Date"]).ToString(),
                            StartTime = reader["StartTime"].ToString(), // Преобразование TimeSpan к строке
                            EndTime = reader["EndTime"].ToString(),     // Преобразование TimeSpan к строке
                            ID_Employee = Convert.ToInt32(reader["ID_Employee"]),
                            ID_Society = Convert.ToInt32(reader["ID_Society"])
                        };

                        lessons.Add(lesson);
                    }
                }
            }

            return lessons;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении уроков для кружка: {ex.Message}");
            return null;
        }
    }
}
