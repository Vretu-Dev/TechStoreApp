using System;
using System.Collections.Generic;

namespace TechStoreApp.Models;

public partial class Shipment
{
    public int ShipmentId { get; set; }

    public int OrderId { get; set; }

    public string? TrackingNumber { get; set; }

    public string CourierName { get; set; } = null!;

    public DateTime? ShippedDate { get; set; }

    public virtual Order Order { get; set; } = null!;
}
