using System.Text.Json.Serialization;
using App;
using App.Data;
using Auctions.Domain;
using Auctions.Json;
using Auctions.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opts.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
    opts.JsonSerializerOptions.Converters.Add(new AmountConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.MapType(typeof(Amount), () => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString(Amount.Zero(CurrencyCode.VAC).ToString())
    });
});
builder.Services.AddSingleton<ITime, Time>();
builder.Services.AddDbContext<AuctionDbContext>(e=>e.UseSqlServer());
if (builder.Configuration["Authentication:Method"]?.Equals( "jwt", StringComparison.OrdinalIgnoreCase)??false)
{
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Authentication:Authority"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.ValidAudiences = new List<string>
                {builder.Configuration["Authentication:ApiName"]!};
        });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
