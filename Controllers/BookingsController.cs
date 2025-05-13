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
    public class BookingsController : Controller
    {
        private readonly EventEaseWebContext _context;

        public BookingsController(EventEaseWebContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchQuery, string sortBy)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            // Filter by searchQuery
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                bookings = bookings.Where(b =>
                    b.Event.EventName.Contains(searchQuery) ||
                    b.Venue.VenueName.Contains(searchQuery));
            }

            // Sorting
            switch (sortBy)
            {
                case "name_asc":
                    bookings = bookings.OrderBy(b => b.Event.EventName);
                    break;
                case "name_desc":
                    bookings = bookings.OrderByDescending(b => b.Event.EventName);
                    break;
                case "date_asc":
                    bookings = bookings.OrderBy(b => b.BookingDate);
                    break;
                case "date_desc":
                    bookings = bookings.OrderByDescending(b => b.BookingDate);
                    break;
            }

            return View(await bookings.ToListAsync());


            //Title: search function for bookings
            //Author: Microsoft
            //Date: 6 May 2025
            //Code cersion: 1
            //Availability: https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application 


        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,BookingDate,EventId,VenueId")] Booking booking)
        {


            bool isDoubleBooked = _context.Bookings.Any(b =>
           b.VenueId == booking.VenueId &&
           EF.Functions.DateDiffDay(b.BookingDate, booking.BookingDate) == 0 &&
           b.BookingId != booking.BookingId);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "This venue is already booked on the selected date.");
            }




            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);



            //Title: validation for double bookings
            //Author: OpenAI(Provided by ChatGPT)
            //Date: 7 May 2025
            //Code cersion: 1
            //Availability: https://chatgpt.com/share/67ed2bb2-0490-8008-ad3c-92c311934736

        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,BookingDate,EventId,VenueId")] Booking booking)
        {



            bool isDoubleBooked = _context.Bookings.Any(b =>
            b.VenueId == booking.VenueId &&
            EF.Functions.DateDiffDay(b.BookingDate, booking.BookingDate) == 0 &&
            b.BookingId != booking.BookingId);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "This venue is already booked on the selected date.");
            }
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);


            //Title: validation for double bookings
            //Author: OpenAI(Provided by ChatGPT)
            //Date: 7 May 2025
            //Code cersion: 1
            //Availability: https://chatgpt.com/share/67ed2bb2-0490-8008-ad3c-92c311934736


        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
