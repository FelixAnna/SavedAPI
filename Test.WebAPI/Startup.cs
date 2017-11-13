using log4net;
using log4net.Config;
using log4net.Repository;
using MEC.Logic;
using MEC.Logic.RabbitMq;
using MEC.Logic.Services;
using MEC.Model.Models;
using MEC.WebApi.Filters;
using MEC.WebApi.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using YD.Data;
using YD.Data.CacheServices;
using YD.Data.Services;
using YD.Data.Utils;
using YD.Data.WrapperServices;
using YD.Foundation.Config;
using YD.Foundation.Redis;
using YD.Foundation.Util;

namespace MEC.WebApi
{
    public class Startup
    {
        private ILoggerRepository repository { get; }
        private IConfigurationRoot configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            env.EnvironmentName = EnvironmentName.Production;//.Production;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("redis.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();
            repository = LogManager.CreateRepository(WsConstants.LOG_REPOSITORY_NAME);
            XmlConfigurator.Configure(repository, new FileInfo(WsConstants.LOG_CONFIG_FILENAME));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(ValidateFite));        // by type
            });

            services.AddSingleton<IConfigurationRoot>(this.configuration);

            //add EF service to container
            var connection = this.configuration.GetSection("ConnectionString").GetValue<string>("SqlServer");
            services.AddDbContext<HiShopTestContext>(options => options.UseSqlServer(connection));

            //add custome services
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ILogLogicService, LogLogicService>();
            services.AddScoped<IRepository, EFRepository>();
            services.AddScoped<LoginLogic>();
            services.AddScoped<PayLogic>();
            services.AddTransient<RabbitmqConfigSection>();

            services.AddScoped<ProductTypeService>();
            services.AddScoped<ProductTypeWrapperService>();
            services.AddScoped<ProductTypeDataService>();
            services.AddScoped<ProductTypeCacheService>();
            services.AddScoped<GameService>(); 

            //每一种MQ Service 都是单例
            services.AddSingleton(typeof(MailMqService<>));

            //only keep one redis instance for each request
            services.AddSingleton<RedisConnectionConfig>();
            services.AddSingleton<RedisManager>();
            services.AddSingleton<RedisRepository>();

            //注册启动后运行的服务
            services.AddSingleton<AfterStartup>(); 
            services.AddSingleton<RedisHeartbeat>();
            services.AddSingleton<CacheLoader>(); 

            // Adds a default in-memory implementation of IDistributedCache.
            //services.AddDistributedMemoryCache();

            //services.AddSession(options =>
            //{
            //    options.CookieName = ".MEC.Session";
            //    options.IdleTimeout = TimeSpan.FromSeconds(15 * 60);
            //});

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, AfterStartup after)
        {
            //解决控制台中文乱码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //异常处理中间件
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            }

            //其他中间件
            app.UseMiddleware(typeof(CorsHandlerMiddleware));

            //app.UseSession();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
                RequestPath = new PathString("/StaticFiles")
            });

            //app.UseStaticFiles();                   // Return static files and end pipeline.
            //app.UseIdentity();                     // Authenticate before you access secure resources.

            //默认路由
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Test}/{action=Index}/{id?}");
            });

            //After Application Start： 启动后做心跳检测，缓存预热等
            after.Start();
        }
    }
}
