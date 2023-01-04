using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain
{
    public record CalculatedOrder(ProductCode productCode, Quantity quantity, Address address, Price price, Price finalPrice);
}
