using Serilog;
using Musafir.AmaduesAPI.Exceptions;
using Serilog.Formatting.Compact;
using Musafir.AmaduesAPI.Middleware.IPValidation;
using Serilog.Filters;
using Serilog.Enrichers.HttpContextData;

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

            builder.Services.AddSerilog(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseIPValidation();

            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
