using System;

namespace lhotse.common
{
    public class UnknownTimeseriesSourceException : Exception
    {
        public override string Message { get; } = "Unknown timeseries source.";
    }
}
