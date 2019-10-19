using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;

namespace SW.Searchy
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchyController : ControllerBase
    {
        readonly IServiceProvider serviceProvider;
        public SearchyController(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IActionResult List()
        {
            var svcNames = serviceProvider.GetServices<ISearchyService>().Select(e => e.Serves);

            HashSet<string> result = new HashSet<string>(); ;

            foreach (var name in svcNames)
            {
                result.Add(name.ToLower());
            }

            return new OkObjectResult(result);
        }

        ISearchyService GetService(string serviceName)
        {
            var svcs = serviceProvider.GetServices<ISearchyService>().Where(e => e.Serves== serviceName);
            return svcs.SingleOrDefault();
        }

        [HttpPost("{serviceName}")]
        public async Task<IActionResult> Search(string serviceName, [FromBody]SearchyRequest request)
        {
            var svc = GetService(serviceName);
            if (svc == null) return NotFound();

            var result = new SearchyResponse<object>
            {
                Result = (await svc.Search(request))
            };

            return new OkObjectResult(result);
        }

        [HttpGet("{serviceName}")]
        public async Task<IActionResult> Get(string serviceName)
        {
            string queryString = Request.QueryString.ToString().ToLower();

            var svc = GetService(serviceName);
            if (svc == null) return NotFound();

            var request = new SearchyRequest();

            return new OkObjectResult(await svc.Search(request));
        }
    }
}