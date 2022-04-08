using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using User_API.Context;
using Polly;
using Serilog;
using User_API.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using FiveInLine.Dapr.EventPubSub;
using FiveInLine.Dapr.Services;
using User_API.IntegrationEvents;

namespace User_API
{
    public static class ProgramExtension
    {
        private const string AppName = "User.API";

        public static void AddCustomConfiguration(this WebApplicationBuilder builder)
        {
            // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
            //builder.Configuration.AddDaprSecretStore(
            //    "eshop-secretstore",
            //    new DaprClientBuilder().Build());
        }

        public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"User_API - {AppName}", Version = "v1" });
            });

        public static void UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppName} V1");
            });
        }

        public static void AddCustomHealthChecks(this WebApplicationBuilder builder) =>
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddDapr()
                .AddSqlServer(
                    builder.Configuration["ConnectionStrings:UserDB"],
                    name: "UserDB-check",
                    tags: new string[] { "userDB" });

        public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IEventPubSubProvider, EventPubSubProvider>();
            builder.Services.AddSingleton<IStorageStateServiceProvider, StorageStateServiceProvider>();
            builder.Services.AddSingleton<IBindStorageProvider, BindStorageProvider>();
            builder.Services.AddTransient<IUserToProcessEventHandler, UserToProcessEventHandler>();
        }

        public static void AddCustomDatabase(this WebApplicationBuilder builder) =>
            builder.Services.AddDbContext<UserDbContext>(
                options => options.UseSqlServer(builder.Configuration["ConnectionStrings:UserDB"]));

        public static void AddDaprConfiguration(this WebApplication app)
        {
            app.UseCloudEvents();
            app.MapControllers();
            app.MapSubscribeHandler();
        }

        public static void ApplyDatabaseMigration(this WebApplication app)
        {
            // Apply database migration automatically. Note that this approach is not
            // recommended for production scenarios. Consider generating SQL scripts from
            // migrations instead.
            using var scope = app.Services.CreateScope();

            var retryPolicy = CreateRetryPolicy(app.Configuration, Log.Logger);
            var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

            retryPolicy.Execute(context.Database.Migrate);
        }

        private static Policy CreateRetryPolicy(IConfiguration configuration, Serilog.ILogger logger)
        {
            // Only use a retry policy if configured to do so.
            // When running in an orchestrator/K8s, it will take care of restarting failed services.
            if (bool.TryParse(configuration["RetryMigrations"], out bool retryMigrations))
            {
                return Policy.Handle<Exception>().
                    WaitAndRetryForever(
                        sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                        onRetry: (exception, retry, timeSpan) =>
                        {
                            logger.Warning(
                                exception,
                                "Exception {ExceptionType} with message {Message} detected during database migration (retry attempt {retry})",
                                exception.GetType().Name,
                                exception.Message,
                                retry);
                        }
                    );
            }

            return Policy.NoOp();
        }
    }
}
