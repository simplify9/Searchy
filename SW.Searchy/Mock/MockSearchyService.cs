using SW.PrimitiveTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.Searchy.Mock
{
    public class MockSearchyService : ISearchyService
    {
        public string Serves => typeof(Employee).FullName.ToLower();

        public  Task<IEnumerable<object>> Search(SearchyRequest request)
        {
            return Task.FromResult(Employee.Sample.AsEnumerable<object>());
        }
    }
}
