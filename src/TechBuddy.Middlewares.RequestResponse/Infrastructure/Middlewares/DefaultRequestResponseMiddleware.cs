namespace TechBuddy.Middlewares.RequestResponse.Infrastructure.Middlewares
{
    public class DefaultRequestResponseMiddleware: BaseMiddleware
    {
        private readonly RequestDelegate next;

        public DefaultRequestResponseMiddleware(RequestDelegate next, ILogWriter logWriter) : base(logWriter)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await InvokeMiddleware(next, httpContext);
        }
    }
}
