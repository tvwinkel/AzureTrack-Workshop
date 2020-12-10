
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RMotownFestival.Api.Common;
using RMotownFestival.Api.DAL;
using RMotownFestival.Api.Options;

namespace RMotownFestival.Api
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
            services.Configure<AppSettingsOptions>(Configuration);

            services.AddCors();
            services.AddControllers();

            services.AddSingleton(p => new StorageSharedKeyCredential(
                Configuration.GetValue<string>("Storage:AccountName"),
                Configuration.GetValue<string>("Storage:AccountKey")
                ));

            services.AddSingleton(p => new BlobServiceClient(Configuration.GetValue<string>("Storage:ConnectionString")));

            services.AddSingleton<BlobUtility>();

            services.Configure<BlobSettingsOptions>(Configuration.GetSection("Storage"));

            services.AddDbContext<MotownDbContext>(
                options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
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

            // THIS IS NOT A SECURE CORS POLICY, DO NOT USE IN PRODUCTION
            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
