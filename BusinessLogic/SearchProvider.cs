using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BusinessLogic
{
    public class SearchProvider
    {
        public SearchProvider(string datasource)
        {
            this.datasource = datasource;
        }

        private string datasource;

        public string GetValue(string operation)
        {
            string result = null;

            using (SqlConnection cn = new SqlConnection(datasource))
            {
                SqlCommand cmd = new SqlCommand("SELECT Number, Fast, Slow, Error, Data from SearchData where Operation = @Operation", cn);
                cmd.Parameters.Add(new SqlParameter("Operation", operation));

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        result = reader.GetString(4);

                        int number = reader.GetInt32(0);
                        int fast = reader.GetInt32(1);
                        int slow = reader.GetInt32(1);
                        int error = reader.GetInt32(1);

                        Updatecount(operation);

                        if (number == error)
                            GenerateError();

                        int i = number % (fast + slow);
                        if (i > fast)
                        {
                            SlowDown();
                        }
                    }
                }

                cn.Close();
            }

            return result;
        }

        private void Updatecount(string operation)
        {
            using (SqlConnection cn = new SqlConnection(datasource))
            {
                SqlCommand cmd = new SqlCommand("UPDATE SearchData SET Number += 1 WHERE Operation = @Operation", cn);
                cmd.Parameters.Add(new SqlParameter("Operation", operation));

                cn.Open();

                cmd.ExecuteNonQuery();

                cn.Close();
            }
        }

        private void GenerateError()
        {
            int i = new Random().Next(3);
            switch (i)
            {
                case 0:
                    throw new ArgumentOutOfRangeException();
                    break;

                case 1:
                    int total = DateTime.Now.Second / (i * 10 - 10);
                    string result = total.ToString();
                    break;

                case 2:
                    using (SqlConnection cn = new SqlConnection(datasource))
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE SearchData SET Number += 1 WHERE Operation2 = @Operation", cn);
                        cmd.Parameters.Add(new SqlParameter("Operation", "value"));

                        cn.Open();

                        cmd.ExecuteNonQuery();

                        cn.Close();
                    }
                    break;
            }
        }

        private void SlowDown()
        {
            int i = new Random().Next(2);
            switch (i)
            {
                case 0:
                    System.Threading.Thread.Sleep(150 + new Random().Next(500));
                    break;

                case 1:
                    using (SqlConnection cn = new SqlConnection(datasource))
                    {
                        int delay = 250 + new Random().Next(749);
                        SqlCommand cmd = new SqlCommand("WAITFOR DELAY '00:00." + delay + "'", cn);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                    break;

                case 2:
                    using (SqlConnection cn = new SqlConnection(datasource))
                    {
                        int seconds = new Random().Next(3);
                        int delay = 150 + new Random().Next(749);
                        SqlCommand cmd = new SqlCommand("WAITFOR DELAY '00:0" + seconds + "." + delay + "'", cn);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                    break;
            }
        }
    }
}
