using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new HttpClient();

            List<Task<HttpResponseMessage>> responses = new List<Task<HttpResponseMessage>>();

            for (int i = 0; i < 10000; ++i)
            {
                responses.Add(client.GetAsync("http://localhost:1602/api/values"));
                Console.WriteLine(i);
            }

            foreach (var response in responses)
            {
                response.ContinueWith(async t => Console.WriteLine(await t.Result.Content.ReadAsStringAsync()));
            }
            Console.ReadLine();
        }
    }
}
