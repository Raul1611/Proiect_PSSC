using System;
using System.Collections.Generic;

namespace ProiectPSSC.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Code { get; set; } = null!;

    public int Stock { get; set; }

    public virtual ICollection<OrderLine> OrderLineOrders { get; } = new List<OrderLine>();

    public virtual ICollection<OrderLine> OrderLineProducts { get; } = new List<OrderLine>();
}
