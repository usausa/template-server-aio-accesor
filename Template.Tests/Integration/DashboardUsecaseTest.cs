namespace Template.Integration
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;

    using Xunit;

    // TODO ref https://docs.microsoft.com/ja-jp/aspnet/core/test/integration-tests
    public class DashboardUsecaseTest : IClassFixture<DashboardUsecaseTest.CustomWebApplicationFactory<Template.Web.Startup>>
    {
        public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
            where TStartup : class
        {
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                //builder.ConfigureServices(services =>
                //{
                //    // TODO Customize startup
                //    services.Remove(services.First(x => x.ServiceType == typeof(IDbProvider)));
                //});
            }
        }

        private readonly CustomWebApplicationFactory<Template.Web.Startup> factory;

        public DashboardUsecaseTest(CustomWebApplicationFactory<Template.Web.Startup> factory)
        {
            this.factory = factory;
        }

        [Fact(DisplayName = "Dashboard display")]
        public async Task DisplayDashboard()
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
