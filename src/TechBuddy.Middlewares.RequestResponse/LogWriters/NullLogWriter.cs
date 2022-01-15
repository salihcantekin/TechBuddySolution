namespace TechBuddy.Middlewares.RequestResponse.LogWriters
{
    internal class NullLogWriter : ILogWriter
    {
        public IMessageCreator MessageCreator { get; }

        public Task Write(RequestResponseContext requestResponseContext)
        {
            return Task.CompletedTask;
        }
    }
}
