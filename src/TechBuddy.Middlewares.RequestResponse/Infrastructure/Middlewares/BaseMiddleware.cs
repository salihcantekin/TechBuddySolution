using System.Text.Json;
using System.Web;

namespace TechBuddy.Middlewares.RequestResponse.Infrastructure.Middlewares
{
    public abstract class BaseMiddleware
    {
        private readonly ILogWriter logWriter;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public BaseMiddleware(ILogWriter logWriter)
        {
            this.logWriter = logWriter is NullLogWriter ? null : logWriter;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task<RequestResponseContext> InvokeMiddleware(RequestDelegate next, HttpContext httpContext)
        {
            string requestText = await GetRequestBody(httpContext);

            var originalBodyStream = httpContext.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            httpContext.Response.Body = responseBody;

            Stopwatch sw = Stopwatch.StartNew();
            await next.Invoke(httpContext); // let the request go trough the system and response to be created
            sw.Stop();

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            await responseBody.CopyToAsync(originalBodyStream);

            var reqResContext = new RequestResponseContext(httpContext)
            {
                RequestBody = requestText,
                ResponseBody = responseText,
                ResponseCreationTime = TimeSpan.FromTicks(sw.ElapsedTicks)
            };
            
            logWriter?.Write(reqResContext);

            return reqResContext;
        }


        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);

            } while (readChunkLength > 0);

            return textWriter.ToString();
        }

        private async Task<string> GetRequestBody(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            string reqBody = ReadStreamInChunks(requestStream);

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            return reqBody;
        }
    }
}
