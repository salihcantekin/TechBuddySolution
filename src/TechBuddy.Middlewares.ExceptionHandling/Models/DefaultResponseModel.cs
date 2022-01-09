namespace TechBuddy.Middlewares.ExceptionHandling.Models
{
    internal class DefaultResponseModel
    {
        public HttpStatusCode HttpStatusCode { get; set; }

        public string ExceptionMessage { get; set; }
    }
}
