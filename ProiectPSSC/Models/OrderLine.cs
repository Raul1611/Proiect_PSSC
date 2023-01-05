using System;
using System.Collections.Generic;

namespace ProiectPSSC.Models;

public partial class OrderLine
{
    public int OrderLineId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Product Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
