using CodeHelper.Core;
using CodeHelper.Data;
using CodeHelper.Models.Domain;
using CodeHelper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnectionString")), ServiceLifetime.Scoped);

builder.Services
    .AddFluentEmail("codehelperemail@gmail.com")
    .AddSmtpSender(new System.Net.Mail.SmtpClient("localhost")
    {
        EnableSsl = false,
        DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
        Port = 25
    });

builder.Services.AddScoped<UsersRepository, UsersRepository>();
builder.Services.AddScoped<AnswerRepository, AnswerRepository>();
builder.Services.AddScoped<QuestionsRepository, QuestionsRepository>();
builder.Services.AddScoped<TagRepository, TagRepository>();
builder.Services.AddScoped<LikesRepository, LikesRepository>();
builder.Services.AddScoped<ImageManager, ImageManager>();
builder.Services.AddScoped<EmailService, EmailService>();

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

//builder.Services.AddAuthentication().AddOAuth(googleOptions =>
//{
//    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
//});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Questions}/{action=All}");

app.Run();
