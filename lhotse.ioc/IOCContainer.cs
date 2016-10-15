using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Reflection;

using lhotse.messaging;

namespace lhotse.ioc
{
    public static class IOCContainer
    {
        public static readonly string ModuleSearchPattern = "lhotse.messaging*.dll";
        private static readonly CompositionContainer Container = Create();

        private static CompositionContainer Create()
        {
            var registrationBuilder = new RegistrationBuilder();

            registrationBuilder
                .ForTypesDerivedFrom<IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>>()
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

        public static Lazy<IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>> Factory = 
            Container.GetExport<IMessageHandlerFactory<TextRequest, TextResponse, TextProgressInfo>>();
    }
}
