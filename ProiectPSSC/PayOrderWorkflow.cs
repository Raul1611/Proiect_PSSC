using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.OrderPaidEvent;
using static ProiectPSSC.Domain.Orders;
using static ProiectPSSC.Domain.OrdersOperations;
using LanguageExt;

namespace ProiectPSSC.Domain
{
    public class PayOrderWorkflow
    {
        public async Task<IOrderPaidEvent> ExecuteAsync(PayOrderCommand command, Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductCode, Quantity, TryAsync<bool>> checkStock, Func<Address, TryAsync<bool>> checkAddress)
        {
            EmptyOrders emptyOrders = new EmptyOrders(command.InputOrders);
            IOrders Orders = await ValidateOrders(checkProductExists, checkStock, checkAddress, emptyOrders);
            Orders = CalculateFinalPrices(Orders);
            Orders = PayOrders(Orders);

            return Orders.Match(
                    whenEmptyOrders: emptyOrders => new OrdersPaidFailedEvent("Unexpected unvalidated state") as IOrderPaidEvent,
                    whenUnvalidatedOrders: unvalidatedOrders => new OrdersPaidFailedEvent(unvalidatedOrders.Reason),
                    whenValidatedOrders: validatedOrders => new OrdersPaidFailedEvent("Unexpected validated state"),
                    whenCalculatedOrders: calculatedOrders => new OrdersPaidFailedEvent("Unexpected calculated state"),
                    whenPaidOrders: paidOrders => new OrdersPaidScucceededEvent(paidOrders.Csv, paidOrders.PublishedDate)
                );
        }
    }
}
