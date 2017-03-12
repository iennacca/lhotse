using System;
using lhotse.common;
using lhotse.ioc;

namespace lhotse.messaging.client
{
    internal class Program
    {
        private static void Main()
        {
            var factory = IOCContainer.Factory.Value;
            TraceExtensions.WriteInfoLine($"Assembly:{factory.HandlerName}; Version:{factory.HandlerVersion}");

            using (var handler = factory.Client)
            {
                handler.SubscribeProgress((t) => Console.WriteLine(t.Text));
                Console.WriteLine(@"Sending request");
                var returnValue = handler.Request(new TextRequest("This is a test"));
                Console.WriteLine($"Received reply: {returnValue.Text}");
            }
        }
    }
}
