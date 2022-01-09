namespace TechBuddy.Middlewares.ExceptionHandling
{
    public class MiddlewareOptions
    {
        internal IResponseModelCreator ResponseModelCreator;

        public MiddlewareOptions()
        {
            ExceptionTypeList = new ExceptionTypeList();
        }

        public Func<HttpContext, Exception, Task> ExceptionHandlerAction { get; set; }

        public bool IsDevelopment { get; set; }

        public ExceptionTypeList ExceptionTypeList { get; set; }

        public HttpStatusCode? DefaultHttpStatusCode { get; set; } = HttpStatusCode.InternalServerError;

        public string DefaultMessage { get; set; } = LibraryConstants.DefaultErrorMessage;

        public string ContentType { get; set; } = LibraryConstants.ContentType;

        public void UseResponseModelCreator<T>() where T : IResponseModelCreator
        {
            ResponseModelCreator = Activator.CreateInstance<T>();
        }

        internal IResponseModelCreator GetResponseModelCreator()
        {
            return ResponseModelCreator ?? new DefaultResponseModelCreator();
        }
    }
}
