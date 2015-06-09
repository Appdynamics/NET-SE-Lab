using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TrainingWCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISearchService" in both code and config file together.
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        void HealthCheck();

        [OperationContract]
        string GetOperation(string[] operations);

        [OperationContract]
        void LogOperation(string[] operations);

        [OperationContract]
        void RegisterOperation(string[] operations);

        [OperationContract]
        bool CheckOperation(string[] operations);
    }
}
