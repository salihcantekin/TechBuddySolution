# ExceptionHandling Middleware

[![NuGet](https://img.shields.io/nuget/v/TechBuddy.Middlewares.ExceptionHandling)](https://www.nuget.org/packages/TechBuddy.Middlewares.ExceptionHandling/)

## Description

This project represents a build-in ExceptionHandling Middleware as an extension method for `IApplicationBuilder`
It adds middleware for managing and handling unhandled exceptions in your web api project. It is also configurable by passing options whilst initializing.

### Dependencies

* As it will serve infrastructure for web apis, it requires [Microsoft.AspNetCore.Http.Abstractions](https://www.nuget.org/packages/Microsoft.AspNetCore.Http.Abstractions/)  package
* This project has been developing in NET5

## Getting Started


It can be used parameterless to handle your exceptions and returns a specific response model which includes `HttpStatusCode` and `ExceptionMessage`. `ExceptionMessage` will only contain simple exception message unless it is configured. In this case, the default `HttpStatusCode` is InternalServerError(500) whereas the default `ExceptionMessage` is "Internal Server Error Occured!"
This extension method can easily be called in `Configure` method in your startup.cs file of web api projects.

```csharp
app.AddTBExceptionHandlingMiddleware();
```

```csharp
class DefaultResponseModel
{
    public HttpStatusCode HttpStatusCode { get; set; }

    public string ExceptionMessage { get; set; }
}
```

It can also be used with `MiddlewareOptions` parameters which allows you to configure your exception handling ability.

ExceptionMessage will be detailed by using exception stack trace string when IsDevelopment is true.

The parameters below will override their original value when they are set.

* `DefaultHttpStatusCode`
* `ContentType`
* `DefaultMessage`

If you wanted to use this middleware for only specific extepcion types, then you can use `ExceptionTypeList` to add your exception types. If the exception is not one of the given exceptions, middleware will not catch and manage the exception.

When `ExceptionHandlerAction` is set, this action will be invoked when any exception has occured.

If you want to override the `DefaultResponseModel` when an exception has occured, you able to use `UseResponseModelCreator` method by passing your customized class derived by `IResponseModelCreator` interface


```csharp
app.AddTBExceptionHandlingMiddleware(opt =>
{
    opt.IsDevelopment = true;
    opt.DefaultHttpStatusCode = HttpStatusCode.NotFound;
    opt.ContentType = "application/json";
    opt.DefaultMessage = "An Exception Occured";
    opt.ExceptionTypeList.Add<ArgumentNullException>();

    opt.ExceptionHandlerAction = async (httpContext, exception) => 
    {
        // When exception is handled
        var logger = httpContext.RequestServices.GetRequiredService<ILogger>();
        logger.LogError(exception, exception.Message);
    };

    // Use custom respone model
    opt.UseResponseModelCreator<CustomResponseModelCreator>();
});
```

An example of creating your `CustomResponseModelCreator`

```csharp
class CustomResponseModelCreator : IResponseModelCreator
{
    public object CreateModel(ModelCreatorContext model)
    {
        return new DefaultResponseModel()
        {
            ExceptionMessage = model.ErrorMessage,
            HttpStatusCode = model.HttpStatusCode
        };
    }
}
```

Definition of ModelCreatorContext

```csharp
public class ModelCreatorContext
{
    public string ErrorMessage { get; set; }

    public HttpStatusCode HttpStatusCode { get; set; }

    public Exception Exception { get; set; }
}
```


