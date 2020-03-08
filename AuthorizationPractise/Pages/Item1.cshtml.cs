using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthorizationPractise
{
    [Authorize]
    public class Item1Model : PageModel
    {
        public void OnGet()
        {

        }
    }
}