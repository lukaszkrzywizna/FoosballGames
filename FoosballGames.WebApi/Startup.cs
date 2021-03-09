using System.Reflection;
using FoosballGames.Infrastructure.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace FoosballGames.WebApi
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
            services.AddOptions();
            services.Configure<PostgreSqlSettings>(Configuration.GetSection(nameof(PostgreSqlSettings)));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "FoosballGames.WebApi", Version = "v1"});
            });

            services.AddScoped<ICommandQueryDispatcher>(sp => new CommandQueryDispatcher(sp.GetRequiredService));
            services.RegisterFoosballGamesComponents();
            services.AddDbContext<FoosballGamesContext>((sp, options) =>
            {
                var settings = sp.GetRequiredService<IOptions<PostgreSqlSettings>>().Value.BuildConnectionString();
                options.UseNpgsql(settings,
                    b => b.MigrationsAssembly(typeof(FoosballGamesContext).GetTypeInfo().Assembly.FullName));
            });
            services.AddScoped<IFoosballGamesRepository, FoosballGamesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FoosballGames.WebApi v1"));
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}