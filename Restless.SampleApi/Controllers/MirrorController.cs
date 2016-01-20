using System.Web.Http;
using Newtonsoft.Json.Linq;

namespace Restless.SampleApi.Controllers
{
    [RoutePrefix("mirror")]
    public class MirrorController : ApiController
    {
        [HttpPost, Route]
        public JToken Mirror(JToken body)
        {
            return body;
        }
    }
}