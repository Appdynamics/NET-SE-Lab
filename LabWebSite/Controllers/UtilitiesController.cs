using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;

namespace LabSite.Controllers
{
    public class UtilitiesController : Controller
    {
        // GET: Utilities
        public async Task<ActionResult> SearchStats()
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.AppSettings["Datasource"]))
            {
                await c.OpenAsync();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SearchData", c);
                var count = cmd.ExecuteScalarAsync();
                await count;

                using (SqlDataAdapter a = new SqlDataAdapter("SELECT Operation,Fast,Slow,Error,Number,Total,Date FROM SearchData", c))
                {

                    DataTable t = new DataTable();
                    a.Fill(t);

                    ViewData["data"] = ConvertDataTable(t);
                }
            }

            await Task.Delay(100);

            return View();
        }

        public async Task<ActionResult> LogStats()
        {
            using (SqlConnection c = new SqlConnection(ConfigurationManager.AppSettings["Datasource"]))
            {
                await c.OpenAsync();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM OperationLog", c);
                var count = cmd.ExecuteScalarAsync();
                await count;

                using (SqlDataAdapter a = new SqlDataAdapter("SELECT * FROM OperationLog", c))
                {

                    DataTable t = new DataTable();
                    a.Fill(t);

                    ViewData["data"] = ConvertDataTable(t);
                }
            }

            return View();
        }

        private IEnumerable<object> ConvertDataTable(DataTable table)
        {
            var result = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (DataColumn col in table.Columns)
                {
                    obj.Add(col.ColumnName, row[col.ColumnName]);
                }
                result.Add(obj);
            }

            return result;
        }
     }
}