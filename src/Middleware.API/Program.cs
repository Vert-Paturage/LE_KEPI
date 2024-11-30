using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Middleware.API.Exceptions;
using Middleware.API.Exceptions.Handlers;

namespace Middleware.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.SetupDependencies(builder.Configuration.GetValue<bool>("IgnoreSSL"));

            builder.Services.AddProblemDetails(options => {
                options.CustomizeProblemDetails = context => {
                    context.ProblemDetails.Instance =
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });

            builder.Services.AddExceptionHandler<ProblemExceptionHandler>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            // app.UseHttpsRedirection();
            app.UseExceptionHandler();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}