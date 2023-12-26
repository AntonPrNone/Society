using Society.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

                // Создаем DataTable для хранения результатов запроса
                DataTable lessonTable = new DataTable();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Заполняем DataTable
                    lessonTable.Load(reader);
                }

                // Обрабатываем данные из DataTable
                foreach (DataRow row in lessonTable.Rows)
                {
                    Lesson lesson = new Lesson
                    {
                        ID_Lesson = Convert.ToInt32(row["ID_Lesson"]),
                        CabinetNumber = Convert.ToInt32(row["CabinetNumber"]),
                        Date = row["Date"].ToString(),
                        StartTime = row["StartTime"].ToString(),
                        EndTime = row["EndTime"].ToString(),
                        ID_Employee = Convert.ToInt32(row["ID_Employee"]),
                        ID_Society = Convert.ToInt32(row["ID_Society"])
                    };

                    lesson.Teacher = GetEmployeeById(lesson.ID_Employee);

                    lessons.Add(lesson);
                }

                // Сортировка списка по дате и времени начала в порядке возрастания
                lessons = lessons.OrderBy(l => DateTime.Parse(l.Date + " " + l.StartTime)).ToList();
            }

            return lessons;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении уроков для кружка: {ex.Message}");
            return null;
        }
    }



    public static Lesson GetLessonById(int lessonId)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Lesson_table WHERE ID_Lesson = @LessonID";

                cmd.Parameters.AddWithValue("@LessonID", lessonId);

                // Создаем DataTable для хранения результатов запроса
                DataTable lessonTable = new DataTable();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Заполняем DataTable
                    lessonTable.Load(reader);
                }

                // Проверяем, найден ли урок с указанным ID
                if (lessonTable.Rows.Count > 0)
                {
                    DataRow row = lessonTable.Rows[0];
                    Lesson lesson = new Lesson
                    {
                        ID_Lesson = Convert.ToInt32(row["ID_Lesson"]),
                        CabinetNumber = Convert.ToInt32(row["CabinetNumber"]),
                        Date = row["Date"].ToString(),
                        StartTime = row["StartTime"].ToString(),
                        EndTime = row["EndTime"].ToString(),
                        ID_Employee = Convert.ToInt32(row["ID_Employee"]),
                        ID_Society = Convert.ToInt32(row["ID_Society"])
                    };

                    lesson.Teacher = GetEmployeeById(lesson.ID_Employee);

                    return lesson;
                }

                else
                {
                    Console.WriteLine($"Урок с ID {lessonId} не найден.");
                    return null;
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении урока по ID: {ex.Message}");
            return null;
        }
    }

    public static bool UpdateLesson(Lesson updatedLesson)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Lesson_table SET CabinetNumber = @CabinetNumber, Date = @Date, " +
                                  "StartTime = @StartTime, EndTime = @EndTime, ID_Employee = @ID_Employee, " +
                                  "ID_Society = @ID_Society WHERE ID_Lesson = @ID_Lesson";

                cmd.Parameters.AddWithValue("@CabinetNumber", updatedLesson.CabinetNumber);
                cmd.Parameters.AddWithValue("@Date", updatedLesson.Date);
                cmd.Parameters.AddWithValue("@StartTime", updatedLesson.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", updatedLesson.EndTime);
                cmd.Parameters.AddWithValue("@ID_Employee", updatedLesson.ID_Employee);
                cmd.Parameters.AddWithValue("@ID_Society", updatedLesson.ID_Society);
                cmd.Parameters.AddWithValue("@ID_Lesson", updatedLesson.ID_Lesson);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении урока: {ex.Message}");
            return false;
        }
    }

    public static bool SaveLesson(Lesson newLesson)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Lesson_table (CabinetNumber, Date, StartTime, EndTime, ID_Employee, ID_Society) " +
                                  "VALUES (@CabinetNumber, @Date, @StartTime, @EndTime, @ID_Employee, @ID_Society)";

                cmd.Parameters.AddWithValue("@CabinetNumber", newLesson.CabinetNumber);
                cmd.Parameters.AddWithValue("@Date", newLesson.Date);
                cmd.Parameters.AddWithValue("@StartTime", newLesson.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", newLesson.EndTime);
                cmd.Parameters.AddWithValue("@ID_Employee", newLesson.ID_Employee);
                cmd.Parameters.AddWithValue("@ID_Society", newLesson.ID_Society);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении нового урока: {ex.Message}");
            return false;
        }
    }

    public static bool DeleteLesson(int lessonId)
    {
        try
        {
            OpenConnection();

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = _connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Lesson_table WHERE ID_Lesson = @ID_Lesson";

                cmd.Parameters.AddWithValue("@ID_Lesson", lessonId);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении урока: {ex.Message}");
            return false;
        }
    }
}
