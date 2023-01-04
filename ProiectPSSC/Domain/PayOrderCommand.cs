using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain
{   
    public record PayOrderCommand
    {
        public PayOrderCommand(IReadOnlyCollection<EmptyOrder> inputOrders)
        {
            InputOrders = inputOrders;
        }

        public IReadOnlyCollection<EmptyOrder> InputOrders { get; }
    }
}
