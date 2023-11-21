using System;
using System.Data;
using System.Data.SqlClient;

public static partial class DB_Interaction
{
    private static SqlConnection _connection;
    private const string _connectionString = "Data Source=DESKTOP-KA59S37;Initial Catalog=Society;Integrated Security=True";
    private const string _connectionStringKtits = "Data Source=K1210-05;Initial Catalog=Society;Integrated Security=True";

    public static void OpenConnection()
    {
        if (_connection == null)
        {
            _connection = new SqlConnection(_connectionString);
        }

        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
            Console.WriteLine("Подключение открыто.");
        }
    }

    public static void CloseConnection()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
            Console.WriteLine("Подключение закрыто.");
        }
    }

    public static void Dispose()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            Console.WriteLine("Ресурсы освобождены.");
        }
    }
}
