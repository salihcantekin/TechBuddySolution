namespace TechBuddy.Middlewares.RequestResponse.MessageCreators
{
    internal sealed class LogMessageWithContextCreator : BaseLogMessageCreator, ILogMessageCreator, ILogMessageWithContextCreator
    {
        private readonly LoggingOptions loggingOptions;
        private List<string> valueList;

        public LogMessageWithContextCreator(LoggingOptions loggingOptions)
        {
            this.loggingOptions = loggingOptions;

            valueList = loggingOptions?.LoggingFields?.Count > 0 ? new List<string>(loggingOptions.LoggingFields.Count) : new List<string>();
        }

        public string Create(RequestResponseContext requestResponseContext)
        {
            valueList.Clear();

            var sb = new StringBuilder();

            foreach (var logField in loggingOptions.LoggingFields)
            {
                var generatedStr = GenerateLogStringByField(requestResponseContext, logField);
                sb.AppendFormat("{0}: {1}{2}", logField, "{" + logField + "}", Environment.NewLine);
                valueList.Add(generatedStr);
            }

            return sb.ToString();
        }

        public ReadOnlyCollection<string> GetValues()
        {
            return valueList.AsReadOnly();
        }
    }
}
