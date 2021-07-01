using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BuggyAmb.Pages.Problem
{
    public class Crash2Model : PageModel
    {
        public string quote;
        ~Crash2Model()
        {
            if (quote.ToString() != string.Empty)
            {
                quote = null;
            }
        }
        public void OnGet()
        {
        }
    }
}
