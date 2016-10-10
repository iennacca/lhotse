using System.Collections.Generic;
using lhotse.common;

namespace lhotse.data.primitives
{
    public class NullTimeseries : ITimeseries
    {
        public NullTimeseries(ITimeseriesSource source, string name)
        {
            Source = source;
            Name = name;
        }

        public string Name { get; }
        public ITimeseriesSource Source { get; }
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
        public IEnumerable<double> Observations { get; } = new List<double>();
    }
}
