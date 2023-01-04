using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Orders;

namespace ProiectPSSC.Domain
{
    public static class OrdersOperations
    {
        public static Task<IOrders> ValidateOrders(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductCode, Quantity, TryAsync<bool>> checkStock, Func<Address, TryAsync<bool>> checkAddress, EmptyOrders Orders) =>
            Orders.OrderList
                      .Select(ValidateOrder(checkProductExists, checkStock, checkAddress))
                      .Aggregate(CreateEmptyValidatedOrderList().ToAsync(), ReduceValidOrders)
                      .MatchAsync(
                            Right: validatedOrders => new ValidatedOrders(validatedOrders),
                            LeftAsync: errorMessage => Task.FromResult((IOrders)new UnvalidatedOrders(Orders.OrderList, errorMessage))
                      );
        private static Func<EmptyOrder, EitherAsync<string, ValidatedOrder>> ValidateOrder(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductCode, Quantity, TryAsync<bool>> checkStock, Func<Address, TryAsync<bool>> checkAddress) =>
        emptyOrder => ValidateOrder(checkProductExists, checkStock, checkAddress, emptyOrder);

        private static EitherAsync<string, ValidatedOrder> ValidateOrder(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductCode, Quantity, TryAsync<bool>> checkStock, Func<Address, TryAsync<bool>> checkAddress, EmptyOrder emptyOrder) =>
            from address in Address.TryParse(emptyOrder.address)
                                    .ToEitherAsync(() => $"Invalid address ({emptyOrder.productCode}, {emptyOrder.address})")
            from productCode in ProductCode.TryParse(emptyOrder.productCode)
                                    .ToEitherAsync(() => $"Invalid product code ({emptyOrder.productCode})")
            from quantity in Quantity.TryParse(emptyOrder.quantity)
                                    .ToEitherAsync(() => $"Invalid quantity ({emptyOrder.productCode}, {emptyOrder.quantity})")
            from price in Price.TryParse(emptyOrder.price)
                        .ToEitherAsync(() => $"Invalid price ({emptyOrder.productCode}, {emptyOrder.price})")
            from productExists in checkProductExists(productCode)
                                    .ToEither(error => error.ToString())
            from stockOK in checkStock(productCode, quantity)
                                    .ToEither(error => error.ToString())
            from addressOK in checkAddress(address)
                                    .ToEither(error => error.ToString())
            select new ValidatedOrder(productCode, quantity, address, price);

        private static Either<string, List<ValidatedOrder>> CreateEmptyValidatedOrderList() =>
            Right(new List<ValidatedOrder>());

        private static EitherAsync<string, List<ValidatedOrder>> ReduceValidOrders(EitherAsync<string, List<ValidatedOrder>> acc, EitherAsync<string, ValidatedOrder> next) =>
            from list in acc
            from nextOrder in next
            select list.AppendValidOrder(nextOrder);

        private static List<ValidatedOrder> AppendValidOrder(this List<ValidatedOrder> list, ValidatedOrder validOrder)
        {
            list.Add(validOrder);
            return list;
        }

        public static IOrders CalculateFinalPrices(IOrders Orders) => Orders.Match(
            whenEmptyOrders: emptyOrder => emptyOrder,
            whenUnvalidatedOrders: unvalidatedOrder => unvalidatedOrder,
            whenCalculatedOrders: calculatedOrder => calculatedOrder,
            whenPaidOrders: paidOrder => paidOrder,
            whenValidatedOrders: CalculateFinalPrice
        );

        private static IOrders CalculateFinalPrice(ValidatedOrders validOrders) =>
            new CalculatedOrders(validOrders.OrderList
                                                          .Select(CalculateOrderFinalPrice)
                                                          .ToList()
                                                          .AsReadOnly());
        private static CalculatedOrder CalculateOrderFinalPrice(ValidatedOrder validOrder) =>
            new CalculatedOrder(validOrder.productCode,
                                      validOrder.quantity,
                                      validOrder.address,
                                      validOrder.price,
                                      validOrder.price * validOrder.quantity);
        public static IOrders PayOrders(IOrders Orders) => Orders.Match(
            whenEmptyOrders: emptyOrder => emptyOrder,
            whenUnvalidatedOrders: unvalidatedOrder => unvalidatedOrder,
            whenPaidOrders: paidOrder => paidOrder,
            whenValidatedOrders: validatedOrder => validatedOrder,
            whenCalculatedOrders: GenerateExport
        );

        private static IOrders GenerateExport(CalculatedOrders calculatedOrder) =>
            new PaidOrders(calculatedOrder.OrderList,
                                    calculatedOrder.OrderList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(),
                                    DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedOrder Order) =>
            export.AppendLine($"{Order.productCode.Code}, {Order.price}, {Order.quantity}, {Order.finalPrice}");
    }
}
