using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GGStream.Controllers
{
    public class AdminController : Controller
    {
        [Route("/adminhome")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
