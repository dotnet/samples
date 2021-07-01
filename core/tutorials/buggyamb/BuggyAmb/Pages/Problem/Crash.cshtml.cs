using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuggyAmb.Pages.Problem
{
    public class CrashModel : PageModel
    {
        public void OnGet()
        {
            string filePath = @"c:\list.txt";
            string allContent = ReadFromFile(filePath);
            ViewData["fileContent"] = allContent;
        }

        public string ReadFromFile(string filePath)
        {
            string allContent = string.Empty;
            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    allContent = reader.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                WriteExceptionToLog(exc.ToString());
            }

            return allContent;
        }

        private void WriteExceptionToLog(string error)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(@"j:\error.log"))
                {
                    sw.WriteLine(DateTime.Now + "..." + error);
                }
            }
            catch (Exception exc)
            {
                WriteExceptionToLog(exc.ToString());
            }
        }

    }
}
