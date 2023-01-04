using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace ProiectPSSC.Domain
{
    [AsChoice]
    public static partial class Orders
    {
        public interface IOrders { }

        public record EmptyOrders : IOrders
        {
            public EmptyOrders(IReadOnlyCollection<EmptyOrder> OrderList)
            {
                this.OrderList = OrderList;
            }

            public IReadOnlyCollection<EmptyOrder> OrderList { get; }
        }

        public record UnvalidatedOrders : IOrders
        {
            internal UnvalidatedOrders(IReadOnlyCollection<EmptyOrder> OrderList, string reason)
            {
                this.OrderList = OrderList;
                Reason = reason;
            }

            public IReadOnlyCollection<EmptyOrder> OrderList { get; }
            public string Reason { get; }
        }

        public record ValidatedOrders : IOrders
        {
            internal ValidatedOrders(IReadOnlyCollection<ValidatedOrder> OrderList)
            {
                this.OrderList = OrderList;
            }

            public IReadOnlyCollection<ValidatedOrder> OrderList { get; }
        }

        public record CalculatedOrders : IOrders
        {
            internal CalculatedOrders(IReadOnlyCollection<CalculatedOrder> OrderList)
            {
                this.OrderList = OrderList;
            }

            public IReadOnlyCollection<CalculatedOrder> OrderList { get; }
        }

        public record PaidOrders : IOrders
        {
            internal PaidOrders(IReadOnlyCollection<CalculatedOrder> OrdersList, string csv, DateTime publishedDate)
            {
                OrderList = OrdersList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedOrder> OrderList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}
