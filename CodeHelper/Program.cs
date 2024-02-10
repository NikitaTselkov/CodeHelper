using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString")), ServiceLifetime.Scoped);

        builder.Services.AddScoped<UsersRepository, UsersRepository>();
        builder.Services.AddScoped<AnswerRepository, AnswerRepository>();
        builder.Services.AddScoped<QuestionsRepository, QuestionsRepository>();
        builder.Services.AddScoped<TagRepository, TagRepository>();
        builder.Services.AddScoped<LikesRepository, LikesRepository>();
        builder.Services.AddScoped<ImageManager, ImageManager>();
        builder.Services.AddScoped<SitemapGenerator, SitemapGenerator>();

        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.LoginPath = "/Views/Autorization/Login";
            options.SlidingExpiration = true;
            options.Cookie.Name = GlobalConstants.AuthCookieName;
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/plain",
                "text/css",
                "application/javascript",
                "text/html"
            });
            options.EnableForHttps = true;
        });

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        var basePath = builder.Configuration.GetSection("BASE_PATH").Value;
        if (basePath != null)
        {
            Console.WriteLine($"Using base path '{basePath}'");
            app.UsePathBase(basePath);
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.UseResponseCompression();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}");

        app.MapControllerRoute(
          name: "sitemap",
          pattern: "/sitemap.xml",
          defaults: new { controller = "Home", action = "SitemapIndex" });

        app.MapControllerRoute(
        name: "sitemap",
        pattern: "{offset}/sitemap.xml",
        defaults: new { controller = "Home", action = "Sitemap", offset = "offset" });

        app.Run();
    }
}