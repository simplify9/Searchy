using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SW.Searchy
{
    public interface ISearchyService
    {
        string Serves { get; }
        Task<object> Search(SearchyRequest request);
    }
}
