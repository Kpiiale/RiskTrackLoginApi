using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RiskTrackLoginApi.Consumers;
using RiskTrackLoginApi.Data;
using RiskTrackLoginApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IEmailService, EmailService>(); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuthenticationCodeConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rabbitMqConfig["HostName"], "/", h =>
        {
            h.Username(rabbitMqConfig["UserName"]);
            h.Password(rabbitMqConfig["Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwt = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]))
    };
});

builder.Services.AddAuthorization(); 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
