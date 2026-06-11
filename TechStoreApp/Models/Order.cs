using System;
using System.Collections.Generic;

namespace TechStoreApp.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? ShippingAddress { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingMethod { get; set; }
    public string? PaymentMethod { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}
