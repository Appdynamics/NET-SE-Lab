using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net.Http;

namespace LabHealthCheckService
{
    public partial class LabService : ServiceBase
    {
        Timer health_timer = null;
        Timer db_timer = null;

        public LabService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (health_timer != null)
            {
                health_timer.Stop();
                health_timer = null;
            }

            health_timer = new Timer();
            health_timer.Elapsed += new ElapsedEventHandler(HealthCheckTimer_Elapsed);
            health_timer.Interval = int.Parse(ConfigurationManager.AppSettings["HealchCheckIntercal"]);
            health_timer.Enabled = true;
            health_timer.Start();

            if (db_timer != null)
            {
                db_timer.Stop();
                db_timer = null;
            }

            db_timer = new Timer();
            db_timer.Elapsed += new ElapsedEventHandler(DbTimer_Elapsed);
            db_timer.Interval = int.Parse(ConfigurationManager.AppSettings["DbCheckInterval"]);
            db_timer.Enabled = true;
            db_timer.Start();
        }

        protected override void OnStop()
        {
            if (health_timer != null)
            {
                health_timer.Stop();
                health_timer = null;
            }

            if (db_timer != null)
            {
                db_timer.Stop();
                db_timer = null;
            }
        }

        void HealthCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            HealthCheck().Wait();
        }

        async Task HealthCheck()
        {
            await HealthCheck1();
            await HealthCheck2();
            await HealthCheck3();
            await HealthCheck4();
        }

        async Task HealthCheck1()
        {
            await PerformHealthCheck(ConfigurationManager.AppSettings["LabMainSite"]);
        }

        async Task HealthCheck2()
        {
            await PerformHealthCheck(ConfigurationManager.AppSettings["LabApiSite"]);
        }

        async Task HealthCheck3()
        {
            await PerformHealthCheck(ConfigurationManager.AppSettings["LabAjaxService"]);
        } //MainSiteRobots

        async Task HealthCheck4()
        {
            await PerformHealthCheck(ConfigurationManager.AppSettings["MainSiteRobots"]);
        } 

        private async Task PerformHealthCheck(string URL)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    await client.GetStringAsync(URL);
                }
            }
            catch(WebException ex)
            {
                LogError(ex);
            }
        }

        private void LogError(Exception ex)
        {
            string message = DateTime.Now.ToString() + ex.ToString();
            System.Threading.Thread.Sleep(11 + DateTime.Now.Second);
        }

        void DbTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DbCleanUp3(null);

            DbCleanup().Wait();
        }

        async Task DbCleanup()
        {
            await Task.Factory.StartNew(() => { DbCleanUp1(null); })
                .ContinueWith((t) => { DbCleanUp2(null); })
                .ContinueWith((t) => { DbCleanUp2(null); });
        }

        private void DbCleanUp1(object state)
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["Datasource"];

                using (SqlConnection cn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from OperationLog WHERE Date < @Date", cn);
                    cmd.Parameters.Add(new SqlParameter("Date", DateTime.Now.Subtract(TimeSpan.FromHours(2))));

                    cn.Open();

                    cmd.ExecuteNonQuery();

                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void DbCleanUp2(object state)
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["Datasource"];

                using (SqlConnection cn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from SearchData WHERE Date < @Date", cn);
                    cmd.Parameters.Add(new SqlParameter("Date", DateTime.Now.Subtract(TimeSpan.FromDays(1))));

                    cn.Open();

                    cmd.ExecuteNonQuery();

                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void DbCleanUp3(object state)
        {
            try
            {
                string cs = ConfigurationManager.AppSettings["Datasource"];

                using (SqlConnection cn = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("DELETE from SearchData WHERE Number > Total", cn);

                    cn.Open();

                    cmd.ExecuteNonQuery();

                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
    }
}
