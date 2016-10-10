using System;
using System.Collections.Generic;

namespace lhotse.common
{
    using ITimeseriesName = String;

    public interface ITimeseriesSource
    {
        string SchemeType { get; }
        Uri SourceUrl { get; }
        bool AutoCreate { get; set; }
        bool TryCreate(Uri sourceUri, out ITimeseriesSource source);
    }

    public interface IGenericTimeseries<TKey, TValue, out TObservation>
    {
        ITimeseriesName Name { get; }
        ITimeseriesSource Source { get; }
        Dictionary<TKey, TValue> Properties { get; }
        IEnumerable<TObservation> Observations { get; }
    }

    public interface ITimeseries: IGenericTimeseries<string, string, double> { }
}
