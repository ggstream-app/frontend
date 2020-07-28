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
    public class StreamsController : Controller
    {
        private readonly Context _context;

        public StreamsController(Context context)
        {
            _context = context;
        }

        #region Public Routes

        // GET: personal/12345678-1234-1234-123456789012
        public async Task<IActionResult> ViewStream(string url, Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream
                .FirstOrDefaultAsync(m => m.StreamKey == id);
            if (stream == null)
            {
                return NotFound();
            }

            if (stream.Collection.URL != url)
            {
                return NotFound();
            }

            return View(stream);
        }

        #endregion

        #region Admin Routes
        // GET: Streams
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stream.ToListAsync());
        }

        // GET: Streams/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream
                .FirstOrDefaultAsync(m => m.StreamKey == id);
            if (stream == null)
            {
                return NotFound();
            }

            return View(stream);
        }

        // GET: Streams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Streams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StreamKey,StartDate,EndDate,ShowHowTo,TeamsLink")] Stream stream)
        {
            if (ModelState.IsValid)
            {
                stream.StreamKey = Guid.NewGuid();
                _context.Add(stream);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stream);
        }

        // GET: Streams/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream.FindAsync(id);
            if (stream == null)
            {
                return NotFound();
            }
            return View(stream);
        }

        // POST: Streams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("StreamKey,StartDate,EndDate,ShowHowTo,TeamsLink")] Stream stream)
        {
            if (id != stream.StreamKey)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stream);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StreamExists(stream.StreamKey))
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
            return View(stream);
        }

        // GET: Streams/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream
                .FirstOrDefaultAsync(m => m.StreamKey == id);
            if (stream == null)
            {
                return NotFound();
            }

            return View(stream);
        }

        // POST: Streams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var stream = await _context.Stream.FindAsync(id);
            _context.Stream.Remove(stream);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        private bool StreamExists(Guid id)
        {
            return _context.Stream.Any(e => e.StreamKey == id);
        }
    }
}
