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
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddMediatR(new[] { Assembly.GetExecutingAssembly() }, configuration: c => { c.AsTransient(); });

            //services.AddMediatR(new[] { Assembly.GetExecutingAssembly() }, configuration: c => { c.AsTransient(); });
            //services.AddMediatR(config => config.AsSingleton(), new[] { Assembly.GetExecutingAssembly() });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

            return services;
        }
    }
}