using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlockChain;
using BlockChainNode.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockChainNode
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.Configure<BlockChainOptions>(options =>
            {
                options.ChainFilePath = Configuration["Chain"];
                options.NodesFilePath = Configuration["Nodes"];
            });
            services.AddSingleton(sp => 
                File.Exists(Configuration["Chain"]) 
                    ? JsonConvert.DeserializeObject<Chain>(File.ReadAllText(Configuration["Chain"])) 
                    : new Chain());
            services.AddSingleton<List<Node>>(sp =>
                File.Exists(Configuration["Nodes"])
                    ? JsonConvert.DeserializeObject<List<Node>>(File.ReadAllText(Configuration["Chain"]))
                    : new List<Node>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
