using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BuggyAmb
{
    public class DataLayer
    {
        private object syncobj = new Object();
        public DataLayer()
        {

        }
        public DataTable GetAllProducts()
        {
            DataTable dt = new DataTable();
            DataRow dr;
            DataColumn dc;

            dc = new DataColumn("ID", typeof(Int32));
            dc.Unique = true;
            dt.Columns.Add(dc);

            dt.Columns.Add(new DataColumn("ProductName", typeof(string)));
            dt.Columns.Add(new DataColumn("Description", typeof(string)));
            dt.Columns.Add(new DataColumn("Price", typeof(string)));

            for (int i = 0; i < 10000; i++)
            {
                dr = dt.NewRow();
                dr["ID"] = i;
                dr["ProductName"] = "Product " + i;
                dr["Description"] = "Description for Product " + i;
                dr["Price"] = "$100";
                dt.Rows.Add(dr);
            }
            return dt;
        }


    }

}
