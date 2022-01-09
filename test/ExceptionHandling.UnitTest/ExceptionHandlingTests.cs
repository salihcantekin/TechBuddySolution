using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TechBuddy.Middlewares.ExceptionHandling;

namespace ExceptionHandling.UnitTest
{
    internal partial class ExceptionHandlingTests
    {
        [Test]
        public async Task App_Should_Throw_Exception_WithoutMiddleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            Func<Task> act = () => host.GetTestClient().GetAsync("/");

            await act.Should().ThrowAsync<Exception>().WithMessage("exception message");
        }


        [Test]
        public async Task App_ShouldNot_Throw_Exception()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.Run(async (ctx) => { await ctx.Response.WriteAsync("Hello World"); });
                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);
            httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult().Should().Contain("Hello");
        }

        [Test]
        public async Task Exception_Should_Be_Cought_WithMiddleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware();
                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task Exception_Should_Cought_With_StackTrace_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.IsDevelopment = true;
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            var content = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            content.Should().Contain(".cs:line");
        }

        [Test]
        public async Task Exception_Should_Cought_With_DefaultMessage_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.DefaultMessage = "Default Message";
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
            var content = httpResponseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            content.Should().Contain("Default Message");
        }

        [Test]
        public async Task Exception_Should_Cought_With_StatusCode_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.DefaultHttpStatusCode = HttpStatusCode.BadRequest;
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Exception_Should_Cought_With_HandlerAction_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.ExceptionHandlerAction = async (ctx, ex) =>
                            {
                                // ignore the exception
                            };
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            Func<Task> act = () => host.GetTestClient().GetAsync("/");
            await act.Should().NotThrowAsync<Exception>();
        }

        [Test]
        public async Task Exception_Should_Cought_With_ExceptionType_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.ExceptionTypeList.Add<ArgumentException>();
                        });

                        app.Run(ctx => throw new ArgumentException("exception message"));
                    });
                })
                .StartAsync();

            Func<Task> act = () => host.GetTestClient().GetAsync("/");
            await act.Should().NotThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task Exception_Should_Thrown_With_ExceptionType_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.ExceptionTypeList.Add<ArgumentException>();
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            Func<Task> act = () => host.GetTestClient().GetAsync("/");
            await act.Should().NotThrowAsync<ArgumentException>();
            await act.Should().ThrowAsync<Exception>();
        }

        [Test]
        public async Task Exception_Should_Cought_WithModelCreator_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBExceptionHandlingMiddleware(opt =>
                        {
                            opt.UseResponseModelCreator<CustomResponseModelCreator>();
                        });

                        app.Run(ctx => throw new Exception("exception message"));
                    });
                })
                .StartAsync();

            Func<Task<HttpResponseMessage>> act = () => host.GetTestClient().GetAsync("/");
            await act.Should().NotThrowAsync<Exception>();

            var httpResponseMessage = await act.Invoke();
            httpResponseMessage.Should().NotBeNull();

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            content.Should().Contain("ErrorMessage");
            content.Should().Contain("StatusCode");
        }
    }
}
