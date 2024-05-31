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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
