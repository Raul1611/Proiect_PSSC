using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain
{
    [AsChoice]
    public static partial class OrderPaidEvent
    {
        public interface IOrderPaidEvent { }

        public record OrdersPaidScucceededEvent : IOrderPaidEvent
        {
            public string Csv { get; }
            public DateTime PublishedDate { get; }

            internal OrdersPaidScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record OrdersPaidFailedEvent : IOrderPaidEvent
        {
            public string Reason { get; }

            internal OrdersPaidFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
