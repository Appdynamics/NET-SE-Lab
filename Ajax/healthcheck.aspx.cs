using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Threading.Tasks;

namespace AjaxService
{
    public partial class healthcheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var task = Task.Factory.StartNew(() =>
                {
                    TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);
                    invoker.HealthCheck();
                });

            task.Wait();
        }
    }
}