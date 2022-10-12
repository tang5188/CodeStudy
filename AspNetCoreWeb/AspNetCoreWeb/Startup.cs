using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreWeb
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //如果是开发者模式，则启用异常中间件
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("default.html");
            //定义默认页面
            app.UseDefaultFiles(options);
            //可以访问静态文件
            app.UseStaticFiles();

            /* 相当于上面的UseDefaultFiles、UseStaticFiles
            * FileServerOptions fileServerOptions = new FileServerOptions();
            * fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            * fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("default.html");
            * app.UseFileServer();
            */

            //路由中间件
            app.UseRouting();

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("MiddleWare(1)-In\r\n");
                await next();
                await context.Response.WriteAsync("MiddleWare(1)-Out\r\n");
            });
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("MiddleWare(2)-In\r\n");
                await next();
                await context.Response.WriteAsync("MiddleWare(2)-Out\r\n");
            });

            ////终结点中间件
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync($"Hello {configuration["MyName"]}!\r\n");
            //    });
            //});

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Run MiddleWare(3)-In\r\n");
            });
        }
    }
}
