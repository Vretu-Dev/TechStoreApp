using System;
using System.Collections.Generic;

namespace TechStoreApp.Models;

public partial class Courier
{
    public int CourierId { get; set; }
    public string Name { get; set; } = null!;
    public decimal BaseShippingCost { get; set; }
    public string? EstimatedDeliveryTime { get; set; }
}
