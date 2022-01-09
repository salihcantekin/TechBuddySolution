namespace TechBuddy.Middlewares.ExceptionHandling.Infrastructure
{
    public interface IResponseModelCreator : IResponeModelCreatorMetadata
    {
        object CreateModel(ModelCreatorContext model);
    }

    internal class DefaultResponseModelCreator : IResponseModelCreator
    {
        public object CreateModel(ModelCreatorContext model)
        {
            return new DefaultResponseModel()
            {
                ExceptionMessage = model.ErrorMessage,
                HttpStatusCode = model.HttpStatusCode
            };
        }
    }
}
