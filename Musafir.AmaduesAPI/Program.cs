using Serilog;
using Musafir.AmaduesAPI.Extensions;
using Musafir.AmaduesAPI.Exceptions;
using Serilog.Formatting.Compact;
using Musafir.AmaduesAPI.Middleware.IPValidation;

namespace Musafir.AmaduesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


            builder.Services.AddProjectDependencies();
            builder.Services.AddThirdPartyDependnecies(builder);
            builder.Services.AddCustomMiddlewares();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .WriteTo.File(new CompactJsonFormatter(), "Logs/log-.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseIpValidation();

            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
