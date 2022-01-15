namespace TechBuddy.Middlewares.RequestResponse.Infrastructure.Interfaces
{
    public interface ILogWriter
    {
        IMessageCreator MessageCreator { get; }
        Task Write(RequestResponseContext requestResponseContext);
    }
}
