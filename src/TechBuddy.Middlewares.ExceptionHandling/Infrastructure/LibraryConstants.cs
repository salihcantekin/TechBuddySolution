namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure
{
    internal class LibraryConstants
    {
        internal const string ContentType = "application/json";

        internal const string DefaultErrorMessage = "Internal Server Error Occured!";

        internal const HttpStatusCode DefaultStatusCode = HttpStatusCode.InternalServerError;
    }
}
