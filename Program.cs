using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ClubsStoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(ClubsStoreDatabaseSettings)));

builder.Services.AddSingleton<IClubsStoreDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<ClubsStoreDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("ClubsStoreDatabaseSettings:ConnectionString")));

builder.Services.AddScoped<IClubsService, ClubsService>();
builder.Services.AddScoped<IUserService, UserService>();

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
        pattern: "api/Teams/{action=ChooseTeam}/{id?}",
        defaults: new { controller = "Teams" });
});

app.Run();
