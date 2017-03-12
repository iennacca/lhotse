using System;
using System.Reflection;

namespace lhotse.messaging
{
    public enum HandlerProtocol
    {
        Request, Reply
    }

    public class MessageHandlerUri
    {
        public MessageHandlerUri(string address)
        {
            OriginalString = address;
        }

        public string OriginalString { get; }
    }

    public interface IMessageHandler<TRequest, TResponse, TProgressInfo> : IDisposable
    {
        MessageHandlerUri Address { get; }
        HandlerProtocol Type { get; }
        TResponse Request(TRequest request);
        void Respond(Func<TRequest, TResponse> callback);
        void PublishProgress(TProgressInfo progressInfo);
        void SubscribeProgress(Action<TProgressInfo> callback);
    }

    public interface IRPCServer<out TRequest, in TResponse, in TProgressInfo> : IDisposable
    {
        MessageHandlerUri Address { get; }
        HandlerProtocol Type { get; }
        void Respond(Func<TRequest, TResponse> callback);
        void PublishProgress(TProgressInfo progressInfo);
    }

    public interface IRPCClient<in TRequest, out TResponse, out TProgressInfo> : IDisposable
    {
        MessageHandlerUri Address { get; }
        HandlerProtocol Type { get; }
        TResponse Request(TRequest request);
        void SubscribeProgress(Action<TProgressInfo> callback);
    }

    public interface IMessageHandlerFactory<TRequest, TResponse, TProgressInfo>
    {
        string HandlerName { get; }
        Version HandlerVersion{ get; }
        IRPCServer<TRequest, TResponse, TProgressInfo> Server { get; }
        IRPCClient<TRequest, TResponse, TProgressInfo> Client { get; }
    }

    public class TextRequest
    {
        public readonly string Text;

        public TextRequest(string text)
        {
            Text = text;
        }
    }

    public class TextResponse
    {
        public readonly string Text;

        public TextResponse(string text)
        {
            Text = text;
        }
    }

    public class TextProgressInfo
    {
        public readonly string Text;

        public TextProgressInfo(string text)
        {
            Text = text;
        }
    }
}