using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using WebApiPractice.Api.Domain;
using WebApiPractice.Api.Extensions;
using WebApiPractice.Api.Mapper;
using WebApiPractice.Api.ValidationFlow;
using WebApiPractice.Api.ValidationFlow.Interfaces;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.Repositories;

namespace WebApiPractice.Api
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
            // Add MediatR functionality
            services.AddMediatR(Assembly.GetExecutingAssembly());
            // register validation contract handlers
            services.AddContractHandlers(Assembly.GetAssembly(typeof(IValidationContractHandler))!);
            // register the pipeline
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ContractValidationPipeline<,>));

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("sqlConnectionString")));
            services.AddScoped<AppDbContext>();
            services.AddScoped<IObjectMapper, ObjectMapper>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddControllers()
                    .AddNewtonsoftJson(
                        options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiPractice.Api", Version = "v1" }));
            services.AddSwaggerGenNewtonsoftSupport();

            services.Configure<ApiConfiguration>(Configuration.GetSection("ApiConfiguration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("v1/swagger.json", "WebApiPractice.Api v1"));
            }
            app.UseMiddleware<Middlewares.ExceptionHandlerMiddleware>();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            // This will make the HTTP requests log as rich logs instead of plain text.
            app.UseSerilogRequestLogging();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
