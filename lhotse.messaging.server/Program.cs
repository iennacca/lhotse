using System.ServiceProcess;
using lhotse.common;
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
            if (Factory == null) return;
            TraceExtensions.WriteInfoLine($"Assembly:{Factory.HandlerName}; Version:{Factory.HandlerVersion}");

            var servicesToRun = new ServiceBase[]
            {
                new MessagingService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
