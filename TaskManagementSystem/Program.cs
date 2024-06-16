using System;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace TaskManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Define the base address for the Web API
            var baseAddress = new Uri("http://localhost:8080/");

            // Set up the self-host configuration
            var config = new HttpSelfHostConfiguration(baseAddress);

            // Configure Web API routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configure formatters (optional, can be extended as needed)
            config.Formatters.XmlFormatter.UseXmlSerializer = true;

            // Enable CORS if needed
            config.EnableCors();

            // Create and open the server
            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("Web API Self-hosted on " + baseAddress);
                Console.WriteLine("Press Enter to terminate.");
                Console.ReadLine();
            }
        }
    }
}
