using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReversiRestApi.Model;

namespace ReversiRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ISpelRepository, SpelRepository>()
                .BuildServiceProvider();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}