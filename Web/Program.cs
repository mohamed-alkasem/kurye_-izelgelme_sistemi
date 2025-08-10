using Web.Hubs;
using Domain.Interfaces;
using Application.Worker;
using Infrastructure.Data;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //SqlServer-------------------------------------------
            var conStr = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<DatabaseContext>(x => x.UseSqlServer(conStr));
            //----------------------------------------------------

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Services--------------------------------------------
            builder.Services.AddSignalR();

            builder.Services.AddScoped<SiparisPdfService>();
            builder.Services.AddScoped<SiparisPdfTumService>();

            builder.Services.AddScoped<KullaniciPdfService>();
            builder.Services.AddScoped<KullaniciPdfTumService>();

            builder.Services.AddScoped<IKuryeService, KuryeService>();

            builder.Services.AddScoped<IEmailSender, EmailSenderQueueProducer>();

            builder.Services.AddScoped<ISikayetService, SikayetService>();           // Proxy pattern burada uygulanmıştır.
            builder.Services.Decorate<ISikayetService, SikayetServiceProxy>();       // Proxy pattern burada uygulanmıştır.

            builder.Services.AddScoped<IDatabaseService, SqlDatabaseService>();      // Adapter pattern burada uygulanmıştır.
            builder.Services.AddScoped<IKullaniciService, DatabaseServiceAdapter>(); // Adapter pattern burada uygulanmıştır.

            builder.Services.AddScoped<IKuryeciRepository, KuryeciRepository>();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.AssemblyReference).Assembly));
            //----------------------------------------------------

            //Configure Identity----------------------------------
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<DatabaseContext>()
                    .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true; // En az bir rakam zorunlu
                options.Password.RequireNonAlphanumeric = false; // Özel karakter zorunluluğu yok
                options.Password.RequiredLength = 8; // Şifrenin minimum uzunluğu
            });
            //----------------------------------------------------

            //Cookie Ayarları-------------------------------------
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromHours(1); // Oturum süresi: 1 saat
                options.SlidingExpiration = true;              // Süre yenilenmez, tam 1 saatte logout
            });
            //----------------------------------------------------

            var app = builder.Build();

            // Rolleri ve admin kullanıcıyı ekleme----------------------------------
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                Task.Run(async () =>
                {
                    // Rolleri oluştur
                    string[] roles = { "Admin", "Kuryeci", "Kullanici" };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    string adminTCNo = "12345678900";
                    string adminPassword = "Admin123!";
                    if (await userManager.FindByNameAsync(adminTCNo) == null)
                    {
                        var adminUser = new AppUser
                        {
                            UserName = adminTCNo,
                            TC = adminTCNo,
                            AdSoyad = "Sistem Yöneticisi",
                            KullaniciRolu = "Admin"
                        };

                        var result = await userManager.CreateAsync(adminUser, adminPassword);
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "Admin");
                        }
                    }
                }).GetAwaiter().GetResult();
            }
            //-----------------------------------------------------------------------

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.MapHub<ChatHub>("/chathub");   // Chathub kullandık

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(

                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            Task.Run(() =>
            {
                var emailConsumer = new EmailQueueConsumer();
                emailConsumer.Start();
            });

            app.Run();
        }
    }
}
