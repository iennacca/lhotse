using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace lhotse.messaging.zeromq
{
    public class LazyPirateClient : IRPCClient<TextRequest, TextResponse, TextProgressInfo>
    {
        public LazyPirateClient(MessageHandlerUri address)
        {
            Address = address;
            Type = HandlerProtocol.Request;
        }

        public void Dispose() { }

        public MessageHandlerUri Address { get; }
        public HandlerProtocol Type { get; }

        private static TextResponse _response;

        public TextResponse Request(TextRequest request)
        {
            RequestSocket client = CreateClientSocket();
            Console.WriteLine("C: Sending ({0})", request.Text);
            client.SendFrame(Encoding.Unicode.GetBytes(request.Text));
            client.Poll(TimeSpan.FromMilliseconds(RequestTimeout));
            TerminateClient(client);
            return _response;
        }

        public void SubscribeProgress(Action<TextProgressInfo> callback)
        {
            throw new NotImplementedException();
        }

        //---------------------------------------------------------

        private const int RequestTimeout = 2500;

        private void TerminateClient(NetMQSocket client)
        {
            client.Disconnect(Address.OriginalString);
            client.Close();
        }

        private RequestSocket CreateClientSocket()
        {
            Console.WriteLine("C: Connecting to server...");

            var client = new RequestSocket();
            client.Connect(Address.OriginalString);
            client.Options.Linger = TimeSpan.Zero;
            client.ReceiveReady += ClientOnReceiveReady;

            return client;
        }

        private void ClientOnReceiveReady(object sender, NetMQSocketEventArgs args)
        {
            var reply = args.Socket.ReceiveFrameBytes();
            var strReply = Encoding.Unicode.GetString(reply);

            _response = new TextResponse(strReply);
        }
    }
}