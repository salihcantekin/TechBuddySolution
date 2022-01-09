namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure.DefaultActions
{
    internal class DefaultExceptionHandlerAction : BaseHandlerAction
    {
        internal static Task Handle(HttpContext context,
                                                    Exception ex,
                                                    MiddlewareOptions opt)
        {
            return Handle(context,
                          ex,
                          opt.GetResponseModelCreator(),
                          opt.DefaultHttpStatusCode.Value,
                          opt.DefaultMessage,
                          opt.ContentType,
                          opt.IsDevelopment);
        }
    }


}
