using CacheManager.Core;
using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.DateTimes;
using MetalCore.CQS.Mapper;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Query;
using MetalCore.CQS.Repository;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System.Reflection;
using MetalCore.Chores.Domain;
using MetalCore.Chores.Domain.Data;
using Microsoft.Extensions.DependencyInjection;
using MetalCore.Chores.UI.Common;

namespace MetalCore.Chores.UI.Setup
{
    public static class IoCSetup
    {
        internal static SimpleInjector.Container Container { get; private set; }

        public static SimpleInjector.Container SetupIoC(IServiceCollection microsoftServices)
        {
            Container = new();
            var container = Container;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //   container.RegisterSingleton<IUserContext>(() => new MyUserContext { Language = "EN", UserName = "jane.doe" });

            container.RegisterSingleton<IDateTimeProvider, DateTimeProvider>();
            container.Register<ISqlLiteDatabase, SqlLiteDatabase>(Lifestyle.Singleton);

            container.RegisterSingleton<IPageResolver, PageResolver>();
            container.RegisterSingleton<INavigationService, NavigationService>();
            microsoftServices.AddSingleton<INavigationService>(sp => container.GetService<INavigationService>());
            //     container.RegisterSingleton<IResponseMediator, ResponseMediator>();
            container.RegisterSingleton<ICqsMediator>(() => new CqsMediator(type => container.GetInstance(type)));
            container.RegisterSingleton<IRepositoryMediator>(() => new RepositoryMediator(type => container.GetInstance(type)));
            container.RegisterSingleton<IMapperMediator>(() => new MapperMediator(type => container.GetInstance(type)));

          //  container.RegisterSingleton<IPublisher>(() => new Publisher(type => container.GetAllInstances(type).Cast<dynamic>().ToList()));

           // container.Register<IQueryCacheRegion, MyQueryCacheRegion>(Lifestyle.Scoped);
            container.RegisterSingleton(typeof(ICacheManager<object>),
                () => CacheFactory.Build<object>(config => config.WithMicrosoftMemoryCacheHandle(true)));

            Assembly[] assembliesToScan = GetAssembliesToScan();

            RegisterViewsAndViewModels(microsoftServices, container, assembliesToScan);
            RegisterMappers(container, assembliesToScan);
            RegisterQueries(container, assembliesToScan);
            ReigsterCommands(container, assembliesToScan);
            ReigsterCommandQueries(container, assembliesToScan);
            RegisterRepositories(container, assembliesToScan);

           // container.Collection.Register(typeof(ISubscriber<>), assembliesToScan);

            return container;
        }

        private static void RegisterViewsAndViewModels(IServiceCollection microsoftServices, Container container, Assembly[] assemblies)
        {
            var typeIPageViewModel = typeof(IPageViewModel);
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsInterface || type.IsAbstract)
                        continue;

                    if (type.IsAssignableTo(typeof(IViewModel)))
                    {
                        container.Register(type);
                        continue;
                    }

                    if (!type.IsSubclassOf(typeof(Page)))
                        continue;

                    container.Register(type);
                    microsoftServices.AddTransient(type, sp => container.GetInstance(type));
                    foreach (var item in type.GetInterfaces())
                    {
                        if (item == typeIPageViewModel || !item.IsAssignableTo(typeIPageViewModel))
                            continue;

                        container.Register(item, type);
                        microsoftServices.AddTransient(item, sp => container.GetInstance(type));
                    }
                }
            }
        }

        private static void RegisterMappers(SimpleInjector.Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(IMapper<,>), assemblies);
        }

        private static void RegisterQueries(SimpleInjector.Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(IQueryHandler<,>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(IQueryLogger<,>), assemblies);
            container.Collection.Register(typeof(IQueryPermission<,>), assemblies);

            // Order matters - First one declared is last one run
           // container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerCacheDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerPermissionDecorator<,>));
           // container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerTimingDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(QueryHandlerLoggerDecorator<,>));
           // container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(MyQueryHandlerExceptionDecorator<,>));
        }

        private static void ReigsterCommands(SimpleInjector.Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(ICommandHandler<>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(ICommandLogger<>), assemblies);
            container.Collection.Register(typeof(ICommandCacheInvalidation<>), assemblies);
            container.Collection.Register(typeof(ICommandValidator<>), assemblies);
            container.Collection.Register(typeof(ICommandPermission<>), assemblies);

            // Order matters - First one declared is last one run
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerRetryDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerCacheInvalidationDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerValidatorDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerPermissionDecorator<>));
           // container.RegisterDecorator(typeof(ICommandHandler<>), typeof(MyCommandHandlerTimingDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(CommandHandlerLoggerDecorator<>));
           // container.RegisterDecorator(typeof(ICommandHandler<>), typeof(MyCommandHandlerExceptionDecorator<>));
        }

        private static void ReigsterCommandQueries(SimpleInjector.Container container, Assembly[] assemblies)
        {
            //NOTE: Only need to register this once and all concrete classes will be auto inclded now and 
            //      in the future.
            container.Register(typeof(ICommandQueryHandler<,>), assemblies, Lifestyle.Scoped);

            // Register interfaces that can be injected into other decorators
            container.Collection.Register(typeof(ICommandQueryLogger<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryCacheInvalidation<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryValidator<,>), assemblies);
            container.Collection.Register(typeof(ICommandQueryPermission<,>), assemblies);

            // Order matters - First one declared is last one run
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerRetryDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerCacheInvalidationDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerValidatorDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerPermissionDecorator<,>));
           // container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(MyCommandQueryHandlerTimingDecorator<,>));
            container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(CommandQueryHandlerLoggerDecorator<,>));
           // container.RegisterDecorator(typeof(ICommandQueryHandler<,>), typeof(MyCommandQueryHandlerExceptionDecorator<,>));
        }

        private static void RegisterRepositories(SimpleInjector.Container container, Assembly[] assemblies)
        {
            container.Register(typeof(IRepository<>), assemblies);
            container.Register(typeof(IRepository<,>), assemblies);
            container.Register(typeof(IValidationRepository<>), assemblies);
        }

        private static Assembly[] GetAssembliesToScan() =>
            new[]
            {
                typeof(IoCSetup).GetTypeInfo().Assembly,
                typeof(IoCReference).GetTypeInfo().Assembly
            };
    }
}
