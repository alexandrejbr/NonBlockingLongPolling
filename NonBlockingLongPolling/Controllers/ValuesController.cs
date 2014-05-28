using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;

namespace NonBlockingLongPolling.Controllers
{
    public class ValuesController : ApiController
    {
        private static readonly NonBlockingBroadcaster<string> Messages = new NonBlockingBroadcaster<string>();
        // GET api/values
        public Task<string> Get()
        {
            return Messages.Take().TimeoutAfter(5000).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw new HttpResponseException(HttpStatusCode.SeeOther);
                return t.Result;
            });
        }

        // POST api/values
        public void Post(string value)
        {
            Messages.Put(value);
        }
    }
}
