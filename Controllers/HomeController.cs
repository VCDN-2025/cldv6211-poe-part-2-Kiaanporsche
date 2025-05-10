using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventEaseCloud.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEaseCloud.Controllers;

//public class HomeController : Controller
//{
//    private readonly ILogger<HomeController> _logger;

//    public HomeController(ILogger<HomeController> logger)
//    {
//        _logger = logger;
//    }

//    public IActionResult Index()
//    {
//        return View();
//    }

//    public IActionResult Privacy()
//    {
//        return View();
//    }

//    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//    public IActionResult Error()
//    {
//        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//    }
//}



public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EventEaseWebContext _context; // <-- Add this

    public HomeController(ILogger<HomeController> logger, EventEaseWebContext context)
    {
        _logger = logger;
        _context = context; // <-- Add this
    }

    public async Task<IActionResult> Index(string searchQuery)
    {
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            // Search Events, Venues, and Bookings
            var events = await _context.Events
                .Where(e => e.EventName.Contains(searchQuery) || e.Description.Contains(searchQuery))
                .ToListAsync();

            var venues = await _context.Venues
                .Where(v => v.VenueName.Contains(searchQuery) || v.Location.Contains(searchQuery))
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .Where(b => b.Event.EventName.Contains(searchQuery) || b.Venue.VenueName.Contains(searchQuery))
                .ToListAsync();

            // You can pass all 3 results into the ViewBag (or create a ViewModel if you want to be more organized)
            ViewBag.Events = events;
            ViewBag.Venues = venues;
            ViewBag.Bookings = bookings;
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
