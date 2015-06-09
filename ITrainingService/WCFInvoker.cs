using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TrainingWCFService
{
    public class WCFInvoker : TrainingWCFService.ISearchService
    {
        private string url = null; // "net.tcp://localhost:9000/MyService2"

        public WCFInvoker(string URL)
        {
            url = URL;
        }

        private static IClientChannel GetClientChannel<T>(string serviceUri)
        {
            return (IClientChannel)CreateChannelFactory<T>(serviceUri).CreateChannel();
        }

        private static ChannelFactory<T> CreateChannelFactory<T>(string url)
        {
            TcpTransportBindingElement httpTransport = new TcpTransportBindingElement
            {
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue
            };

            TextMessageEncodingBindingElement messageEncoding = new TextMessageEncodingBindingElement
            {
                MessageVersion = MessageVersion.Soap11,
                WriteEncoding = System.Text.Encoding.UTF8,
                MaxReadPoolSize = int.MaxValue,
                MaxWritePoolSize = int.MaxValue
            };

            //-- Use this binding incase your service required CustomBinding configuration.  
            // CustomBinding binding = new CustomBinding(messageEncoding, httpTransport);

            //-- Else use WSHttpBinding configuration.  
            return new ChannelFactory<T>(new NetTcpBinding(), new EndpointAddress(url));
        }

        public void HealthCheck()
        {
            IClientChannel clientChannel = null;

            using (clientChannel = GetClientChannel<TrainingWCFService.ISearchService>(url))
            {
                (clientChannel as TrainingWCFService.ISearchService).HealthCheck();
            }
        }

        public void LogOperation(string[] operations)
        {
            IClientChannel clientChannel = null;

            using (clientChannel = GetClientChannel<TrainingWCFService.ISearchService>(url))
            {
                (clientChannel as TrainingWCFService.ISearchService).LogOperation(operations);
            }
        }

        public string GetOperation(string[] operations)
        {
            IClientChannel clientChannel = null;

            using (clientChannel = GetClientChannel<TrainingWCFService.ISearchService>(url))
            {
                return (clientChannel as TrainingWCFService.ISearchService).GetOperation(operations);
            }
        }

        public void RegisterOperation(string[] operations)
        {
            IClientChannel clientChannel = null;

            using (clientChannel = GetClientChannel<TrainingWCFService.ISearchService>(url))
            {
                (clientChannel as TrainingWCFService.ISearchService).RegisterOperation(operations);
            }
        }

        public bool CheckOperation(string[] operations)
        {
            IClientChannel clientChannel = null;

            using (clientChannel = GetClientChannel<TrainingWCFService.ISearchService>(url))
            {
                return (clientChannel as TrainingWCFService.ISearchService).CheckOperation(operations);
            }
        }
    }
}
