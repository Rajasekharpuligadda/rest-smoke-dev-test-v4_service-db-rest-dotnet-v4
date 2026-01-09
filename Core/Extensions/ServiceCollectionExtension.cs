namespace ServiceDbRestDotnetV4.Core.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection InitApp(this IServiceCollection services)
        {
            var app = new Bootstrap(services);
            return app.Configure();
        }
    }
}
