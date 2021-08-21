using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuggyAmb.Pages.Problem
{
    public class HandledExceptionModel : PageModel
    {
        public void OnGet()
        {
            try
            {
                FileStream fileStream = new FileStream("j:\file.txt", FileMode.Create);
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string line = reader.ReadLine();
                }
            }
            catch (Exception exc)
            {
                ViewData["ExceptionInfo"] = exc.GetType().ToString();
            }
        }
    }
}
