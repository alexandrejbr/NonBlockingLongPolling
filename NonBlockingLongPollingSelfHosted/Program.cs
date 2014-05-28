using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using NonBlockingLongPolling;
using Owin;

namespace NonBlockingLongPollingSelfHosted
{
    public class ValuesController : ApiController
    {
        private static readonly NonBlockingBroadcaster<string> Messages = new NonBlockingBroadcaster<string>();
        // GET api/values
        public Task<string> Get()
        {
            //return Task.FromResult("Benfica");

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


    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        }
    } 

    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:1602/";

            // Start OWIN host 
            WebApp.Start<Startup>(url: baseAddress);            

            Console.ReadLine(); 
        }
    }
}
