namespace User_API.HealthCheck
{
    public static class DaprHealthCheckBuilderExtensions
    {
        public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
        builder.AddCheck<DaprHealthCheck>("dapr");
    }
}
