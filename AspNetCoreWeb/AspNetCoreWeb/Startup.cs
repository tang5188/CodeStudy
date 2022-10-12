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
            //����ǿ�����ģʽ���������쳣�м��
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("default.html");
            //����Ĭ��ҳ��
            app.UseDefaultFiles(options);
            //���Է��ʾ�̬�ļ�
            app.UseStaticFiles();

            /* �൱�������UseDefaultFiles��UseStaticFiles
            * FileServerOptions fileServerOptions = new FileServerOptions();
            * fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            * fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("default.html");
            * app.UseFileServer();
            */

            //·���м��
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

            ////�ս���м��
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
