using System;
using lhotse.ioc;

namespace lhotse.messaging.client
{
    internal class Program
    {
        private static void Main()
        {
            var factory = IOCContainer.Container.GetExport<IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>>();

            if (factory == null) return;
            using (var handler = factory.Value.Client)
            {
                handler.SubscribeProgress((t) => Console.WriteLine(t.Text));
                Console.WriteLine(@"Sending request");
                var returnValue = handler.Request(new TextRequest("This is a test"));
                Console.WriteLine($"Received reply: {returnValue.Text}");
            }
        }
    }
}
