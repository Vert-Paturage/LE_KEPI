namespace Middleware.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.SetupDependencies(builder.Configuration);

            builder.Services.AddProblemHandler();
            builder.AddLogging();
            
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