using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

using Microsoft.OpenApi;
using MongoDB.Driver;
using ProyectoBDII.Application.Service;
using ProyectoBDII.Application.Service.CassandraService;
using ProyectoBDII.Domain.Interface;
using ProyectoBDII.Domain.Interface.Cassandra_Interfaces;
using ProyectoBDII.Domain.Settings;
using ProyectoBDII.Infraestructure;
using ProyectoBDII.Infraestructure.Persistencia;
using ProyectoBDII.Infraestructure.Security;
using ProyectoBDII.Settings.JwSettings;
using ProyectoBDII.Settings.MongoSettings;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<MongoDbSetting>(
    builder.Configuration.GetSection("MongoDbSettings"));//Referencia al apartado de la cadena de conexion

builder.Services.Configure<CassandraSettings>(
    builder.Configuration.GetSection("Cassandra"));

builder.Services.AddSingleton<CassandraContext>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



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
builder.Services.AddScoped<IUsuarioRepository,UserRepository>();
builder.Services.AddScoped<IPasswordHash,PasswordHasherService>();
builder.Services.AddScoped<IJwToken, JwTokens>();
builder.Services.AddScoped<ICategoriasRepository, CategoriasRepositories>();
builder.Services.AddScoped<IPublicacionesRepository, PublicacionesRepositories>();
builder.Services.AddScoped<IMessageRepository,MessageRepository>();
builder.Services.AddScoped<ILoginHistorialRepository, HistorialLoginRepository>();



builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CategoriasService>();
builder.Services.AddScoped<PublicacionService>();
builder.Services.AddScoped<CassandraMensajeService>();
builder.Services.AddScoped<HistorialesServicio>();


builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Issuer"],
            ValidAudience = builder.Configuration["Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Key"])),
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();




//Inyeccion de Servicios






builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Marketplace API",
        Version = "v1"
    });


    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });



});



var app = builder.Build();
app.MapOpenApi(); //Activa Swagger
app.UseSwagger();
app.UseSwaggerUI();


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

        
        var result = userRepository.RegisterAsync(adminInit, "Admin123!");         }

app.Run();
