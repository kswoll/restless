using System;
using System.Linq;
using System.Web.Http;
using Restless.SampleApi.Models;

namespace Restless.SampleApi.Controllers
{
    [RoutePrefix("pagination")]
    public class PaginationController : ApiController
    {
        [HttpGet, Route]
        public Page GetPage(int offset = 0, int limit = 25)
        {   
            const int totalCount = 151;
            var random = new Random(1);
            for (var i = 0; i < offset; i++)
                random.Next();
            return new Page
            {
                Data = Enumerable.Range(offset, Math.Min(limit, totalCount - offset)).Select(x => random.Next().ToString()).ToArray(),
                TotalCount = totalCount
            };
        }
    }
}