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
using Microsoft.AspNetCore.Authorization;

namespace GGStream.Controllers
{
    [Authorize]
    public class StreamsController : Controller
    {
        private readonly Context _context;

        public StreamsController(Context context)
        {
            _context = context;
        }

        [Route("/admin/streams")]
        public async Task<IActionResult> Index()
        {
            ViewData["DisableCreate"] = true;
            return View(await _context.Stream.ToListAsync());
        }

        [Route("/admin/{url}/streams")]
        public async Task<IActionResult> Index(string url)
        {
            Collection collection = await _context.Collection.FindAsync(url);

            if (collection == null)
            {
                return NotFound();
            }

            var streams = await _context.Stream.Where(s => s.CollectionURL == url).ToListAsync();

            streams.ConvertAll(s =>
            {
                s.Collection = collection;
                return s;
            });

            return View(streams);
        }

        [Route("/admin/{url}/streams/{id}")]
        public async Task<IActionResult> Details(string id)
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

        [Route("/admin/{url}/streams/create")]
        public async Task<IActionResult> Create(string url)
        {
            Collection collection = await _context.Collection.FindAsync(url);

            if (collection == null)
            {
                return NotFound();
            }

            ViewData["Collection:Name"] = collection.Name;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/streams/create")]
        public async Task<IActionResult> Create(string url, [Bind("Name,StartDate,EndDate,Private")] Stream stream)
        {
            Collection collection = await _context.Collection.FindAsync(url);
            ViewData["Collection:Name"] = collection.Name;

            if (collection == null)
            {
                return NotFound();
            } 
            else
            {
                stream.Collection = collection;
                stream.CollectionURL = url;
            }

            if (ModelState.IsValid)
            {
                stream.ID = Nanoid.Nanoid.Generate("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", 10);
                stream.StreamKey = $"{url}-{Nanoid.Nanoid.Generate("_0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ")}";
                _context.Add(stream);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { url });
            }

            return View(stream);
        }

        [Route("/admin/{url}/streams/{id}/edit")]
        public async Task<IActionResult> Edit(string url, string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream.FindAsync(id);
            if (stream == null || stream.CollectionURL != url)
            {
                return NotFound();
            }
            return View(stream);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/streams/{id}/edit")]
        public async Task<IActionResult> Edit(string url, string id, [Bind("ID,CollectionURL,StreamKey,Name,StartDate,EndDate,Private")] Stream stream)
        {
            if (id != stream.ID || url != stream.CollectionURL)
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
                    if (!StreamExists(stream.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { url });
            }
            return View(stream);
        }

        [Route("/admin/{url}/streams/{id}/delete")]
        public async Task<IActionResult> Delete(string url, string id)
        {
            if (id == null || url == null)
            {
                return NotFound();
            }

            var stream = await _context.Stream.FindAsync(id);
            if (stream == null || stream.CollectionURL != url)
            {
                return NotFound();
            }

            return View(stream);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/streams/{id}/delete")]
        public async Task<IActionResult> DeleteConfirmed(string url, string id)
        {
            var stream = await _context.Stream.FindAsync(id);
            if (stream == null || stream.CollectionURL != url)
            {
                return NotFound();
            }

            _context.Stream.Remove(stream);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { url });
        }

        private bool StreamExists(string id)
        {
            return _context.Stream.Any(e => e.ID == id);
        }
    }
}
