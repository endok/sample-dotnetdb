using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adonet
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

                Console.WriteLine("----- SqlCommand -----");

                // SqlCommandによる参照
                SelectRowsWithSqlCommand(connection);
                // SqlCommandによる更新
                UpdateWithSqlCommand(connection);

                Console.WriteLine("----- DataSet -----");

                // DataSetによる参照、更新
                SelectAndUpdateRowsWithDataSet(connection);

            }

            Console.ReadLine();
        }

        private static void SelectRowsWithSqlCommand(SqlConnection connection)
        {
            string query = "SELECT id, name, age FROM member WHERE age >= @age ";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@age", 25);

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("id={0},name={1},age={2}", reader[0], reader[1], reader[2]);
            }
            reader.Close();

        }

        private static void UpdateWithSqlCommand(SqlConnection connection)
        {
            string query = "UPDATE member SET age = 40 WHERE id = @id";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", 3);

            command.ExecuteNonQuery();
        }

        private static void SelectAndUpdateRowsWithDataSet(SqlConnection connection)
        {
            string selectQuery = "SELECT id, name, age FROM member";
            string updateQuery = "UPDATE member SET age = @age WHERE id = @id";

            SqlDataAdapter adapter = new SqlDataAdapter();

            // 主キー情報をDBから取得
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            // クエリの設定
            adapter.SelectCommand = new SqlCommand(selectQuery, connection);
            adapter.UpdateCommand = new SqlCommand(updateQuery, connection);

            // Updateパラメータの設定
            SqlParameter ageParam = new SqlParameter();
            ageParam.ParameterName = "@age";
            ageParam.SourceColumn = "age";
            adapter.UpdateCommand.Parameters.Add(ageParam);
            SqlParameter idParam = new SqlParameter();
            idParam.ParameterName = "@id";
            idParam.SourceColumn = "id";
            adapter.UpdateCommand.Parameters.Add(idParam);

            DataSet dataset = new DataSet();
            adapter.Fill(dataset, "memberTable");
            DataTable datatable = dataset.Tables["memberTable"];

            // データの参照
            foreach (DataRow row in datatable.Rows)
            {
                Console.WriteLine("id={0},name={1},age={2}", row["id"], row["name"], row["age"]);
            }

            // データの更新
            DataRow carolRow = datatable.Rows.Find(3);
            carolRow["age"] = 30;

            adapter.Update(dataset, "memberTable");
        }
    }
}
