using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.Infraestructure.Persistencia;
using ProyectoBDII.Infraestructure.Security;
using ProyectoBDII.Settings.JwSettings;
using ProyectoBDII.Settings.MongoSettings;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDbSetting>(
    builder.Configuration.GetSection("MongoDbSettings"));//Referencia al apartado de la cadena de conexion



builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var setting = sp.GetRequiredService<IOptions<MongoDbSetting>>().Value;
    return new MongoClient(setting.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<MongoDbSetting>>().Value;
        var client  = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(settings.DatabaseName);


    }
    );

// JWT settings
builder.Services.Configure<JwSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwSettings>();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();



//Inyeccion de Servicios
builder.Services.AddScoped<IUsuarioRepository,UserRepository>();
builder.Services.AddScoped<IPasswordHash,PasswordHasherService>();
builder.Services.AddScoped<IJwToken, JwTokens>();
builder.Services.AddScoped<ICategoriasRepository, CategoriasRepositories>();
builder.Services.AddScoped<IPublicacionesRepository, PublicacionesRepositories>();


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoriasService>();




builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var adminInit = new Usuario()
    {
        Email = "imosa@admin.com",
      
        Name = "Jonathan",
        LastName = "Orantes",
        Phone = 78036808.ToString(),
        Role = "admin",
        City ="San Salvador",
        Country = "San Salvador",
        Department = "San Salvador"
    };

        var userRepository = scope.ServiceProvider.GetRequiredService<UserService>();

        
        var result = userRepository.RegisterAsync(adminInit, "Admin123!");
            
}

app.Run();
