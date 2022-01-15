namespace TechBuddy.Middlewares.RequestResponse.MessageCreators
{
    public sealed class LogMessageCreator : BaseLogMessageCreator, ILogMessageCreator
    {
        private readonly LoggingOptions loggingOptions;

        public LogMessageCreator(LoggingOptions loggingOptions)
        {
            this.loggingOptions = loggingOptions;
        }

        public string Create(RequestResponseContext requestResponseContext)
        {
            var sb = new StringBuilder();

            foreach (var logField in loggingOptions.LoggingFields)
            {
                var generatedStr = GenerateLogStringByField(requestResponseContext, logField);
                sb.AppendFormat("{0}: {1}{2}", logField, generatedStr, Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}