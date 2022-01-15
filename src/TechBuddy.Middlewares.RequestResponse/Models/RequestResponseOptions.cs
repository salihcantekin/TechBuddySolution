namespace TechBuddy.Middlewares.RequestResponse
{
    public class RequestResponseOptions
    {
        internal LoggingOptions LogginOptions;
        internal ILoggerFactory LoggerFactory;

        internal ILogWriter LogWriter { get; set; }

        internal Func<RequestResponseContext, Task> ReqResHandler { get; set; }

        internal bool LogWriterUsing => LogWriter is not null;

        internal bool HandlerUsing => ReqResHandler is not null;


        public void UseLogWriter(ILogWriter logWriter)
        {
            LogWriter = logWriter;
        }

        public void UseHandler(Func<RequestResponseContext, Task> Handler)
        {
            ReqResHandler = Handler;
        }

        public void UseLogger(ILoggerFactory loggerFactory, Action<LoggingOptions> options)
        {
            LogginOptions = new LoggingOptions();
            options.Invoke(LogginOptions);

            this.LoggerFactory = loggerFactory;
        }
    }
}
