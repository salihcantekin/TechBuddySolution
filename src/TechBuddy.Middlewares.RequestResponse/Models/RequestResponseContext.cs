using System.Text.Json.Serialization;

namespace TechBuddy.Middlewares.RequestResponse
{
    public class RequestResponseContext
    {
        internal readonly HttpContext Context;
        internal RequestResponseContext(HttpContext context)
        {
            Context = context;
        }

        public string RequestBody { get; internal set; }

        public string ResponseBody { get; internal set; }

        [JsonIgnore]
        public TimeSpan? ResponseCreationTime { get; internal set; }

        /// <summary>
        /// Gets total response duration. Format: mm:ss.fff
        /// </summary>
        public string ResponseTimeStr => ResponseCreationTime is null ? "" : string.Format("{0:mm\\:ss\\.fff}", ResponseCreationTime);


        public int? RequestLength => RequestBody?.Length == 0 ? null : RequestBody.Length;

        public int? ResponseLength => ResponseBody?.Length;


        private string url;
        public string Url => url ??= BuildUrl().ToString();

        public Uri BuildUrl()
        {
            var url = Context.Request.GetDisplayUrl();
            
            return new Uri(url, UriKind.RelativeOrAbsolute);
        }
    }
}
