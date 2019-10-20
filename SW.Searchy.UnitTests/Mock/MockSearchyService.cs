using SW.PrimitiveTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.Searchy.UnitTests.Mock
{
    public class MockSearchyService : ISearchyService
    {
        public string Serves => typeof(Employee).FullName.ToLower();

        public IEnumerable<ISearchyFilterConfig> FilterConfigs
        {
            get
            {
                return new List<ISearchyFilterConfig> 
                { new SearchyFilterConfig {Type="int", Field="Id", Text="The Id" } };  
            }
        }


        public  Task<IEnumerable<object>> Search(SearchyRequest request)
        {
            return Task.FromResult(Employee.Sample.AsEnumerable<object>());
        }
    }
}
