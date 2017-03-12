using System;
using System.Diagnostics;

namespace lhotse.common
{
    public static class TraceExtensions
    {
        public static void WriteInfoLine(string message)
        {
            Trace.WriteLine($"[{DateTime.Now}][INFO]{message}");
        }
    }
}
