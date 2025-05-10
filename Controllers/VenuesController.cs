using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseCloud.Models;

namespace EventEaseCloud.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseWebContext _context;

       
        private readonly Services.BlobStorageService _blobStorageService;

        public VenuesController(EventEaseWebContext context, Services.BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }



        /* public VenuesController(EventEaseWebContext context)
         {
             _context = context;
         }*/

        // GET: Venues
        public async Task<IActionResult> Index(string searchQuery, string sortBy)
        {
            var venues = _context.Venues.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                venues = venues.Where(v =>
                    v.VenueName.Contains(searchQuery) ||
                    v.Location.Contains(searchQuery));
            }

            switch (sortBy)
            {
                case "name_asc":
                    venues = venues.OrderBy(v => v.VenueName);
                    break;
                case "name_desc":
                    venues = venues.OrderByDescending(v => v.VenueName);
                    break;
                case "location_asc":
                    venues = venues.OrderBy(v => v.Location);
                    break;
                case "location_desc":
                    venues = venues.OrderByDescending(v => v.Location);
                    break;
            }

            return View(await venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    venue.ImageUrl = await _blobStorageService.UploadFileAsync(stream, imageFile.FileName);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("ImageUrl", "Image is required.");
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
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
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {



            bool hasBookings = _context.Bookings.Any(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete this venue because it has active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue != null)
            {
                _context.Venues.Remove(venue);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
