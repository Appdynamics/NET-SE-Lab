using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace LabSite
{
    public partial class healthcheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize cache client
            string cacheHost = ConfigurationManager.AppSettings["cacheHost"];
            int cachePort = int.Parse(ConfigurationManager.AppSettings["cachePort"]);
            CacheLib.Client client = new CacheLib.Client(cachePort, cacheHost);

            client.Get("test");

            TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);
            invoker.HealthCheck();
        }
    }
}