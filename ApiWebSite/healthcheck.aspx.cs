using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace ApiSite
{
    public partial class healthcheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);
            invoker.HealthCheck();
        }
    }
}