using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Data;
/// <summary>
/// Extension class to automigrate Discount DB and keep the db ready on application startup
/// </summary>
public static class DataExtensions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        //scope helps to get the DBContext object
        using var scope = app.ApplicationServices.CreateScope();
        //Get DB Context and reach the service - DiscountContext
        using var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        //MigrateAsync - validates database exists, creates it and apply the migration in the project migration folder
        dbContext.Database.MigrateAsync();

        return app;
    }
}
