using System;
using System.Collections.Generic;

namespace ProiectPSSC.Models;

public partial class OrderHeader
{
    public int OrderId { get; set; }

    public string Address { get; set; } = null!;

    public decimal? Total { get; set; }
}
