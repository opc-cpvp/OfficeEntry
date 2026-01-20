using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OfficeEntry.Application.Common.Behaviours;
using System.Reflection;

namespace OfficeEntry.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
                // config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
            });

            //services.AddMediatR(new[] { Assembly.GetExecutingAssembly() }, configuration: c => { c.AsTransient(); });
            //services.AddMediatR(new[] { Assembly.GetExecutingAssembly() }, configuration: c => { c.AsTransient(); });
            //services.AddMediatR(config => config.AsSingleton(), new[] { Assembly.GetExecutingAssembly() });

            return services;
        }
    }
}
