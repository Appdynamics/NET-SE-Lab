using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Net;
using TrainingWCFService.HealthServiceReference;
using System.IO;

namespace TrainingWCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SearchService" in both code and config file together.
    public class SearchService : ISearchService
    {
        public void HealthCheck()
        {
            HealthService ws = new HealthService();
            ws.Url = ConfigurationManager.AppSettings["HealthCheckWebService"];
            ws.CheckHealthCompleted += ws_CheckHealthCompleted;
            ws.CheckHealthAsync();
        }

        void ws_CheckHealthCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Test the DB connectivity and basic query
            string cs = ConfigurationManager.AppSettings["Datasource"];
            using (SqlConnection cn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SearchData", cn);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        public string GetOperation(string[] operations)
        {
            string operation = string.Join("+", operations);
            return operation;
        }

        public void LogOperation(string[] operations)
        {
            string operation = GetOperation(operations);

            string cs = ConfigurationManager.AppSettings["Datasource"];

            using (SqlConnection cn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO OperationLog (Date, Query) Values ( @Date, @Query)", cn);
                cmd.Parameters.Add(new SqlParameter("Date", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("Query", operation));

                cn.Open();

                cmd.ExecuteNonQuery();

                cn.Close();
            }
        }

        public bool CheckOperation(string[] operations)
        {
            string operation = GetOperation(operations);

            string cs = ConfigurationManager.AppSettings["Datasource"];
            int i = 0;

            using (SqlConnection cn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SearchData WHERE Operation = @Operation", cn);
                cmd.Parameters.Add(new SqlParameter("Operation", operation));

                cn.Open();

                i = Convert.ToInt32(cmd.ExecuteScalar());

                cn.Close();
            }
            return i > 0;
        }

        public void RegisterOperation(string[] operations)
        {
            string operation = GetOperation(operations);

            string cs = ConfigurationManager.AppSettings["Datasource"];

            using (SqlConnection cn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO SearchData (Operation, Fast, Slow, Error, Number, Total, Date, Data) Values (@Operation, @Fast, @Slow, @Error, @Number, @Total, @Date, @Data)", cn);

                cmd.Parameters.Add(new SqlParameter("Operation", operation));

                Random rnd = new Random();
                int total = 10 + rnd.Next(1000);

                cmd.Parameters.Add(new SqlParameter("Fast", 50 + rnd.Next(total / 3)));
                cmd.Parameters.Add(new SqlParameter("Slow", 10 + rnd.Next(total / 10)));
                cmd.Parameters.Add(new SqlParameter("Error", 40 + rnd.Next(total / 2)));

                int number = 0;
                cmd.Parameters.Add(new SqlParameter("Number", number));
                cmd.Parameters.Add(new SqlParameter("Total", total));
                cmd.Parameters.Add(new SqlParameter("Date", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("Data", DownloadData(operation)));

                cn.Open();

                cmd.ExecuteNonQuery();

                cn.Close();
            }
        }

        private string DownloadData(string operation)
        {
            string url = ConfigurationManager.AppSettings["SearchUrl"] + operation;
            string result = null;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            var sync = request.BeginGetResponse(null, null);
            var response = request.EndGetResponse(sync);

            Stream stream = response.GetResponseStream();
            using (StreamReader sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            response.Close();

            return result;
        }
    }
}
