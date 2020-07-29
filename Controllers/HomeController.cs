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
            List<Collection> collections = _context.Collection.Where(c => c.Private == false).OrderBy(s => s.URL).ToList();
            List<string> collectionURLs = collections.Select(c => c.URL).ToList();
            List<Stream> streams = _context.Stream.Where(s => (s.EndDate == null || s.EndDate > DateTime.Now) && collectionURLs.Contains(s.CollectionURL)).OrderBy(s => s.StartDate).ToList();

            HomeModel model = new HomeModel
            {
                PublicCollections = collections,
                UpcomingPublicStreams = streams
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
