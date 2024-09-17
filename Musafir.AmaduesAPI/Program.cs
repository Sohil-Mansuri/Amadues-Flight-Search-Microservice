using Serilog;
using Musafir.AmaduesAPI.Exceptions;
using Musafir.AmaduesAPI.Middleware.IPValidation;
using Musafir.AmaduesAPI.FluentValidation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Runtime.CompilerServices;


namespace Musafir.AmaduesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<FlightSearchRequestValidator>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


            builder.Services.AddProjectDependencies();
            builder.Services.AddThirdPartyDependnecies(builder);
            builder.Services.AddCustomMiddlewares();
            builder.Services.AddSerilog(builder);

            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("FixedWindowPolicy", config =>
                {
                    config.PermitLimit = 10;
                    config.Window = TimeSpan.FromMinutes(1);
                    config.QueueLimit = 1;
                    config.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                });

                options.OnRejected =  (context, CancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429; // Too Many Requests
                    context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return ValueTask.CompletedTask;
                };
            });

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

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}
