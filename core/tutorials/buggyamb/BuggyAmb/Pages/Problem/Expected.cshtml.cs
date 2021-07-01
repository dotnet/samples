using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuggyAmb.Pages.Problem
{
    public class ExpectedModel : PageModel
    {
        public void OnGet()
        {
            string envVar = Environment.GetEnvironmentVariable("COMPlus_DbgEnableMiniDump");

            DataLayer dataLayer = new DataLayer();
            DataTable dt = dataLayer.GetAllProducts();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string ProductsTable = "<table><tr><td><B>Product ID</B></td><td><B>Product Name</B></td><td><B>Description</B></td></tr>";

            StringBuilder sb = new StringBuilder(ProductsTable);

            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine("<tr><td>" + dr[0] + "</td><td>" + dr[1] + "</td><td>" + dr[2] + "</td></tr>");
            }
            sb.AppendLine("</table>");

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            ViewData["ProductsTable"] = sb.ToString();
            ViewData["ElapsedTime"] = ts.ToString();
        }
    }
}
