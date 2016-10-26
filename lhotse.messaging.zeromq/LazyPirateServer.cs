using System;
using System.Diagnostics;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace lhotse.messaging.zeromq
{
    public class LazyPirateServer : IRPCServer<TextRequest, TextResponse, TextProgressInfo>
    {
        public LazyPirateServer(MessageHandlerUri address, MessageHandlerUri statusUri, string topic)
        {
            Address = address;
            Type = HandlerProtocol.Reply;
            Topic = topic;
            StatusUri = statusUri;
        }

        public void Dispose() { }

        public MessageHandlerUri Address { get; }
        public HandlerProtocol Type { get; }
        public string Topic { get; }
        public MessageHandlerUri StatusUri { get; }

        public void Respond(Func<TextRequest, TextResponse> callback)
        {
            var server = new ResponseSocket();

            server.Bind(Address.OriginalString);
            while (true)
            {
                var bRequest = server.ReceiveFrameBytes();
                var sRequest = Encoding.Unicode.GetString(bRequest);

                var sReturn = callback(new TextRequest(sRequest));
                var bReturn = Encoding.Unicode.GetBytes(sReturn.Text);
                server.SendFrame(bReturn);
            }
        }

        public void PublishProgress(TextProgressInfo progressInfo)
        {
            Trace.WriteLine($"PublishProgress: {progressInfo}");
            using (var socket = new PublisherSocket())
            {
                socket.Options.SendHighWatermark = 1000;
                socket.Bind(StatusUri.OriginalString);
                socket.SendMoreFrame(Topic).SendFrame(progressInfo.Text);
            }
        }
    }
}
