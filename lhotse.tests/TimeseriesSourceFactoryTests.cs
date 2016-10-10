using System;
using lhotse.common;
using lhotse.data.primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lhotse.tests
{
    [TestClass]
    public class TimeseriesSourceFactoryTests
    {
        [TestMethod]
        public void TimeseriesSourceFactoryIsASingleton()
        {
            var sources1 = TimeseriesSources.Factory;
            var sources2 = TimeseriesSources.Factory;
            Assert.IsTrue(ReferenceEquals(sources1, sources2));
        }

        [TestMethod]
        public void CanCreateNullTimeseriesSource()
        {
            var sourceUri = new Uri("null://testinstance.testdb");
            var count = TimeseriesSources.Factory.Count;

            var source = TimeseriesSources.Factory[sourceUri];
            Assert.IsNotNull(source);
            Assert.IsTrue(source.SourceUrl.ToString() == sourceUri.ToString());
            Assert.IsTrue(source is NullTimeseriesSource);
            Assert.IsTrue(TimeseriesSources.Factory.Count == (count + 1));
        }

        [TestMethod]
        public void CanCheckTimeseriesSourceType()
        {
            var sourceUri = new Uri("null://testinstance.testdb");
            var source = TimeseriesSources.Factory[sourceUri];
            Assert.IsTrue(source is NullTimeseriesSource);
            Assert.IsTrue(TimeseriesSources.Factory.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(UnknownTimeseriesSourceException))]
        public void ThrowsExceptionOnInvalidScheme()
        {
            var sourceUri = new Uri("meh://testinstance.testdb");
            var source = TimeseriesSources.Factory[sourceUri];

            Assert.IsTrue(TimeseriesSources.Factory.Count == 0);
            Assert.IsNotNull(source);
        }
    }
}
