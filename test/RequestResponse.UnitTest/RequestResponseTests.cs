using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Sinks.Graylog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TechBuddy.Middlewares.RequestResponse;
using TechBuddy.Middlewares.RequestResponse.Models;

namespace RequestResponse.UnitTest
{
    public class Tests
    {
        [Test]
        public async Task Response_Length_Shoud_Match_InHandler()
        {
            const string defaultResponseMessage = "Default response string";

            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                context.ResponseLength.Should().Be(defaultResponseMessage.Length);
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("", _app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                await context.Response.WriteAsync(defaultResponseMessage);
                            });
                        });

                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
        }

        [Test]
        public async Task Request_Body_Should_Match()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                context.RequestBody.Should().Contain("Salih").And.Contain("Cantekin");
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("", _app =>
                        {
                        });

                    });
                })
                .StartAsync();

            var reqObj = new { FirstName = "Salih", LastName = "Cantekin" };
            var httpResponseMessage = await host.GetTestClient().PostAsJsonAsync("/", reqObj);
        }

        [Test]
        public async Task Request_Uri_Should_Match()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                var uri = context.BuildUrl();

                                uri.Host.Should().Be("localhost");
                                uri.Port.Should().Be(80);
                                uri.Scheme.Should().Be("http");
                                uri.Query.Should().Be("?x=1&y=2");
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("/TestPath", _app =>
                        {
                        });

                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/TestPath?x=1&y=2");
        }

        [Test]
        public async Task Response_Timing_Shoud_Match_InHandler()
        {
            const string defaultResponseMessage = "Default response string";

            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                context.ResponseCreationTime.Value.TotalMilliseconds.Should().BeGreaterOrEqualTo(100);
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("", _app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                await Task.Delay(100);
                                await context.Response.WriteAsync(defaultResponseMessage);
                            });
                        });

                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
        }


        [Test]
        public async Task Response_Body_Should_Match()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                context.ResponseBody.Should().Contain("Message");
                                context.ResponseBody.Should().Contain("Response Message");
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("", _app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                var response = new { Message = "Response Message" };
                                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                            });
                        });

                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/");
        }

        [Test]
        public async Task RequestAndResponse_Body_Should_Match()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseHandler(async context =>
                            {
                                context.RequestBody.Should().Contain("Salih").And.Contain("Cantekin");
                                context.ResponseBody.Should().Contain("Message");
                                context.ResponseBody.Should().Contain("Response Message");
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("", _app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                var response = new { Message = "Response Message" };
                                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                            });
                        });

                    });
                })
                .StartAsync();

            var reqObj = new { FirstName = "Salih", LastName = "Cantekin" };
            var httpResponseMessage = await host.GetTestClient().PostAsJsonAsync("/", reqObj);
        }

        [Test]
        public async Task Exception_Should_Cought_With_StackTrace_With_Middleware()
        {
            using var host = await new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddJsonConsole();
                    logging.AddDebug();
                })
                .UseSerilog()
                .ConfigureWebHost(webHost =>
                {
                    Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

                    Log.Logger = new LoggerConfiguration()
                    .WriteTo.Graylog(new GraylogSinkOptions()
                    {
                        HostnameOrAddress = "localhost",
                        Port = 12201,
                        TransportType = Serilog.Sinks.Graylog.Core.Transport.TransportType.Udp,
                        Facility = nameof(UnitTest)
                    })
                    .CreateLogger();

                    webHost.UseTestServer();
                    webHost.Configure(app =>
                    {
                        app.AddTBRequestResponseMiddleware(opt =>
                        {
                            opt.UseLogger(app.ApplicationServices.GetService<ILoggerFactory>(), opt =>
                            {
                                opt.UseSeparateContext = true;
                                opt.LoggingLevel = LogLevel.Information;
                                opt.LoggingFields = new List<LogFields>
                                {
                                    LogFields.Path,
                                    LogFields.QueryString,
                                    LogFields.HostName,
                                    LogFields.ResponseTiming
                                };
                            });

                            opt.UseHandler(async context =>
                            {
                                await Task.CompletedTask;
                            });
                        });

                        app.Map("/TestPathAndQueryString", _app =>
                        {
                            app.Use(async (context, next) =>
                            {
                                await Task.Delay(1000);
                                await context.Response.WriteAsync("TestPathAndQueryString response");
                            });
                        });

                    });
                })
                .StartAsync();

            var httpResponseMessage = await host.GetTestClient().GetAsync("/TestPathAndQueryString?p1=1&p2=2");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            content.Should().NotBeNull().And.NotBe("");
        }
    }
}