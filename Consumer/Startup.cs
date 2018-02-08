using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Consumer
{
    public class Startup : IConsoleStartup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Service>();

            services.AddScoped<BusObserver>();
            services.AddScoped<PublishObserver>();
            services.AddScoped<ConsumeObserver>();
        }
    }
}