using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;

namespace HealthCheckWebSite
{
    [ServiceContract]
    public class HealthService
    {
        [OperationContract]
        public async Task CheckHealth()
        {
            string connectionString = ConfigurationManager.AppSettings["Datasource"];

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string text = "SELECT COUNT(*) from OperationLog";
                if (new Random().Next(25) == 0)
                {
                    text = text + " WHERE NewField = " + DateTime.Now.Second;
                }

                SqlCommand sqlCommand = new SqlCommand(text, sqlConnection);

                await sqlConnection.OpenAsync();

                var res = sqlCommand.ExecuteNonQueryAsync();
                await res;

                sqlConnection.Close();
            }
        }
    }
}
