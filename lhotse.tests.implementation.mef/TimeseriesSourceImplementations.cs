using System;
using lhotse.common;

namespace lhotse.tests.implementation.mef
{
    public class DroppedFileTimeseriesSource : ITimeseriesSource
    {
        public string SchemeType => "dropfile";
        public Uri SourceUrl { get; }
        public bool AutoCreate { get; set; }
        public bool TryCreate(Uri sourceUri, out ITimeseriesSource source)
        {
            throw new NotImplementedException();
        }
    }
}
