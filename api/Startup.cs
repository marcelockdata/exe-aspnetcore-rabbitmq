using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace api
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

            // Para este exemplo foi criado um container Docker baseado
            // em uma imagem do RabbitMQ. Segue o comando para geração
            // desta estrutura:
            // docker run -d --hostname rabbit-local --name testes-rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=ckdata -e RABBITMQ_DEFAULT_PASS=ckdata@ckdata rabbitmq:3-management
            var rabbitMQConfigurations = new RabbitMQConfigurations();
            new ConfigureFromConfigurationOptions<RabbitMQConfigurations>(
                Configuration.GetSection("RabbitMQConfigurations"))
                    .Configure(rabbitMQConfigurations);
            services.AddSingleton(rabbitMQConfigurations);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
