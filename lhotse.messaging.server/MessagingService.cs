using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Diagnostics;

namespace lhotse.messaging.server
{
    public enum ServiceState
    {
        ServiceStopped = 0x00000001,
        ServiceStartPending = 0x00000002,
        ServiceStopPending = 0x00000003,
        ServiceRunning = 0x00000004,
        ServiceContinuePending = 0x00000005,
        ServicePausePending = 0x00000006,
        ServicePaused = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public long dwServiceType;
        public ServiceState dwCurrentState;
        public long dwControlsAccepted;
        public long dwWin32ExitCode;
        public long dwServiceSpecificExitCode;
        public long dwCheckPoint;
        public long dwWaitHint;
    };

    public partial class MessagingService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private IRPCServer<TextRequest, TextResponse, TextProgressInfo> _handler;

        public MessagingService()
        {
            InitializeComponent();
        }

        private CancellationTokenSource _ctSource;
        private CancellationToken _ct;

        [Log]
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            var serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.ServiceStartPending,
                dwWaitHint = 100000
            };
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // Run the messaging server
            _handler = Program.Factory.Server;

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.ServiceRunning;
            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // Initialize the background task
            _ctSource = new CancellationTokenSource();
            _ct = _ctSource.Token;
            Task.Factory.StartNew(RunServer, _ct);
        }

        private void RunServer()
        {
            try
            {
                _handler.Respond(ProcessRequest);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Exception: {ex.Message}");
                _handler.Dispose();
            }
        }

        [Log]
        protected override void OnStop()
        {
            _ctSource.Cancel();
        }

        private TextResponse ProcessRequest(TextRequest request)
        {
            _handler.PublishProgress(new TextProgressInfo("Entering: ProcessRequest"));

            SimulateLongRunningTask();

            var rev = request.Text.ToArray().Reverse().ToArray();
            var revstr = new string(rev);

            var datetimestamp = DateTime.Now.ToString();
            var returnString = $"RunServer: [{datetimestamp}]: {revstr}";
            Trace.WriteLine(returnString);
            _handler.PublishProgress(new TextProgressInfo("Leaving: ProcessRequest"));
            return new TextResponse(revstr);
        }

        private void SimulateLongRunningTask()
        {
            for (var i = 0; i < 10; i++)
            {
                var datetimestamp = DateTime.Now.ToString();
                var progressInfo = $"SimulateLongRunningTask: [{datetimestamp}]: {i * 10} %";

                Thread.Sleep(1000);
                _handler.PublishProgress(new TextProgressInfo(progressInfo));
            }
        }
    }
}