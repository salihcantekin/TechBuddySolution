namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure.DefaultActions
{
    internal class ParameterlessDefaultExceptionHandlerAction : BaseHandlerAction
    {
        internal static Task Handle(HttpContext context, Exception ex)
        {
            return Handle(context,
                          ex,
                          new DefaultResponseModelCreator(),
                          LibraryConstants.DefaultStatusCode,
                          LibraryConstants.DefaultErrorMessage,
                          LibraryConstants.ContentType,
                          false);
        }
    }
}
