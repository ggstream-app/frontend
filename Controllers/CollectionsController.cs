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

        // GET: Collections
        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collection.ToListAsync());
        }

        // GET: Collections/Details/5
        [Route("/admin/{url}")]
        public async Task<IActionResult> Details(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // GET: Collections/Create
        [Route("/admin/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/create")]
        public async Task<IActionResult> Create([Bind("URL,Name,Icon,BaseColor,Private,InstructionType,CallLink")] Collection collection)
        {
            if (collection.BaseColor == "#000000")
            {
                collection.BaseColor = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collection);
        }

        // GET: Collections/Edit/5
        [Route("/admin/{url}/edit")]
        public async Task<IActionResult> Edit(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(url);
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
        [Route("/admin/{url}/edit")]
        public async Task<IActionResult> Edit(string url, [Bind("URL,Name,Icon,BaseColor,Private,InstructionType,CallLink")] Collection collection)
        {
            if (url != collection.URL)
            {
                return NotFound();
            }

            if (collection.BaseColor == "#000000")
            {
                collection.BaseColor = null;
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
        [Route("/admin/{url}/delete")]
        public async Task<IActionResult> Delete(string url)
        {
            if (url == null)
            {
                return NotFound();
            }

            var collection = await _context.Collection.FindAsync(url);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/admin/{url}/delete")]
        public async Task<IActionResult> DeleteConfirmed(string url)
        {
            var collection = await _context.Collection.FindAsync(url);
            _context.Collection.Remove(collection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CollectionExists(string url)
        {
            return _context.Collection.Any(e => e.URL == url);
        }
    }
}
