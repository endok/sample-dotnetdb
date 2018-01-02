using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
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
            const string connectionString = "Data Source=(local);Initial Catalog=testDB;User ID=sa;Password=P@ssw0rd";

            Console.WriteLine("----- SqlCommand -----");
            SqlCommandSample(connectionString);

            Console.WriteLine("----- DataSet -----");
            DataSetSample(connectionString);

            Console.WriteLine("----- EntityFramework -----");
            EntityFrameworkSample();

            Console.WriteLine("----- Dapper -----");
            DapperSample(connectionString);

            Console.ReadLine();
        }

        private static void SqlCommandSample(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // SqlCommandによる参照
                string selectquery = "SELECT id, name, age FROM member WHERE age >= @age ";

                SqlCommand selectcommand = new SqlCommand(selectquery, connection);
                selectcommand.Parameters.AddWithValue("@age", 25);

                SqlDataReader reader = selectcommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("id={0},name={1},age={2}", reader[0], reader[1], reader[2]);
                }
                reader.Close();

                // SqlCommandによる更新
                string updatequery = "UPDATE member SET age = 40 WHERE id = @id";

                SqlCommand updatecommand = new SqlCommand(updatequery, connection);
                updatecommand.Parameters.AddWithValue("@id", 3);

                updatecommand.ExecuteNonQuery();
            }
        }

        private static void DataSetSample(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // DataSetによる参照、更新
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
                carolRow["age"] = 50;

                adapter.Update(dataset, "memberTable");
            }

        }

        private static void EntityFrameworkSample()
        {
            // Entity SQLでの参照
            Console.WriteLine("-- Entity SQL");
            using (EntityConnection connection = new EntityConnection("name=SampleContext"))
            {
                connection.Open();

                string query1 = "SELECT m.id, m.name, m.age FROM SampleContext.member AS m WHERE m.age >= @age";

                using (EntityCommand cmd = new EntityCommand(query1, connection))
                {
                    cmd.Parameters.AddWithValue("age", 25);

                    using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        while (rdr.Read())
                        {
                            Console.WriteLine("id={0},name={1},age={2}", rdr["id"], rdr["name"], rdr["age"]);
                        }
                    }
                }

            }

            using (var db = new SampleContext())
            {
                // LINQ to Entitiesでの参照
                Console.WriteLine("-- LINQ to Entities");
                var query1 = from m in db.member
                             where m.age >= 25
                             select m;

                foreach (var member in query1)
                {
                    Console.WriteLine("id={0},name={1},age={2}", member.id, member.name, member.age);
                }

                // 別の書き方での参照
                Console.WriteLine("-- LINQ to Entities 2");
                var query2 = db.member.Where(m => m.age >= 25);

                foreach (var member in query2)
                {
                    Console.WriteLine("id={0},name={1},age={2}", member.id, member.name, member.age);
                }

                // Entity Framewokでの更新
                var carol = db.member.Find(3);
                carol.age = 60;
                db.SaveChanges();
            }
        }

        private static void DapperSample(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // Dapperによる参照
                string selectquery = "SELECT id, name, age FROM member WHERE age >= @age";
                var members = connection.Query<member>(selectquery, new { age = 25 });

                foreach (var member in members)
                {
                    Console.WriteLine("id={0},name={1},age={2}", member.id, member.name, member.age);
                }

                // Dapperによる更新
                string updatequery = "UPDATE member SET age = 30 WHERE id = @id";
                connection.Execute(updatequery, new { id = 3 });
            }
        }
    }
}
