namespace TechBuddy.Middlewares.RequestResponse.Infrastructure.Interfaces
{
    internal interface ILogMessageWithContextCreator : IMessageCreator
    {
        ReadOnlyCollection<string> GetValues();
    }
}
