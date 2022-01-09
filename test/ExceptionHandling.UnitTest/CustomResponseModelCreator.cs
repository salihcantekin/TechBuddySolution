using TechBuddy.Middlewares.ExceptionHandling;
using TechBuddy.Middlewares.ExceptionHandling.Infrastructure;

namespace ExceptionHandling.UnitTest
{
    internal partial class ExceptionHandlingTests
    {
        internal class CustomResponseModelCreator : IResponseModelCreator
        {
            public object CreateModel(ModelCreatorContext model)
            {
                return new
                { 
                    ErrorMessage = model.ErrorMessage,
                    StatusCode = model.HttpStatusCode
                };
            }
        }
    }
}
