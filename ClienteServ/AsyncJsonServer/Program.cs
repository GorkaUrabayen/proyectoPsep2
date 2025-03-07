using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClienteServ.Servidor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            host.Run();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/proxy/{**catchAll}", async context =>
                {
                    var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
                    string apiUrl = "http://localhost:5062/api/pokemon";
                    string query = context.Request.Path.Value.Replace("/proxy", "");

                    var response = await httpClient.GetAsync(apiUrl + query);
                    var content = await response.Content.ReadAsStringAsync();
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(content);
                });

                endpoints.MapPost("/proxy", async context =>
                {
                    var httpClient = context.RequestServices.GetRequiredService<HttpClient>();
                    string apiUrl = "http://localhost:5062/api/pokemon";

                    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseBody);
                });
            });
        }
    }
}
