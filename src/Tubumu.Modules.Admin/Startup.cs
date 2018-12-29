using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Modules;
using Tubumu.Modules.Admin.Entities;
using Tubumu.Modules.Admin.Frontend;
using Tubumu.Modules.Admin.Hubs;
using Tubumu.Modules.Admin.ModuleMenus;
using Tubumu.Modules.Admin.Repositories;
using Tubumu.Modules.Admin.Services;
using Tubumu.Modules.Admin.Settings;
using Tubumu.Modules.Framework.Authorization;
using Tubumu.Modules.Framework.Services;

namespace Tubumu.Modules.Admin
{
    public class Startup : StartupBase
    {
        private readonly IConfiguration _configuration;

        public Startup(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Permission
            services.AddScoped<IPermissionProvider, Permissions>();

            // Permission
            services.AddScoped<IMenuProvider, Menus>();

            services.AddHttpClient();
            services.AddDbContext<TubumuContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("Tubumu")).ConfigureWarnings(warnings =>
                {
                    warnings.Throw(CoreEventId.IncludeIgnoredWarning);
                    //warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
                }));
        
            // Repositories
            services.AddScoped<IBulletinRepository, BulletinRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Services
            services.AddSingleton<ISmsSender, SubMailSmsSender>();
            services.AddScoped<IBulletinService, BulletinService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminUserService, AdminUserService>();

            // Frontend
            services.Configure<FrontendSettings>(_configuration.GetSection("FrontendSettings"));

            // SubMail 短信发送接口参数配置
            services.Configure<SubMailSmsSettings>(_configuration.GetSection("SubMailSmsSettings"));

            // 认证设置
            services.Configure<AuthenticationSettings>(_configuration.GetSection("AuthenticationSettings"));
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            //string[] controllerNamespaces = new string[] { "Tubumu.Modules.Admin.Controllers" };

            #region View

            routes.MapAreaRoute(
                name: "Admin.View",
                areaName: "Tubumu.Modules.Admin",
                template: "Admin/View",
                defaults: new { controller = "View", action = "View" }
            ); // 无 namespaces

            routes.MapAreaRoute(
                name: "Admin.View.Action",
                areaName: "Tubumu.Modules.Admin",
                template: "{action}",
                defaults: new { controller = "View", action = "Index" }
            ); // 无 namespaces；Login, Index 等无 Controller 前缀

            #endregion

            app.UseSignalR(configure =>
            {
                configure.MapHub<NotificationHub>("/hubs/notificationHub");
            });

        }
    }
}
