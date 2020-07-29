using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GGStream.Models;
using GGStream.Data;
using Microsoft.EntityFrameworkCore;

namespace GGStream.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Context _context;

        public HomeController(Context context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Route("/")]
        public IActionResult Index()
        {
            // Public collections
            List<Collection> collections = _context.Collection.Where(c => c.Private != true).OrderBy(s => s.URL).ToList();
            List<string> collectionURLs = collections.Select(c => c.URL).ToList();

            // Public streams
            List<Stream> streams = _context.Stream.Where(s => (s.StartDate == null || s.StartDate < DateTime.Now.AddDays(30)) && 
                (s.EndDate == null || s.EndDate > DateTime.Now) && 
                collectionURLs.Contains(s.CollectionURL) &&
                s.Private != true)
            .OrderBy(s => s.StartDate).ToList().ConvertAll(s => {
                s.Collection = collections.First(c => c.URL == s.CollectionURL);
                return s;
            });

            HomeViewModel model = new HomeViewModel
            {
                PublicCollections = collections,
                CurrentPublicStreams = streams
            };

            return View(model);
        }

        [Route("/Error")]
        public IActionResult HandleError([FromQuery]int code, [FromQuery]string message)
        {
            var genericMessages = new string[]{ "Something went wrong!",
                                                "Egads! Something broke!",
                                                "Zoinks! I think we broke it!" };

            string errorMsg = null;
            if (code == 401 || code == 403)
            {
                errorMsg = "I can't let you do that, Dave.";
            }
            else if (code == 404)
            {
                errorMsg = "This is not the resource you're looking for.";
            }
            else if (code == 500 || code == 502 || code == 503)
            {
                errorMsg = "It's our fault! Sorry!";
            }

            if (message != null)
            {
                errorMsg = message;
            }

            if (errorMsg == null)
            {
                Random random = new Random();
                int messageIdx = random.Next(0, genericMessages.Length);
                errorMsg = $"{genericMessages[messageIdx]} Error code: {code}";
            }

            ViewData["ErrorMessage"] = errorMsg;
            return View("~/Views/Shared/Error.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
