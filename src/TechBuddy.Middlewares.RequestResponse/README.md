# RequestResponse Middleware

[![NuGet](https://img.shields.io/nuget/v/TechBuddy.Middlewares.RequestResponse)](https://www.nuget.org/packages/TechBuddy.Middlewares.RequestResponse/)


## Description

This project represents a build-in Request-Response handling and logging middleware as an extension method for `IApplicationBuilder`
It adds middleware for managing and handling Request-Response details in your web api project. It is also configurable by passing options whilst initializing.

### Dependencies

* [Microsoft.AspNetCore.Http](https://www.nuget.org/packages/Microsoft.AspNetCore.Http/)
* [Microsoft.AspNetCore.Http.Extensions](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Extensions/)
* [Microsoft.Extensions.Logging.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Logging.Abstractions/)
* [Microsoft.IO.RecyclableMemoryStream](https://www.nuget.org/packages/Microsoft.IO.RecyclableMemoryStream/)

## Getting Started

This library help you to get details of request and response by adding a Middleware on your system. When the request end, it calls a handler method by passing a context model which held the required properties you might need.

This extension method can easily be called in `Configure` method in your startup.cs file of web api projects.

```csharp
app.AddTBRequestResponseMiddleware(opt => 
{
    opt.UseHandler(async context => 
    {
        // When request and response are created and you handle
        var fullRequestUrl = context.Url;
        var reqBody = context.RequestBody;
        var reqLength = context.RequestLength;

        var resBody = context.RequestBody;
        var resLength = context.ResponseLength;

        var responseTimeSpan = context.ResponseCreationTime;
        var responseFormattedTimeSpan = context.ResponseTimeStr;
        
        //Console.Write(System.Text.Json.JsonSerializer.Serialize(context));
    });
});
```

By using `UseHandler` method, you can easily access the properties in the context. This method is fired just before the request is completed by the middleware which collects all the information to serve you. There are several properties you might need in this context. You may use this data to do whatever you want to. If you just want to log all the data, you can simply use `UseLogger` method. 
By using this way, you basically tell the middleware that it can use the logging features that your system already have.

Let's say you use console logging. That would mean, your system will send all the produced logs to the console. In this case, when you use `UseLogger` and give it a default log provider of your system, the request and response logs would directly goes to your console. You are also able to add more than one built-in place to write the logs to such as `Debug`, `EventLog` or even custom logger libraries such as `Serilog`.

``` csharp
services.AddLogging(configure => 
{
    configure.AddConsole();
    configure.AddDebug();
    configure.AddEventLog();
});
```

```csharp
opt.UseLogger(app.ApplicationServices.GetRequiredService<ILoggerFactory>(), opt => 
{
    opt.LoggerCategoryName = "MyCustomLoggerCategoryName"; // Optional. Default Value: RequestResponseLogger
    opt.LoggingLevel = LogLevel.Information; // Optional. Default Value: LogLevel.Information
    opt.UseSeparateContext = true;

    // Required fields. At least 1
    opt.LoggingFields.Add(LogFields.Path);
    opt.LoggingFields.Add(LogFields.RequestLength);
    opt.LoggingFields.Add(LogFields.ResponseLength);
    opt.LoggingFields.Add(LogFields.ResponseTiming);
});
```

Once you choose to use your loggerfactory to write the logs to, you might want to customize the log output. If you use a logging system that supports Context Feature as Serilog does, you want to set `UseSeparateContext` property to true. This will send the logs through the LoggerFactory by using Context. [See an example for Serilog](https://benfoster.io/blog/serilog-best-practices/#log-context)

On the other hand, you can also customize the output properties by adding what information you want to see on the output. To do that, you can add the properties you want to the `LoggingFields` list. `LogingLevel` property is used to send the logs to the providers by using this level. Let's say you customized your Console Logging by only showing warning and error message by setting the LogLevel to 'Warning'. In this case, your request and response logs wouldn't appear on the console unless you set the `LoggingLevel` to LogLevel.Warning. So you are able to customize the log level that middleware uses while sending the logs to the providers. The `LoggerCategoryName` is used to create the logger by giving it a name. 

PS: When you use both `UseHandler` and `UseLogger`, the calling order is the Logger is ran first and then the Handler is fired.

