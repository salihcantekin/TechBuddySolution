using Microsoft.AspNetCore.Builder;
using TechBuddy.Middlewares.RequestResponse.Infrastructure.Middlewares;

namespace TechBuddy.Middlewares.RequestResponse
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddTBRequestResponseMiddleware(this IApplicationBuilder appBuilder,
                                                                           Action<RequestResponseOptions> options)
        {
            var opt = new RequestResponseOptions();
            options(opt);


            ILogWriter logWriter = opt.LoggerFactory is null 
                                    ? new NullLogWriter() 
                                    : new LoggerFactoryLogWriter(opt.LoggerFactory, opt.LogginOptions);

            if (opt.HandlerUsing)
                appBuilder.UseMiddleware<DefaultRequestResponseWithHandlerMiddleware>(opt.ReqResHandler, logWriter);
            else
                appBuilder.UseMiddleware<DefaultRequestResponseMiddleware>(logWriter);

            return appBuilder;
        }
    }
}
