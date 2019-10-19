using SW.PrimitiveTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SW.Searchy.Mock
{
    public class MockSearchyService : ISearchyService
    {
        public string Serves => typeof(Employee).FullName.ToLower();

        public Task<object> Search(SearchyRequest request)
        {
            return Task.FromResult((object)Employee.Sample);
        }
    }
}
