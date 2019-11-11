using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CertTester
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            var result = store.Certificates.Find(X509FindType.FindBySerialNumber, "2CA3F406530988964A6015AACC5BA210", false);
            Console.WriteLine($"Found {result.Count} matching certs");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}