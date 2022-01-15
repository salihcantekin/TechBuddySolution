namespace TechBuddy.Middlewares.RequestResponse
{
    public class LoggingOptions
    {
        private List<LogFields> loggingFields { get; set; }

        public LogLevel LoggingLevel { get; set; } = LogLevel.Information;

        public List<LogFields> LoggingFields 
        {
            get
            { 
                return loggingFields ??= new List<LogFields>(); // It will create new instance only when needed
            }

            set => loggingFields = value;
        }

        public bool UseSeparateContext { get; set; } = false;

        public string LoggerCategoryName { get; set; } = "RequestResponseLogger";
    }

    public enum LogFields
    { 
        Request = 1,
        Response = 2,
        HostName = 4,
        Path = 8,
        QueryString = 16,
        ResponseTiming = 32,
        RequestLength = 64,
        ResponseLength = 128
    }
}
