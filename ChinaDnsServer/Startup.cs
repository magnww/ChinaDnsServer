using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnsServerCore;
using DnsServerCore.Dns;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechnitiumLibrary.Net.Dns;
using TechnitiumLibrary.Net.Proxy;

namespace ChinaDnsServer
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var logFolder = Path.Combine(Util.GetWorkingDirectory(), "logs");
            Directory.CreateDirectory(logFolder);

            var forwarders = Configuration.GetSection("Forwarders");
            var forwarderNameServers = forwarders.GetChildren().Where(t => t.Key == "NameServers").FirstOrDefault();
            var forwarderProxy = forwarders.GetChildren().Where(t => t.Key == "Proxy").FirstOrDefault();
            var reliableForwarders = Configuration.GetSection("ReliableForwarders");
            var reliableForwarderNameServers = reliableForwarders.GetChildren().Where(t => t.Key == "NameServers").FirstOrDefault();
            var reliableForwarderProxy = reliableForwarders.GetChildren().Where(t => t.Key == "Proxy").FirstOrDefault();
            var dnsServer = new DnsServer
            {
                LogManager = new LogManager(logFolder),
                ServerDomain = "ChinaDnsServer",
                AllowRecursion = true,
                ForwarderProtocol = Enum.Parse<DnsTransportProtocol>(forwarders["Protocol"]),
                Forwarders = forwarderNameServers.GetChildren().Select(ip => new NameServerAddress(ip.Value)).ToArray(),
                Proxy = forwarderProxy != null && forwarderProxy.GetChildren().Count() > 0 ? forwarderProxy.GetChildren().Select(t => new NetProxy(Enum.Parse<NetProxyType>(
                    t["Type"]),
                    IPAddress.Parse(t["Address"]),
                    int.Parse(t["Port"]))).ToArray() : null,
                ReliableForwarderProtocol = Enum.Parse<DnsTransportProtocol>(reliableForwarders["Protocol"]),
                ReliableForwarders = reliableForwarderNameServers.GetChildren().Select(ip => new NameServerAddress(ip.Value)).ToArray(),
                ReliableProxy = reliableForwarderProxy != null && reliableForwarderProxy.GetChildren().Count() > 0 ? reliableForwarderProxy.GetChildren().Select(t => new NetProxy(Enum.Parse<NetProxyType>(
                    t["Type"]),
                    IPAddress.Parse(t["Address"]),
                    int.Parse(t["Port"]))).ToArray() : null,
            };
            dnsServer.Start();

        }
    }
}
