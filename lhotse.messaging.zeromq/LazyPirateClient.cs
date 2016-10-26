using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace lhotse.messaging.zeromq
{
    public class LazyPirateClient : IRPCClient<TextRequest, TextResponse, TextProgressInfo>
    {
        public LazyPirateClient(MessageHandlerUri address, MessageHandlerUri statusUri, string topic)
        {
            Address = address;
            Type = HandlerProtocol.Request;
            Topic = topic;
            StatusUri = statusUri;
        }

        public void Dispose() { }

        public MessageHandlerUri Address { get; }
        public HandlerProtocol Type { get; }
        public string Topic { get; }
        public MessageHandlerUri StatusUri { get; }

        private static TextResponse _response;

        public TextResponse Request(TextRequest request)
        {
            RequestSocket client = CreateClientSocket();
            client.SendFrame(Encoding.Unicode.GetBytes(request.Text));
            client.Poll(TimeSpan.FromMilliseconds(RequestTimeout));
            TerminateClient(client);
            return _response;
        }

        public void SubscribeProgress(Action<TextProgressInfo> callback)
        {
            Task.Run(() =>
            {
                using (var socket = new SubscriberSocket())
                {
                    socket.Options.ReceiveHighWatermark = 1000;
                    socket.Connect(StatusUri.OriginalString);
                    socket.Subscribe(Topic);
                    {
                        var messageTopicReceived = socket.ReceiveFrameString();
                        Debug.Assert(messageTopicReceived.Equals(Topic));

                        var messageReceived = socket.ReceiveFrameString();
                        callback(new TextProgressInfo(messageReceived));
                        Console.WriteLine(messageReceived);
                    }
                }
            });
        }

        //---------------------------------------------------------

        private const int RequestTimeout = 20000;

        private void TerminateClient(NetMQSocket client)
        {
            client.Disconnect(Address.OriginalString);
            client.Close();
        }

        private RequestSocket CreateClientSocket()
        {
            var client = new RequestSocket();
            client.Connect(Address.OriginalString);
            client.Options.Linger = TimeSpan.Zero;
            client.ReceiveReady += ClientOnReceiveReady;

            return client;
        }

        private void ClientOnReceiveReady(object sender, NetMQSocketEventArgs args)
        {
            StringBuilder frameString = new StringBuilder();
            var more = true;

            while (more)
            {
                string str;
                args.Socket.TryReceiveFrameString(TimeSpan.MaxValue, Encoding.UTF8, out str, out more);
                frameString.Append(str);
            }
            _response = new TextResponse(frameString.ToString());
        }
    }
}