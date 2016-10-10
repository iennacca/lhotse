using System;
using lhotse.common;

namespace lhotse.tests.unitytests
{
    public class TestTimeseriesSource : ITimeseriesSource
    {
        public string SchemeType => "test";
        public Uri SourceUrl { get; }
        public bool AutoCreate { get; set; }
        public bool TryCreate(Uri sourceUri, out ITimeseriesSource source)
        {
            throw new NotImplementedException();
        }
    }

    public class AnotherTestTimeseriesSource : ITimeseriesSource
    {
        public string SchemeType => "anothertest";
        public Uri SourceUrl { get; }
        public bool AutoCreate { get; set; }
        public bool TryCreate(Uri sourceUri, out ITimeseriesSource source)
        {
            throw new NotImplementedException();
        }
    }
}
