using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;

namespace ApiSite.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Search(string value)
        {
            string response = "";

            if (!string.IsNullOrEmpty(value))
            {
                string[] values = value.Split(new char[] { '/' });

                TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);
                invoker.LogOperation(values);

                string operation = invoker.GetOperation(values);
                string result = new BusinessLogic.SearchProvider(ConfigurationManager.AppSettings["Datasource"]).GetValue(operation);
                if (result != null)
                {
                    response = result;
                }
                else
                {
                    invoker.RegisterOperation(values);
                    result = new BusinessLogic.SearchProvider(ConfigurationManager.AppSettings["Datasource"]).GetValue(operation);
                    response = result;
                }
            }
            else
            {
                Thread.Sleep(999);
                response = "Can't call this page directly";

            }

            return Content(response);
        }
    }
}