using System.Linq;
using System.Threading.Tasks;
using GGStream.Data;
using GGStream.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GGStream.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IConfiguration _config;
        private readonly Context _context;
        private readonly IApplicationDateTime _dateTime;

        public PlayerController(Context context, IConfiguration config, ApplicationDateTime dateTime)
        {
            _context = context;
            _config = config;
            _dateTime = dateTime;
        }

        [Route("/{url}/{id?}")]
        public async Task<IActionResult> ViewStream(string url, string id)
        {
            if (url == null) return NotFound();

            ViewData["IngestEndpoint"] = $"rtmp://{_config.GetValue<string>("IngestEndpoint")}:1935/app/";

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null)
            {
                // Check if it's a direct stream link
                var stream = await _context.Stream.FindAsync(url);
                if (stream == null) return NotFound();

                if ((stream.StartDate == null || stream.StartDate < _dateTime.Now()) &&
                    (stream.EndDate == null || stream.EndDate > _dateTime.Now()))
                {
                    // Attach Collection to stream
                    var streamCollection = await _context.Collection.FindAsync(stream.CollectionURL);

                    stream.Collection = streamCollection;
                    return View(stream);
                }

                ViewData["Message"] = "This stream isn't live right now.";
                ViewData["Color"] = "info";
                ViewData["Icon"] = "fad fa-calendar-times";

                return View("Error");
            }

            if (id == null)
            {
                // Determine which stream to play
                if (collection.Private) return NotFound();

                var stream = StreamToPlay(collection);
                if (stream != null) return View(stream);

                ViewData["Message"] = "There are no streams live right now.";
                ViewData["Color"] = "info";
                ViewData["Icon"] = "fad fa-calendar-times";

                return View("Error");
            }

            {
                // This is a collection-stream link
                var stream = await _context.Stream.FindAsync(id);
                if (stream == null || stream.CollectionURL != url) return NotFound();

                // Attach Collection to stream
                var streamCollection = await _context.Collection.FindAsync(stream.CollectionURL);

                stream.Collection = streamCollection;
                return View(stream);
            }
        }

        private Stream StreamToPlay(Collection collection)
        {
            var today = _dateTime.Now();
            var stream = _context.Stream.FirstOrDefault(s =>
                s.Private != true &&
                s.Collection.URL == collection.URL &&
                (s.StartDate == null || s.StartDate < today) &&
                (s.EndDate == null || s.EndDate > today));

            if (stream == null) return null;

            // Attach collection to stream
            stream.Collection = collection;

            return stream;
        }
    }
}