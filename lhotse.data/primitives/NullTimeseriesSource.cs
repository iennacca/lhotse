using System;
using lhotse.common;

namespace lhotse.data.primitives
{
    public class NullTimeseriesSource : ITimeseriesSource
    {
        public NullTimeseriesSource(Uri keyUri)
        {
            SourceUrl = keyUri;
        }

        public string SchemeType => "null";
        public Uri SourceUrl { get; }
        public bool AutoCreate { get; set; }
        public bool TryCreate(Uri sourceUri, out ITimeseriesSource source)
        {
            throw new NotImplementedException();
        }

        public ITimeseries Create(string name)
        {
            return new NullTimeseries(this, name);
        }
    }
}
