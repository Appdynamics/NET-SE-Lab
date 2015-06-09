using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.Configuration;

namespace TrainingWCFService
{
    public partial class MainService : ServiceBase
    {
        ServiceHost serviceHost = null;

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(SearchService));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("TrainingLabWCFService", e.ExceptionObject.ToString(), EventLogEntryType.Error);
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
