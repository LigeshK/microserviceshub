namespace Ordering.API;

public static class DependencyInjection
{
    /// <summary>
    /// Add API Extensions
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        //services.AddCarter();

        return services;
    }
    /// <summary>
    /// Use API extensions
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseApiServices(this WebApplication app)
    {
        //app.MapCarter();
        return app;
    }

}
