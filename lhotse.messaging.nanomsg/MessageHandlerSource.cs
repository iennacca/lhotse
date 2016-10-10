using System;
using System.Diagnostics;
using System.Text;
using NNanomsg;
using NNanomsg.Protocols;

namespace lhotse.messaging.nanomsg
{
    /**
     * Error codes:
     * Server error EBADF: 9
     * Server error EMFILE: 24
     * Server erro EINVALr: 22
     * Server error ENAMETOOLONG: 38
     * Server error EPROTONOSUPPORT: 135
     * Server error EADDRNOTAVAIL: 101
     * Server error ENODEV: 19
     * Server error EADDRINUSE: 100
     * Server error ETERM: 156384765
     **/

    /**
     * Test addresses:
     * tcp://test.lhotse:5000
     * inproc://reqrep_test
     **/
    public class MessageHandlerSource : IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>
    {
        public static MessageHandlerSource Factory = new MessageHandlerSource();
        private static readonly MessageHandlerUri AddressUri = new MessageHandlerUri("inproc://reqrep_test");

        private IRPCClient<TextRequest, TextResponse, TextProgressInfo> _client;
        private IRPCServer<TextRequest, TextResponse, TextProgressInfo> _server;

        public IRPCClient<TextRequest, TextResponse, TextProgressInfo> Client => _client ?? (_client = new ClientMessageHandler(AddressUri));
        public IRPCServer<TextRequest, TextResponse, TextProgressInfo> Server => _server ?? (_server = new ServerMessageHandler(AddressUri));

        private MessageHandlerSource() { }

        private class ServerMessageHandler : IRPCServer<TextRequest, TextResponse, TextProgressInfo>
        {
            private readonly ReplySocket _socket;

            public ServerMessageHandler(MessageHandlerUri address)
            {
                Address = address;
                Type = HandlerProtocol.Reply;
                _socket = new ReplySocket();
                _socket.Bind(address.OriginalString);
                Trace.WriteLine($"Server initialized: socket = {_socket}");
                Trace.WriteLine($"Server error: {NN.Errno()}");
            }

            public void Dispose()
            {
                _socket.Dispose();
            }

            public MessageHandlerUri Address { get; }
            public HandlerProtocol Type { get; }

            public void Respond(Func<TextRequest, TextResponse> callback)
            {
                var bs = _socket.Receive();
                var s = Encoding.UTF8.GetString(bs);

                var r = callback(new TextRequest(s));

                bs = Encoding.UTF8.GetBytes(r.Text);
                _socket.Send(bs);
            }

            public void PublishProgress(TextProgressInfo progressInfo)
            {
                throw new NotImplementedException();
            }
        }

        private class ClientMessageHandler : IRPCClient<TextRequest, TextResponse, TextProgressInfo>
        {
            private readonly RequestSocket _socket;

            public ClientMessageHandler(MessageHandlerUri address)
            {
                Address = address;
                Type = HandlerProtocol.Request;
                _socket = new RequestSocket();
                _socket.Connect(address.OriginalString);
                Trace.WriteLine($"Client initialized: socket = {_socket}");
                Trace.WriteLine($"Server error: {NN.Errno()}");
            }

            public void Dispose()
            {
                _socket.Dispose();
            }

            public MessageHandlerUri Address { get; }
            public HandlerProtocol Type { get; }

            public TextResponse Request(TextRequest request)
            {
                _socket.Send(Encoding.UTF8.GetBytes(request.Text));
                var bs = _socket.Receive();
                var s = Encoding.UTF8.GetString(bs);
                return new TextResponse(s);
            }

            public void SubscribeProgress(Action<TextProgressInfo> callback)
            {
                throw new NotImplementedException();
            }
        }
    }
}