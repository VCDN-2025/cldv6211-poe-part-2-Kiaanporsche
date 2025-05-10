using System;
using System.Collections.Generic;

namespace EventEaseCloud.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    
    public string VenueName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
