using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NNanomsg;
using NNanomsg.Protocols;

namespace lhotse.tests
{
    /// <summary>
    /// Summary description for MessageQueueTests
    /// </summary>
    [TestClass]
    public class MessageQueueTests
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        public static class MessageQueueConstants
        {
            public const string Address = "inproc://reqrep_test";
            public const string RequestPayload = "REQUESTPAYLOAD";
            public const int ReplyPayload = 777;
        }


#region NNPrimitives
        public static class NNPrimitivesRequestReplyProcesses
        {
            public static void Response(CancellationToken cancelToken)
            {
                var rep = NN.Socket(Domain.SP, Protocol.REP);
                NN.Bind(rep, MessageQueueConstants.Address);

                while (!cancelToken.IsCancellationRequested)
                {
                    byte[] payloadBytes;
                    NN.Recv(rep, out payloadBytes, SendRecvFlags.NONE);
                    Debug.WriteLine("Response: Received request");

                    var payload = Encoding.ASCII.GetString(payloadBytes);
                    Assert.IsTrue(payload.Equals(MessageQueueConstants.RequestPayload));

                    Debug.WriteLine("Response: Sending reply");
                    NN.Send(rep, BitConverter.GetBytes(MessageQueueConstants.ReplyPayload), SendRecvFlags.NONE);
                }
            }

            public static void Request()
            {
                var req = NN.Socket(Domain.SP, Protocol.REQ);
                NN.Connect(req, MessageQueueConstants.Address);

                Debug.WriteLine("Request: Sending request");
                NN.Send(req, Encoding.ASCII.GetBytes(MessageQueueConstants.RequestPayload), SendRecvFlags.NONE);

                Debug.WriteLine("Request: Receiving reply");
                byte[] replyPayload;
                NN.Recv(req, out replyPayload, SendRecvFlags.NONE);
                Assert.IsTrue(BitConverter.ToInt32(replyPayload, 0) == MessageQueueConstants.ReplyPayload);
            }
        }

        [TestMethod]
        public void CanCreateMessageQueueUsingNNPrimitives()
        {
            var cancelSource = new CancellationTokenSource();
            Debug.WriteLine("Executing ReqRep test");

            var responseThread = new Thread(() => NNPrimitivesRequestReplyProcesses.Response(cancelSource.Token));
            responseThread.Start();
            NNPrimitivesRequestReplyProcesses.Request();
            cancelSource.Cancel();
        }

        [TestMethod]
        public void CanDoSuccessiveRequestReplies()
        {
            var cancelSource = new CancellationTokenSource();
            Debug.WriteLine("Executing ReqRep test");

            var responseThread = new Thread(() => NNPrimitivesRequestReplyProcesses.Response(cancelSource.Token));
            responseThread.Start();
            Assert.IsTrue(responseThread.IsAlive);
            NNPrimitivesRequestReplyProcesses.Request();
            Debug.WriteLine(String.Empty);
            Assert.IsTrue(responseThread.IsAlive);
            NNPrimitivesRequestReplyProcesses.Request();
            cancelSource.Cancel();
        }
#endregion

#region NNClasses testing
        public static class NNClassRequestReplyProcesses
        {
            public static void Response(CancellationToken cancelToken)
            {
                using (var s = new ReplySocket())
                {
                    s.Bind(MessageQueueConstants.Address);

                    while (!cancelToken.IsCancellationRequested)
                    {
                        byte[] payloadBytes = s.Receive();
                        Debug.WriteLine("Response: Received request");

                        var payload = Encoding.UTF8.GetString(payloadBytes);
                        Assert.IsTrue(payload.Equals(MessageQueueConstants.RequestPayload));

                        Debug.WriteLine("Response: Sending reply");
                        s.Send(BitConverter.GetBytes(MessageQueueConstants.ReplyPayload));
                    }
                }
            }

            public static void Request()
            {
                using (var s = new RequestSocket())
                {
                    s.Connect(MessageQueueConstants.Address);

                    Debug.WriteLine("Request: Sending request");
                    s.Send(Encoding.UTF8.GetBytes(MessageQueueConstants.RequestPayload));

                    Debug.WriteLine("Request: Receiving reply");
                    var replyPayload = s.Receive();
                    Assert.IsTrue(BitConverter.ToInt32(replyPayload, 0) == MessageQueueConstants.ReplyPayload);
                }
            }
        }

        [TestMethod, Timeout(100)]
        public void CanCreateMessageQueueUsingNNClasses()
        {
            var cancelSource = new CancellationTokenSource();
            Debug.WriteLine("Executing ReqRep test");
            var serverThread = new Thread(() => NNClassRequestReplyProcesses.Response(cancelSource.Token));
            serverThread.Start();
            
            NNClassRequestReplyProcesses.Request();
            cancelSource.Cancel();
        }
#endregion

    }
}