using System;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelDemo
{
    class Program
    {
        public static void Main(string[] args)
        {
            Excel.Application excel;
            Excel.Workbook workbook;
            Excel.Worksheet sheet;
            Excel.Range range;

            try
            {
                // Start Excel and get Application object.
                excel = new Excel.Application();
                excel.Visible = true;

                // Get a new workbook.
                workbook = excel.Workbooks.Add(Missing.Value);
                sheet = (Excel.Worksheet)workbook.ActiveSheet;

                // Add table headers going cell by cell.
                sheet.Cells[1, 1] = "First Name";
                sheet.Cells[1, 2] = "Last Name";
                sheet.Cells[1, 3] = "Full Name";
                sheet.Cells[1, 4] = "Salary";

                // Format A1:D1 as bold, vertical alignment = center.
                sheet.get_Range("A1", "D1").Font.Bold = true;
                sheet.get_Range("A1", "D1").VerticalAlignment =
                Excel.XlVAlign.xlVAlignCenter;

                // Create an array to multiple values at once.
                string[,] saNames = new string[5, 2];

                saNames[0, 0] = "John";
                saNames[0, 1] = "Smith";
                saNames[1, 0] = "Tom";
                saNames[1, 1] = "Brown";
                saNames[2, 0] = "Sue";
                saNames[2, 1] = "Thomas";
                saNames[3, 0] = "Jane";
                saNames[3, 1] = "Jones";
                saNames[4, 0] = "Adam";
                saNames[4, 1] = "Johnson";

                // Fill A2:B6 with an array of values (First and Last Names).
                sheet.get_Range("A2", "B6").Value2 = saNames;

                // Fill C2:C6 with a relative formula (=A2 & " " & B2).
                range = sheet.get_Range("C2", "C6");
                range.Formula = "=A2 & \" \" & B2";

                // Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                range = sheet.get_Range("D2", "D6");
                range.Formula = "=RAND()*100000";
                range.NumberFormat = "$0.00";

                // AutoFit columns A:D.
                range = sheet.get_Range("A1", "D1");
                range.EntireColumn.AutoFit();

                // Make sure Excel is visible and give the user control
                // of Microsoft Excel's lifetime.
                excel.Visible = true;
                excel.UserControl = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} Line: {e.Source}");
            }
        }
    }
}
