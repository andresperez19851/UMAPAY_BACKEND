using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Quartz;
using System.Text;

using UmaPay.Api.Helpers;
using UmaPay.Domain;
using UmaPay.IoC;
using UmaPay.Api.Job;
using Microsoft.EntityFrameworkCore;
using UmaPay.Repository;
using UmaPay.Interface.Repository;

var builder = WebApplication.CreateBuilder(args);

// Set the environment
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var services = builder.Services;

services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DataConnection")));

services.AddScoped<IUnitOfWork>(sp =>
           sp.GetRequiredService<DataContext>());

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:Secret"])),
                  ValidateIssuer = true,
                  ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
                  ValidateAudience = true,
                  ValidAudience = builder.Configuration["ApiSettings:Audience"],
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero
              };
          });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CORSPolicy", builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed((hosts) => true));
    });

    services.Configure<ApiConfig>(builder.Configuration.GetSection("AppSettings"));

    services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();

        var cleanupToInitiatedJob = new JobKey("CleanupToInitiatedJob");
        q.AddJob<ProcessInitiatedJob>(opts => opts.WithIdentity(cleanupToInitiatedJob));
        q.AddTrigger(opts => opts
            .ForJob(cleanupToInitiatedJob)
            .WithIdentity("CleanupToInitiatedJob-trigger")
            .WithCronSchedule(configuration["ScheduledSettings:CleanupToInitiated:CronExpression"]));
    });

    builder.Services.AddHttpClient();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    services.AddNetDependency(builder.Configuration);
    services.AddScoped<IUserContextService, UserContextService>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        dbContext.Database.Migrate();
    }

    app.ConfigureExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("CORSPolicy");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}