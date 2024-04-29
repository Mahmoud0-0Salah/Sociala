using AuthorizationService;
using EmailSendertServices;
using EncryptServices;
using Microsoft.EntityFrameworkCore;
using Sociala.Data;
using Sociala.Services;
using Sociala.Hubs;

namespace Sociala
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppData>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IEncrypt, EncryptClass>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddTransient <IAuthorization,Authorization>();

            builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            // Add services to the container.
          
             builder.Services.AddTransient<ICheckRelationShip, CheckRelationShip>();
            builder.Services.AddScoped<INotification, NotificationService>();

            builder.Services.AddSignalR();


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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<NotificationsHub>("/notifications");
            app.Run();
        }
    }

}