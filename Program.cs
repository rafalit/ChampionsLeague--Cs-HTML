using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja aplikacji
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Rejestracja us³ug MongoDB
builder.Services.Configure<ClubsStoreDatabaseSettings>(builder.Configuration.GetSection(nameof(ClubsStoreDatabaseSettings)));
builder.Services.AddSingleton<IClubsStoreDatabaseSettings>(sp => sp.GetRequiredService<IOptions<ClubsStoreDatabaseSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration.GetValue<string>("ClubsStoreDatabaseSettings:ConnectionString")));

// Rejestracja serwisów
builder.Services.AddScoped<IClubsService, ClubsService>();
builder.Services.AddScoped<IUserService, UserService>();

// Dodanie obs³ugi sesji
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Rejestracja kontrolerów i widoków
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseRouting();

app.UseSession(); // Dodanie obs³ugi sesji

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/account/login");
        return;
    }

    await next();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");

    endpoints.MapControllerRoute(
        name: "users",
        pattern: "api/Users/{action=Login}/{id?}",
        defaults: new { controller = "Users" });

    endpoints.MapControllerRoute(
        name: "clubs",
        pattern: "api/Clubs/{action=Get}/{id?}",
        defaults: new { controller = "Clubs" });

    endpoints.MapControllerRoute(
        name: "weather",
        pattern: "WeatherForecast/{id?}",
        defaults: new { controller = "WeatherForecast", action = "Get" });

    endpoints.MapControllerRoute(
        name: "teams",
        pattern: "api/Teams/{action=GetTeams}/{id?}",
        defaults: new { controller = "Teams" });

    endpoints.MapControllerRoute(
        name: "drawGroups",
        pattern: "Teams/DrawGroups",
        defaults: new { controller = "Teams", action = "DrawGroups" });

    endpoints.MapControllerRoute(
        name: "chooseTeam",
        pattern: "Teams/ChooseTeam",
        defaults: new { controller = "Teams", action = "ChooseTeam" });
});

app.Run();
