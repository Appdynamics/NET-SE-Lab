using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace LabSite
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
 
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            CleanUpDb();
        }

        private void CleanUpDb()
        {
            string cs = ConfigurationManager.AppSettings["Datasource"];

            using (SqlConnection cn = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM SearchData", cn);
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (TextBoxSearch.Text != null && TextBoxSearch.Text.Length > 0)
            {
                string s = TextBoxSearch.Text.Trim();
                string[] values = s.Split(' ');

                if (values.Length > 0)
                {
                    TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);
                    invoker.LogOperation(values);

                    string operation = invoker.GetOperation(values);

                    // Initialize cache client
                    string cacheHost = ConfigurationManager.AppSettings["cacheHost"];
                    int cachePort = int.Parse(ConfigurationManager.AppSettings["cachePort"]);
                    CacheLib.Client client = new CacheLib.Client(cachePort, cacheHost);

                    client.Get(operation);

                    string result = new BusinessLogic.SearchProvider(ConfigurationManager.AppSettings["Datasource"]).GetValue(operation);
                    if (result != null)
                    {
                        Response.Write(result);
                    }
                    else
                    {
                        invoker.RegisterOperation(values);
                        result = new BusinessLogic.SearchProvider(ConfigurationManager.AppSettings["Datasource"]).GetValue(operation);
                        Response.Write(result);

                        client.Put(operation, result);
                    }
                }
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Write("<iframe src='searchstats' style='width:600px; height:450px;'></iframe>");
        }
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Write("<iframe src='logstats' style='width:600px; height:450px;'></iframe>");
        }
    }
}