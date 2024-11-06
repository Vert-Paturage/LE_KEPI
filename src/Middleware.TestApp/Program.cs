using Middleware.TestApp;
using Middleware.TestApp.Interfaces;

namespace Middleware.Test.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddDependencies(builder.Configuration["MiddlewareUrl"]!);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.MapControllers();
            
            app.Services.GetService<IErpMiddleware>()
               .CallRegisterMethodAsync()
               .ConfigureAwait(false);

            app.Run();
            
          
        }
    }
}
