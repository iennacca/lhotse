using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using lhotse.common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace lhotse.tests.meftests
{
    /// <summary>
    /// Summary description for MEFTests
    /// </summary>
    [TestClass]
    public class MEFTests
    {
        private readonly CompositionContainer _container;
        private const string ModuleSearchPattern = "lhotse.tests*.dll";

        public MEFTests()
        {
            _container = CreateMEFContainer();
        }

        private static CompositionContainer CreateMEFContainer()
        {
            var registrationBuilder = new RegistrationBuilder();

            registrationBuilder
                .ForTypesDerivedFrom<ITimeseriesSource>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .ExportInterfaces(x => x.IsPublic);

            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(
                new AssemblyCatalog(Assembly.GetExecutingAssembly(), registrationBuilder));

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            aggregateCatalog.Catalogs.Add(new DirectoryCatalog(baseDirectory, ModuleSearchPattern, registrationBuilder));

            return new CompositionContainer(aggregateCatalog,
                                                  CompositionOptions.DisableSilentRejection |
                                                  CompositionOptions.IsThreadSafe);
        }

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
        public void CanResolveADroppedFileTimeseriesSource()
        {
            var sources = _container.GetExports<ITimeseriesSource>();
            foreach (var s in sources)
                Debug.WriteLine(s.Value.SchemeType);
            Assert.IsNotNull(sources);
        }
    }
}
