using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGStream.Data;
using GGStream.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GGStream.Controllers
{
    public class PlayerController : Controller
    {
        private readonly Context _context;

        public PlayerController(Context context)
        {
            _context = context;
        }

        [Route("/{url}/{id?}")]
        public async Task<IActionResult> ViewStream(string url, string? id)
        {
            if (url == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null)
            {
                // Check if it's a direct stream link
                var stream = await _context.Stream.FindAsync(url);
                if (stream == null)
                {
                    return NotFound();
                }
                else
                {
                    // Attach Collection to stream
                    var streamCollection = await _context.Collection.FindAsync(stream.CollectionURL);

                    stream.Collection = streamCollection;
                    return View(stream);
                }
            } 
            else if (id == null)
            {
                // Determine which stream to play
                if (collection.Private)
                {
                    // TODO: In the future, show a "private" message here that tells users to query the stream
                    return NotFound("This collection is private");
                }
                else
                {
                    var stream = StreamToPlay(collection);
                    if (stream != null)
                    {
                        return View(stream);
                    }
                    else
                    {
                        return NotFound("There's no current streams playing");
                    }
                }
            }
            else
            {
                // This is a collection-stream link
                var stream = await _context.Stream.FindAsync(id);
                if (stream == null || stream.CollectionURL != url)
                {
                    return NotFound();
                } 
                else
                {
                    // Attach Collection to stream
                    var streamCollection = await _context.Collection.FindAsync(stream.CollectionURL);

                    stream.Collection = streamCollection;
                    return View(stream);
                }
            }
        }

        private Stream StreamToPlay(Collection collection)
        {
            var today = DateTime.Today;
            var stream = _context.Stream.FirstOrDefault((Stream s) =>
                s.Collection.URL == collection.URL &&
                (s.StartDate == null || s.StartDate > today) &&
                (s.EndDate == null || s.EndDate < today));

            // Attach collection to stream
            stream.Collection = collection;

            return stream;
        }
    }
}
