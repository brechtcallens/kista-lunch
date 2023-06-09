namespace TheMostAmazingLunchAPI.Data;
public static class DbExtensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<LunchContext>();
            context.Database.EnsureCreated();
            DbInitializer.initialize(context);
        }
    }
}
