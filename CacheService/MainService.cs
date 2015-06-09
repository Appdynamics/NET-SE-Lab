using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CacheLib;
using System.Configuration;

namespace CacheService
{
    public partial class MainService : ServiceBase
    {
        public MainService()
        {
            InitializeComponent();
        }

        Server server = null;

        protected override void OnStart(string[] args)
        {
            int port = int.Parse(ConfigurationManager.AppSettings["serverPort"]);

            server = new Server(port);
            server.Start();
        }

        protected override void OnStop()
        {
            server.Stop();
        }

        
    }
}
