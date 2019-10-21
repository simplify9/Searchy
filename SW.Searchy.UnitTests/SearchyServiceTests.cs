using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var filterTyped = new SearchyFilterTyped { Field = "FirstName", Rule = SearchyRule.EqualsTo, ValueDate = DateTime.UtcNow };
            var v2 = await sc.Search<Mock.Employee>(new SearchyRequest { Conditions = new List<SearchyCondition> { new SearchyCondition(new SearchyFilter(filterTyped))  } } );
            //var v3 = await sc.Search("mock");
            //var v4 = await sc.Get("mock", "key1");
        }

        public class SearchyFilterTyped : ISearchyFilterTyped
        {
            public string Field { get; set; }
            public  object Value { get; set; }
            public SearchyRule Rule { get; set; }
            public bool? ValueBool { get; set; }
            public byte? ValueByte { get; set; }
            public int? ValueInt { get; set; }
            public long? ValueLong { get; set; }
            public decimal? ValueDecimal { get; set; }
            public string ValueString { get; set; }
            public DateTime? ValueDate { get; set; }
            public byte[] ValueByteArray { get; set; }
            public int[] ValueIntArray { get; set; }
            public long[] ValueLongArray { get; set; }
            public string[] ValueStringArray { get; set; }
        }
    }
}
