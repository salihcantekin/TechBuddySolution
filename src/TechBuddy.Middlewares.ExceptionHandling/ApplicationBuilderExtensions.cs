namespace TechBuddy.Middlewares.ExceptionHandling
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder AddTBExceptionHandlingMiddleware(this IApplicationBuilder appBuilder,
                                                                           Action<MiddlewareOptions> options)
        {
            var opt = new MiddlewareOptions();
            options(opt);
            appBuilder.UseMiddleware<DefaultExceptionHandlingMiddleware>(opt);

            return appBuilder;
        }

        public static IApplicationBuilder AddTBExceptionHandlingMiddleware(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ParameterlessDefaultExceptionHandlingMiddleware>();

            return appBuilder;
        }
    }
}
