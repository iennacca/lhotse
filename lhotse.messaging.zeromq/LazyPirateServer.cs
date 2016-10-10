using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace lhotse.messaging.zeromq
{
    public class LazyPirateServer : IRPCServer<TextRequest, TextResponse, TextProgressInfo>
    {
        public LazyPirateServer(MessageHandlerUri address)
        {
            Address = address;
            Type = HandlerProtocol.Reply;
        }

        public void Dispose() { }

        public MessageHandlerUri Address { get; }
        public HandlerProtocol Type { get; }

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
            throw new NotImplementedException();
        }
    }
}
