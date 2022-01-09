namespace TechBuddy.Middlewares.ExceptionHandling
{
    public class ModelCreatorContext
    {
        public string ErrorMessage { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public Exception Exception { get; set; }
    }
}
