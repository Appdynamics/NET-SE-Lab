using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace AjaxService
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AjaxLabService
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        [WebGet]
        public string CheckData(string value)
        {
            string[] values = value.Trim().Split(' ');

            var task1 = LogOperation(values);
            var task2 = PerformOperation(values);

            Task.WaitAll(task1, task2);

            return task2.Result;
        }

        private async Task LogOperation(string[] values)
        {
            TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);

            await Task.Factory.StartNew(() => { invoker.LogOperation(values); });
        }

        private async Task<string> PerformOperation(string[] values)
        {
            TrainingWCFService.WCFInvoker invoker = new TrainingWCFService.WCFInvoker(ConfigurationManager.AppSettings["WCFServiceAddress"]);

            string operation = await Task.Factory.StartNew<string>(() => { return invoker.GetOperation(values); });

            bool result = await Task.Factory.StartNew<bool>(() => { return invoker.CheckOperation(values); });  

            return string.Format("'{0}' - {1}", operation, result);
        }
    }
}
