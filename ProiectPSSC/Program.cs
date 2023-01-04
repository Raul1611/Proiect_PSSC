using ProiectPSSC.Domain;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Orders;

namespace ProiectPSSC
{
    class Program
    {
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            Task.Run(async () => { await Start(args); })
                            .GetAwaiter()
                            .GetResult();
        }

        static async Task Start(string[] args)
        {
            var listOfProducts = ReadListOfOrders().ToArray();
            PayOrderCommand command = new(listOfProducts);
            PayOrderWorkflow workflow = new PayOrderWorkflow();
            var result = await workflow.ExecuteAsync(command, CheckProductExists, CheckStock, CheckAddress);

            result.Match(
                    whenOrdersPaidFailedEvent: @event =>
                    {
                        Console.WriteLine($"Pay failed: {@event.Reason}");
                        return @event;
                    },
                    whenOrdersPaidScucceededEvent: @event =>
                    {
                        Console.WriteLine($"Pay succeeded.");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );

            Console.WriteLine("Hello World!");
        }

        private static List<EmptyOrder> ReadListOfOrders()
        {
            int count = 5;
            List<EmptyOrder> listOfOrders = new();
            do
            {
                //read registration number and grade and create a list of greads
                var quantity = ReadValue("Cantitatea produsului comandat: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                var product_code = ReadValue("Codul produsului: ");
                if (string.IsNullOrEmpty(product_code))
                {
                    break;
                }

                var address = ReadValue("Adresa: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }

                var price = ReadValue("Pretul: ");
                if (string.IsNullOrEmpty(price))
                {
                    break;
                }

                listOfOrders.Add(new(product_code, quantity, address, price));
            } while (count-- != 0);
            return listOfOrders;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        private static TryAsync<bool> CheckProductExists(ProductCode product) => async () => true;
        private static TryAsync<bool> CheckStock(ProductCode product, Quantity quantity) => async () => true;
        private static TryAsync<bool> CheckAddress(Address address) => async () => true;



    }
}
