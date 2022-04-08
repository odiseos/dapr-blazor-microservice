using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Game_API.Context;
using Polly;
using Serilog;
using Game_API.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using FiveInLine.Dapr.EventPubSub;
using FiveInLine.Dapr.Services;
using Stoxapi_invoice.Business.Actors;

namespace Game_API
{
    public static class ProgramExtension
    {
        private const string AppName = "Game.API";

        public static void AddCustomConfiguration(this WebApplicationBuilder builder)
        {
            // Disabled temporarily until https://github.com/dapr/dotnet-sdk/issues/779 is resolved.
            //builder.Configuration.AddDaprSecretStore(
            //    "fiveinline-secretstore",
            //    new DaprClientBuilder().Build());
        }

        public static void AddCustomSwagger(this WebApplicationBuilder builder) =>
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Game_API - {AppName}", Version = "v1" });
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
                    builder.Configuration["ConnectionStrings:GameDB"],
                    name: "GameDB-check",
                    tags: new string[] { "gameDB" });

        public static void AddCustomApplicationServices(this WebApplicationBuilder builder)
        {
            //Inject DaprClient in Controllers
            builder.Services.AddControllers().AddDapr();
            builder.Services.AddDaprClient();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Inject Dapr Actors
            builder.Services.AddActors(options =>
            {
                options.Actors.RegisterActor<GameActor>();

                // Configure default settings
                options.ActorIdleTimeout = TimeSpan.FromMinutes(2);
                options.ActorScanInterval = TimeSpan.FromMinutes(1);
                options.DrainOngoingCallTimeout = TimeSpan.FromMinutes(1);
                options.DrainRebalancedActors = true;
            });

            //Services and Providers
            builder.Services.AddSingleton<IEventPubSubProvider, EventPubSubProvider>();
            builder.Services.AddSingleton<IStorageStateServiceProvider, StorageStateServiceProvider>();
            builder.Services.AddSingleton<IBindStorageProvider, BindStorageProvider>();
        }

        public static void AddCustomDatabase(this WebApplicationBuilder builder) =>
            builder.Services.AddDbContext<GameDbContext>(options => options.UseSqlServer(builder.Configuration["ConnectionStrings:GameDB"]));

        public static void AddDaprConfiguration(this WebApplication app)
        {
            app.UseCloudEvents();
            app.MapControllers();
            app.MapSubscribeHandler();
            app.MapActorsHandlers();
        }

        public static void ApplyDatabaseMigration(this WebApplication app)
        {
            // Apply database migration automatically. Note that this approach is not
            // recommended for production scenarios. Consider generating SQL scripts from
            // migrations instead.
            using var scope = app.Services.CreateScope();

            var retryPolicy = CreateRetryPolicy(app.Configuration, Log.Logger);
            var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();

            retryPolicy.Execute(context.Database.Migrate);
        }

        private static Policy CreateRetryPolicy(IConfiguration configuration, Serilog.ILogger logger)
        {
            // Only use a retry policy if configured to do so.
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
