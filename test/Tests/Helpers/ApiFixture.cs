using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class ApiFixture<TAuth>:IApiFixture where TAuth:IApiAuth
{
    private readonly FakeSystemClock _fakeSystemClock= new(InitialNow);
    private readonly IDatabaseContextSetup _databaseContextSetup;
    TestServerContext Create()
    {
        var webAppFactory = new WebApplicationFactory<Program>();
        var application = webAppFactory
            .WithWebHostBuilder(builder =>
            {
                _auth.Configure(builder);
                builder.ConfigureServices(services =>
                {
                    _databaseContextSetup.Use(services);
                    services.Remove(services.First(s => s.ServiceType == typeof(ISystemClock)));
                    services.AddSingleton<ISystemClock>(_fakeSystemClock);
                    ConfigureServices(services);
                });
                builder.UseEnvironment("Test");
            });
        using var serviceScope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        _databaseContextSetup.Migrate(serviceScope);
        return new TestServerContext(application.Server, webAppFactory);
    }


    protected virtual void ConfigureServices(IServiceCollection services) {}



    private readonly TestServerContext _testServerContext;
    private readonly TAuth _auth;

    public ApiFixture(IDatabaseContextSetup databaseContextSetup, TAuth auth)
    {
        _databaseContextSetup = databaseContextSetup;
        _auth = auth;
        _testServerContext = Create();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _testServerContext.Dispose();
            _databaseContextSetup.TryRemove().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public TestServer Server => _testServerContext.Server;

    public async Task<HttpResponseMessage> PostAuction(string auctionRequest, AuthToken auth) =>
        await Server.CreateRequest("/auctions").And(r =>
        {
            r.Content = Json(auctionRequest);
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).PostAsync();

    public async Task<HttpResponseMessage> PostBidToAuction(long id, string bidRequest, AuthToken auth) =>
        await Server.CreateRequest($"/auctions/{id}/bids").And(r =>
        {
            r.Content = Json(bidRequest);
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).PostAsync();
    private static StringContent Json(string bidRequest) => new(bidRequest, Encoding.UTF8, "application/json");
    public async Task<HttpResponseMessage> GetAuction(long id, AuthToken auth)=>
        await Server.CreateRequest($"/auctions/{id}").And(r =>
        {
            AcceptJson(r);
            _auth.TryAddAuth(r, auth);
        }).GetAsync();
    private static void AcceptJson(HttpRequestMessage r) => r.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    public void SetTime(DateTimeOffset now) => _fakeSystemClock.Now = now;

    private sealed class TestServerContext:IDisposable
    {
        internal TestServerContext(TestServer server, WebApplicationFactory<Program> webApplicationFactory)
        {
            Server = server;
            _webApplicationFactory = webApplicationFactory;
        }

        public TestServer Server { get; }

        private WebApplicationFactory<Program> _webApplicationFactory;

        public void Dispose()
        {
            Server.Dispose();
            _webApplicationFactory.Dispose();
        }
    }
}