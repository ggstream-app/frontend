using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GGStream.Data;
using GGStream.Models;

namespace GGStream.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly Context _context;

        public CollectionsController(Context context)
        {
            _context = context;
        }

        #region Public Routes

        // GET: personal (Collection's current stream)
        // GET: 12345678-1234-1234-123456789012 (Direct to stream)
        public async Task<IActionResult> ViewStream(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .FirstOrDefaultAsync(m => m.URL == url);
            if (collection == null)
            {
                // Check if it's a direct stream link
                var stream = await _context.Stream
                    .FirstOrDefaultAsync(m => m.ID == new Guid(url));
                if (stream == null)
                {
                    return NotFound();
                }
                else
                {
                    // Attach Collection to stream
                    var streamCollection = await _context.Collection
                        .FirstOrDefaultAsync(m => m.URL == stream.CollectionURL);

                    stream.Collection = streamCollection;
                    return View("../Streams/ViewStream", stream);
                }
            }

            // Determine which stream to play
            Stream streamToPlay;
            if (collection.Private)
            {
                // TODO: In the future, show a "private" message here that tells users to query the stream
                return NotFound("This collection is private");
            }
            else
            {
                streamToPlay = StreamToPlay(collection);
            }

            if (streamToPlay != null)
            {
                return View("../Streams/ViewStream", streamToPlay);
            }

            // TODO: In the future, show a "no stream playing" message here
            return NotFound("There's no streams playing!");
        }

        #endregion

        #region Admin Routes

        // GET: Collections
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collection.ToListAsync());
        }

        // GET: Collections/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .FirstOrDefaultAsync(m => m.URL == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("URL,Name,BaseColor,Private,ShowHowTo,TeamsLink")] Collection collection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("URL,Name,BaseColor,Private,ShowHowTo,TeamsLink")] Collection collection)
        {
            if (id != collection.URL)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionExists(collection.URL))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection
                .FirstOrDefaultAsync(m => m.URL == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var collection = await _context.Collection.FindAsync(id);
            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(string id)
        {
            return _context.Collection.Any(e => e.URL == id);
        }

        #endregion

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
