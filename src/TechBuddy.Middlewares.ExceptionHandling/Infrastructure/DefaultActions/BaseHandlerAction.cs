namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure.DefaultActions
{
    internal abstract class BaseHandlerAction
    {
        internal static Task Handle(HttpContext context, Exception ex,
            IResponseModelCreator responseModelCreator,
            HttpStatusCode statusCode,
            string errorMessage,
            string httpContentType,
            bool useExceptionDetails)
        {
            if (ex is UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }

            var modelModel = new ModelCreatorContext()
            {
                Exception = ex,
                HttpStatusCode = statusCode,
                ErrorMessage = useExceptionDetails
                                    ? ex.ToString()
                                    : errorMessage
            };

            var model = responseModelCreator.CreateModel(modelModel);

            var result = JsonSerializer.Serialize(model);

            context.Response.ContentType = httpContentType;
            
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }
}
