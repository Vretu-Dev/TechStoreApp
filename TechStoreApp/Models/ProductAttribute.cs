using System;
using System.Collections.Generic;

namespace TechStoreApp.Models;

public partial class ProductAttribute
{
    public int AttributeId { get; set; }

    public int ProductId { get; set; }

    public string KeyName { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
