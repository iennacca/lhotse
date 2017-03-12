using System;
using System.Reflection;
using EasyNetQ;

namespace lhotse.messaging.easynetq
{
    public class MessageHandlerSource : IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>
    {
        public static MessageHandlerSource Factory = new MessageHandlerSource();
        private static readonly MessageHandlerUri AddressUri = new MessageHandlerUri("localhost");
        private static readonly AssemblyName AssemblyNameInfo = Assembly.GetExecutingAssembly().GetName();

        private string _handlerName;
        private Version _handlerVersion;
        private IRPCClient<TextRequest, TextResponse, TextProgressInfo> _client; 
        private IRPCServer<TextRequest, TextResponse, TextProgressInfo> _server;

        public string HandlerName => _handlerName ?? (_handlerName = AssemblyNameInfo.Name);
        public Version HandlerVersion => _handlerVersion ?? (_handlerVersion = AssemblyNameInfo.Version);
        public IRPCClient<TextRequest, TextResponse, TextProgressInfo> Client => _client ?? (_client = new ClientMessageHandler(AddressUri));
        public IRPCServer<TextRequest, TextResponse, TextProgressInfo> Server => _server ?? (_server = new ServerMessageHandler(AddressUri));

        private MessageHandlerSource() { }

        private class ClientMessageHandler : IRPCClient<TextRequest, TextResponse, TextProgressInfo>
        {
            private readonly IBus _messageBus;

            public ClientMessageHandler(MessageHandlerUri address)
            {
                Address = address;
                Type = HandlerProtocol.Request;
                var username = Properties.Resources.MessageBrokerAdminUserName;
                var password = Properties.Resources.MessageBrokerAdminPassword;
                var timeout = Properties.Resources.MessageBrokerTimeout;
                _messageBus = RabbitHutch.CreateBus($"host={Address.OriginalString};username={username};password={password};timeout={timeout}");
            }

            public void Dispose()
            {
                _messageBus.Dispose();
            }

            public MessageHandlerUri Address { get; }
            public HandlerProtocol Type { get; }

            public TextResponse Request(TextRequest request)
            {
                return _messageBus.Request<TextRequest, TextResponse>(request);
            }

            private const string SubscriptionID = "lhotse.test.subscription";

            public void SubscribeProgress(Action<TextProgressInfo> callback)
            {
                _messageBus.Subscribe(SubscriptionID, callback);
            }
        }

        private class ServerMessageHandler : IRPCServer<TextRequest, TextResponse, TextProgressInfo>
        {
            private readonly IBus _messageBus;

            public ServerMessageHandler(MessageHandlerUri address)
            {
                Address = address;
                Type = HandlerProtocol.Reply;
                _messageBus = RabbitHutch.CreateBus($"host={Address.OriginalString};username=guest;password=elsinore;timeout=60");
            }

            public void Dispose()
            {
                _messageBus.Dispose();
            }

            public MessageHandlerUri Address { get; }
            public HandlerProtocol Type { get; }

            public void Respond(Func<TextRequest, TextResponse> callback)
            {
                _messageBus.Respond(callback);
            }

            public void PublishProgress(TextProgressInfo progressInfo)
            {
                _messageBus.Publish(progressInfo);
            }
        }
    }

}
