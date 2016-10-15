using System.ServiceProcess;
using lhotse.ioc;

namespace lhotse.messaging.server
{
    internal static class Program
    {
        public static readonly IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo> Factory = IOCContainer.Factory.Value;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new MessagingService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
