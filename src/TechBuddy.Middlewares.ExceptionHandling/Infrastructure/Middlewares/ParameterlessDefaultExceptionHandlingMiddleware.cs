namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure.Middlewares
{
    internal class ParameterlessDefaultExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ParameterlessDefaultExceptionHandlingMiddleware(RequestDelegate Next)
        {
            next = Next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ParameterlessDefaultExceptionHandlerAction.Handle(context, ex);
            }
        }
    }
}
