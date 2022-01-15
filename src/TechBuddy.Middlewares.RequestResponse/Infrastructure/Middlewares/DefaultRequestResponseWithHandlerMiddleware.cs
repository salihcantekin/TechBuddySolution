namespace TechBuddy.Middlewares.RequestResponse.Infrastructure.Middlewares
{
    internal class DefaultRequestResponseWithHandlerMiddleware : BaseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Func<RequestResponseContext, Task> reqResHandler;

        public DefaultRequestResponseWithHandlerMiddleware(RequestDelegate next,
                                                           Func<RequestResponseContext, Task> reqResHandler,
                                                           ILogWriter logWriter) : base(logWriter)
        {
            this.next = next;
            this.reqResHandler = reqResHandler;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var reqResContext = await InvokeMiddleware(next, httpContext);

            await reqResHandler.Invoke(reqResContext);
        }
    }
}
