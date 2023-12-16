namespace Template.Integration;

// TODO ref https://docs.microsoft.com/ja-jp/aspnet/core/test/integration-tests
public sealed class DashboardUsecaseTest
{
    private sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.ConfigureServices(services =>
            //{
            //    // TODO Customize startup
            //    services.Remove(services.First(static x => x.ServiceType == typeof(IDbProvider)));
            //});
        }
    }

    [Fact(DisplayName = "Dashboard display")]
    public async Task DisplayDashboard()
    {
        await using var application = new CustomWebApplicationFactory();

        // Arrange
        var client = application.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
