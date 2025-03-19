using System;
using System.Collections.Generic;

namespace EventEaseCloud.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public DateTime BookingDate { get; set; }

    public int? EventId { get; set; }

    public int? VenueId { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Venue? Venue { get; set; }
}
