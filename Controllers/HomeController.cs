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
        private readonly IApplicationDateTime _dateTime;

        public HomeController(Context context, ILogger<HomeController> logger, IApplicationDateTime dateTime)
        {
            _context = context;
            _logger = logger;
            _dateTime = dateTime;
        }

        [Route("/")]
        public IActionResult Index()
        {
            // Public collections
            List<Collection> allCollections = _context.Collection.OrderBy(s => s.URL).ToList();
            List<string> collectionURLs = allCollections.Select(c => c.URL).ToList();

            // Public streams
            List<Stream> streams = _context.Stream.Where(s => (s.StartDate == null || s.StartDate < _dateTime.Now().AddDays(30)) && (s.EndDate == null || s.EndDate > _dateTime.Now()))
                .OrderBy(s => s.StartDate)
                .ToList()
                .ConvertAll(s => {
                    s.Collection = allCollections.First(c => c.URL == s.CollectionURL);
                    return s;
                })
                .Where(s => User.Identity.IsAuthenticated || (s.Private != true || s.Collection.Private != true))
                .ToList();

            HomeViewModel model = new HomeViewModel
            {
                PublicCollections = allCollections.Where(c => c.Private != true).ToList(),
                CurrentPublicStreams = streams
            };

            return View(model);
        }

        [Route("/error")]
        public IActionResult HandleError([FromQuery]int code, [FromQuery]string message, [FromQuery]string icon)
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

            ViewData["Message"] = errorMsg;
            ViewData["Color"] = "warning";
            ViewData["Icon"] = "fad fa-exclamation-triangle";

            return View("~/Views/Shared/Error.cshtml");
        }

        [Route("/exception")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["Message"] = "It's our fault! Sorry!";
            ViewData["Color"] = "danger";
            ViewData["Icon"] = "fad fa-exclamation-triangle";

            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
