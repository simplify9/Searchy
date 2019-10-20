using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace SW.Searchy.UnitTests
{
    [TestClass]
    public class SearchyServiceTests
    {

        static TestServer server;
        

        [ClassInitialize]
        public static void ClassInitialize(TestContext tcontext)
        {
            server = new TestServer(new WebHostBuilder()
                .UseEnvironment("UnitTesting")
                .UseStartup<TestStartup>());
            
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            server.Dispose();  
        }

        [TestMethod]
        async public Task TestMethod1()
        {
            //SearchyFilterConfigType.RulesFor(SearchyFilterConfigType.String).(SearchyRule.GreaterThan);
            var sc = new SearchyClient(server.CreateClient());

            var v1 = await sc.ListAvailable();
            var v3 = await sc.GetFilterConfigs<Mock.Employee>(); 
            var v2 = await sc.Search<Mock.Employee>(new SearchyRequest());
            //var v3 = await sc.Search("mock");
            //var v4 = await sc.Get("mock", "key1");
        }
    }
}
