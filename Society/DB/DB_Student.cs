using Society.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

public static partial class DB_Interaction
{
    // ------------------------------------------------ Ученик ------------------------------------------------

    public static List<Student> GetStudents()
    {
        OpenConnection();

        List<Student> students = new List<Student>();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT * FROM Student_table";

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            ID_Student = Convert.ToInt32(reader["ID_Student"]),
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Patronymic = reader["Patronymic"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
                        };

                        students.Add(student);
                    }

                    return students;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных студентов: {ex.Message}");
                return null;
            }
        }
    }

    public static List<Student> GetStudentsBySociety(int societyID)
    {
        OpenConnection();

        List<Student> students = new List<Student>();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            // Получаем ID студентов, участвующих в указанном кружке
            cmd.CommandText = "SELECT ID_Student FROM StudentSociety_table WHERE ID_Society = @SocietyID";
            cmd.Parameters.AddWithValue("@SocietyID", societyID);

            try
            {
                List<int> studentIds = new List<int>();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        studentIds.Add(Convert.ToInt32(reader["ID_Student"]));
                    }
                }

                // Получаем информацию о каждом студенте по его ID
                foreach (int studentId in studentIds)
                {
                    Student student = GetStudentById(studentId);
                    if (student != null)
                    {
                        students.Add(student);
                    }
                }

                return students;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных студентов по кружку: {ex.Message}");
                return null;
            }
        }
    }

    private static Student GetStudentById(int studentId)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            // Получаем информацию о студенте по его ID
            cmd.CommandText = "SELECT * FROM Student_table WHERE ID_Student = @StudentID";
            cmd.Parameters.AddWithValue("@StudentID", studentId);

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Student
                        {
                            ID_Student = Convert.ToInt32(reader["ID_Student"]),
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Patronymic = reader["Patronymic"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
                        };
                    }

                    else
                    {
                        Console.WriteLine($"Студент с ID {studentId} не найден.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных студента: {ex.Message}");
                return null;
            }
        }
    }

    public static (bool success, string errorMessage) AddStudent(string name, string surname, string patronymic, DateTime dateOfBirth)
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
                    cmd.CommandText = "INSERT INTO Student_table (Name, Surname, Patronymic, DateOfBirth) VALUES (@Name, @Surname, @Patronymic, @DateOfBirth)";
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Surname", surname);
                    cmd.Parameters.AddWithValue("@Patronymic", patronymic);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    return (true, null);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при добавлении студента: {ex.Message}");
                    return (false, "Произошла ошибка при добавлении студента");
                }
            }
        }
    }

    public static (bool success, string errorMessage) UpdateStudents(DataTable dataTable)
    {
        OpenConnection();

        try
        {
            // Удаляем студентов, которых нет в переданном DataTable
            DeleteNonExistingStudents(dataTable);

            // Обновляем существующих студентов
            foreach (DataRow row in dataTable.Rows)
            {
                int id = (int)row["ID"];
                string name = (string)row["Имя"];
                string surname = (string)row["Фамилия"];
                string patronymic = (string)row["Отчество"];
                DateTime dateOfBirth = (DateTime)row["Дата рождения"]; // Добавлено поле DateOfBirth

                if (StudentExists(id))
                {
                    UpdateStudent(id, name, surname, patronymic, dateOfBirth);
                }
            }

            // Добавляем новых студентов
            foreach (DataRow row in dataTable.Rows)
            {
                int id = (int)row["ID"];
                string name = (string)row["Имя"];
                string surname = (string)row["Фамилия"];
                string patronymic = (string)row["Отчество"];
                DateTime dateOfBirth = (DateTime)row["Дата рождения"]; // Добавлено поле DateOfBirth

                if (!StudentExists(id))
                {
                    AddStudent(name, surname, patronymic, dateOfBirth);
                }
            }

            return (true, null);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обновлении данных студентов: {ex.Message}");
            return (false, "Произошла ошибка при обновлении данных студентов");
        }
    }

    private static void UpdateStudent(int id, string name, string surname, string patronymic, DateTime dateOfBirth)
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
                    cmd.CommandText = "SELECT COUNT(*) FROM Student_table WHERE ID_Student = @ID";
                    cmd.Parameters.AddWithValue("@ID", id);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Если запись существует, обновляем ее данные
                        cmd.Parameters.Clear();
                        cmd.CommandText = "UPDATE Student_table SET Name = @Name, Surname = @Surname, Patronymic = @Patronymic, DateOfBirth = @DateOfBirth WHERE ID_Student = @ID";
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Surname", surname);
                        cmd.Parameters.AddWithValue("@Patronymic", patronymic);
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Запись с ID {id} не найдена при обновлении студента.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при обновлении данных студента: {ex.Message}");
                }
            }
        }
    }

    private static bool StudentExists(int id)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT COUNT(*) FROM Student_table WHERE ID_Student = @ID";
            cmd.Parameters.AddWithValue("@ID", id);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    private static void DeleteNonExistingStudents(DataTable dataTable)
    {
        List<int> existingStudentIds = GetStudentIds();

        // Определяем студентов, которых нет в переданном DataTable
        List<int> nonExistingStudentIds = existingStudentIds.Except(dataTable.AsEnumerable().Select(row => row.Field<int>("ID"))).ToList();

        foreach (int id in nonExistingStudentIds)
        {
            // Удаляем студента из базы данных
            DeleteStudent(id);
        }
    }

    private static List<int> GetStudentIds()
    {
        List<int> studentIds = new List<int>();

        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT ID_Student FROM Student_table";

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    studentIds.Add(Convert.ToInt32(reader["ID_Student"]));
                }
            }
        }

        return studentIds;
    }

    private static void DeleteStudent(int id)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "DELETE FROM Student_table WHERE ID_Student = @ID";
            cmd.Parameters.AddWithValue("@ID", id);

            cmd.ExecuteNonQuery();
        }
    }

    public static (bool success, string errorMessage) DeleteStudentSocietyLink(int studentId, int societyId)
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
                    // Получаем ID_StudentSociety по ID_Student и ID_Society
                    cmd.CommandText = "SELECT ID_StudentSociety FROM StudentSociety_table WHERE ID_Student = @StudentID AND ID_Society = @SocietyID";
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);

                    int studentSocietyLinkId = Convert.ToInt32(cmd.ExecuteScalar());

                    if (studentSocietyLinkId > 0)
                    {
                        // Удаляем связь по полученному ID_StudentSociety
                        cmd.Parameters.Clear();
                        cmd.CommandText = "DELETE FROM StudentSociety_table WHERE ID_StudentSociety = @StudentSocietyLinkID";
                        cmd.Parameters.AddWithValue("@StudentSocietyLinkID", studentSocietyLinkId);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        return (true, null);
                    }

                    else
                    {
                        transaction.Rollback();
                        return (false, "Связь между учеником и кружком не найдена.");
                    }
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при удалении связи между учеником и кружком: {ex.Message}");
                    return (false, "Произошла ошибка при удалении связи между учеником и кружком");
                }
            }
        }
    }

    public static (bool success, string errorMessage) AddStudentSocietyLink(int studentId, int societyId)
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
                    // Проверяем, существует ли ученик с указанным ID
                    if (!StudentExists(studentId))
                    {
                        transaction.Rollback();
                        return (false, "Ученик с указанным ID не найден.");
                    }

                    // Проверяем, существует ли кружок с указанным ID
                    if (!SocietyExists(societyId))
                    {
                        transaction.Rollback();
                        return (false, "Кружок с указанным ID не найден.");
                    }

                    // Добавляем связь между учеником и кружком
                    cmd.CommandText = "INSERT INTO StudentSociety_table (ID_Student, ID_Society) VALUES (@StudentID, @SocietyID)";
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SocietyID", societyId);

                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    return (true, null);
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при добавлении связи между учеником и кружком: {ex.Message}");
                    return (false, "Произошла ошибка при добавлении связи между учеником и кружком");
                }
            }
        }
    }

    private static bool SocietyExists(int societyId)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            cmd.Connection = _connection;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT COUNT(*) FROM Society_table WHERE ID_Society = @ID";
            cmd.Parameters.AddWithValue("@ID", societyId);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }

}
