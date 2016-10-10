using System;
using System.Collections;
using System.Collections.Generic;
using lhotse.common;

namespace lhotse.data.primitives
{
    public class TimeseriesSources : IReadOnlyDictionary<Uri, ITimeseriesSource>
    {
        #region Initialization
        public static TimeseriesSources Factory = new TimeseriesSources();
        private readonly IDictionary<Uri, ITimeseriesSource> _sourceDictionary;

        private TimeseriesSources()
        {
            _sourceDictionary = new Dictionary<Uri, ITimeseriesSource>();
        }

        private static ITimeseriesSource CreateSource(Uri keyUri)
        {
            switch (keyUri.Scheme)
            {
                case "null":
                    return new NullTimeseriesSource(keyUri);
                default:
                    throw new UnknownTimeseriesSourceException();
            }
        }
        #endregion

        #region IReadOnlyDictionary implementation
        public IEnumerator<KeyValuePair<Uri, ITimeseriesSource>> GetEnumerator()
        {
            return _sourceDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_sourceDictionary).GetEnumerator();
        }

        public int Count => _sourceDictionary.Count;

        public bool ContainsKey(Uri key)
        {
            return _sourceDictionary.ContainsKey(key);
        }

        public bool TryGetValue(Uri key, out ITimeseriesSource value)
        {
            return _sourceDictionary.TryGetValue(key, out value);
        }

        public ITimeseriesSource this[Uri keyUri]
        {
            get
            {
                if (!_sourceDictionary.ContainsKey(keyUri))
                    _sourceDictionary.Add(keyUri, CreateSource(keyUri));
                return _sourceDictionary[keyUri];
            }
        }

        public IEnumerable<Uri> Keys => _sourceDictionary.Keys;
        public IEnumerable<ITimeseriesSource> Values => _sourceDictionary.Values;
        #endregion
    }
}
