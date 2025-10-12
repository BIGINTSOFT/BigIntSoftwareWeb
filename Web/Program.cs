using Microsoft.EntityFrameworkCore;
using DataAccess.DbContext;
using Bussiness.Repository.Abstract;
using Bussiness.Repository.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Entities.Entity;
using System.Text.Json.Serialization;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
             builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                })
                .AddRazorRuntimeCompilation();
            // Database connection
            builder.Services.AddDbContext<BigIntSoftwareDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repository pattern - Generic Repositories
            builder.Services.AddScoped<IGenericRepository<User>, GenericRepository<User>>();
            builder.Services.AddScoped<IGenericRepository<Role>, GenericRepository<Role>>();
            builder.Services.AddScoped<IGenericRepository<Menu>, GenericRepository<Menu>>();
            builder.Services.AddScoped<IGenericRepository<Permission>, GenericRepository<Permission>>();
            builder.Services.AddScoped<IGenericRepository<UserRole>, GenericRepository<UserRole>>();
            builder.Services.AddScoped<IGenericRepository<UserMenu>, GenericRepository<UserMenu>>();
            builder.Services.AddScoped<IGenericRepository<UserMenuPermission>, GenericRepository<UserMenuPermission>>();
            builder.Services.AddScoped<IGenericRepository<RoleMenu>, GenericRepository<RoleMenu>>();
            builder.Services.AddScoped<IGenericRepository<RoleMenuPermission>, GenericRepository<RoleMenuPermission>>();
            builder.Services.AddScoped<IGenericRepository<Customers>, GenericRepository<Customers>>();

            // Repository pattern - Specific Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IMenuRepository, MenuRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IUserMenuRepository, UserMenuRepository>();
            builder.Services.AddScoped<IUserMenuPermissionRepository, UserMenuPermissionRepository>();
            builder.Services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
            builder.Services.AddScoped<IRoleMenuPermissionRepository, RoleMenuPermissionRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/Login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(24);
                    options.SlidingExpiration = true;
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Auth}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
