namespace TechBuddy.Middlewares.RequestResponse.LogWriters
{
    internal class LoggerFactoryLogWriter : ILogWriter
    {
        private readonly LoggingOptions options;
        private readonly ILogger logger;

        public LoggerFactoryLogWriter(ILoggerFactory loggerFactory,
                                      LoggingOptions options)
        {
            this.options = options;

            MessageCreator = options.UseSeparateContext
                                ? new LogMessageWithContextCreator(options)
                                : new LogMessageCreator(options);

            logger = loggerFactory.CreateLogger(options.LoggerCategoryName);
        }

        public IMessageCreator MessageCreator { get; }

        public Task Write(RequestResponseContext requestResponseContext)
        {
            string logStr = MessageCreator.Create(requestResponseContext);
            string[] parameters = null;

            if (MessageCreator is ILogMessageWithContextCreator logMessageWithContextCreator)
                parameters = logMessageWithContextCreator.GetValues().ToArray();

            logger.Log(options.LoggingLevel, logStr, parameters);
            
            return Task.CompletedTask;
        }
    }
}