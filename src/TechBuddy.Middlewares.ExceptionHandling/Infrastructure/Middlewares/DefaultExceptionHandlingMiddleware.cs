namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure.Middlewares
{
    internal class DefaultExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly MiddlewareOptions options;

        public DefaultExceptionHandlingMiddleware(RequestDelegate Next, MiddlewareOptions options = null)
        {
            next = Next;
            this.options = options;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                if (options.ExceptionTypeList.Any() && !options.ExceptionTypeList.Contains(ex.GetType()))
                    throw;

                if (options.ExceptionHandlerAction != null)
                {
                    await options.ExceptionHandlerAction.Invoke(httpContext, ex);
                    return;
                }

                await DefaultExceptionHandlerAction.Handle(httpContext, ex, options);
            }
        }
    }
}
