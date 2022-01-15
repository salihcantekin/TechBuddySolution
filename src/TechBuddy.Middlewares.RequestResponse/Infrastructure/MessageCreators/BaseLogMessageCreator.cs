namespace TechBuddy.Middlewares.RequestResponse.MessageCreators
{
    public abstract class BaseLogMessageCreator
    {
        protected string GenerateLogStringByField(RequestResponseContext requestResponseContext, LogFields field)
        {
            return field switch
            {
                LogFields.Request => requestResponseContext.RequestBody,
                LogFields.Response => requestResponseContext.ResponseBody,
                LogFields.QueryString => requestResponseContext.Context?.Request?.QueryString.Value,
                LogFields.Path => requestResponseContext.Context?.Request?.Path.Value,
                LogFields.HostName => requestResponseContext.Context?.Request?.Host.Value,
                LogFields.ResponseTiming => requestResponseContext.ResponseTimeStr,
                LogFields.RequestLength => requestResponseContext.RequestLength.ToString(),
                LogFields.ResponseLength => requestResponseContext.ResponseLength.ToString(),
                _ => string.Empty
            };
        }
    }
}