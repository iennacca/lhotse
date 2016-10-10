using System;
using System.Linq;
using lhotse.common;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lhotse.tests.unitytests
{
    /// <summary>
    /// Summary description for UnityTests
    /// </summary>
    [TestClass]
    public class UnityTests
    {
        public UnityTests()
        {
            _container = new UnityContainer();
            _container.RegisterType<ITimeseriesSource, TestTimeseriesSource>("TestTimeseriesSource");
            _container.RegisterType<ITimeseriesSource, AnotherTestTimeseriesSource>("AnotherTestTimeseriesSource");
        }

        private readonly UnityContainer _container;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanResolveATimeseriesSourceViaNaming()
        {
            var source = _container.Resolve<ITimeseriesSource>("AnotherTestTimeseriesSource");
            Assert.IsTrue(source is AnotherTestTimeseriesSource);

            source = _container.Resolve<ITimeseriesSource>("TestTimeseriesSource");
            Assert.IsTrue(source is TestTimeseriesSource);
        }

        [TestMethod]
        public void CanIterateThroughTimeseriesSources()
        {
            var sources = _container.ResolveAll<ITimeseriesSource>().ToList();
            Assert.IsTrue(sources.Count() == 2);
            foreach (var source in sources)
                Console.WriteLine(source.GetType().ToString());
        }
    }
}
