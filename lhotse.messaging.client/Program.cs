using System;
using lhotse.messaging.easynetq;

namespace lhotse.messaging.client
{
    internal class Program
    {
        private static void Main()
        {
            var factory = MessageHandlerSource.Factory;
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
