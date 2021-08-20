using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuggyAmb.Pages.Problem
{
    public class SlowModel : PageModel
    {
        public void OnGet()
        {
            DataLayer dataLayer = new DataLayer();
            DataTable dt = dataLayer.GetAllProducts();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string ProductsTable = "<table><tr><td><B>Product ID</B></td><td><B>Product Name</B></td><td><B>Description</B></td></tr>";

            foreach (DataRow dr in dt.Rows)
            {
                ProductsTable += "<tr><td>" + dr[0] + "</td><td>" + dr[1] + "</td><td>" + dr[2] + "</td></tr>";
            }
            ProductsTable += "</table>";

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            ViewData["ProductsTable"] = ProductsTable;
            ViewData["ElapsedTime"] = ts.ToString();
        }
    }
}
