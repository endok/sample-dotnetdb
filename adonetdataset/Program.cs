using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adonetdataset
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=(local);Initial Catalog=testDB;User ID=sa;Password=P@ssw0rd";

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                WriteRows(connection);
            }

            Console.ReadLine();
        }

        private static void WriteRows(SqlConnection connection)
        {
            string query = "SELECT id, name, age FROM member WHERE age >= @age ";
            int paramAge = 25;

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@age", paramAge);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("id={0},name={1},age={2}", reader[0], reader[1], reader[2]);
            }
            reader.Close();

        }
    }
}
