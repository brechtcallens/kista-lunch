using System.Reflection;
using System.IO;
using Microsoft.EntityFrameworkCore;
using TheMostAmazingLunchAPI.Data;
using TheMostAmazingLunchAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Kista's Most Amazing Lunch API",
        Description = "Provides interfaces for querying lunch menus and restaurants in Kista.",
        Contact = new OpenApiContact
        {
            Name = "Brecht Callens",
            Email = "brecht.callens@ericsson.com"
        }
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlite<LunchContext>("Data source=LunchData.db");

builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<MenuService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.CreateDbIfNotExists();
app.Run();
