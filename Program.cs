using DockerDemo.Data;
using DockerDemo.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// for serilog
Log.Logger  = new LoggerConfiguration()
    //.WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
//adding authenication in system with azure active directory
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        //options.TokenValidationParameters = new TokenValidationParameters
        //{
        //    // Add both the Client ID and the App ID URI here
        //    ValidateAudience = false,
        //    ValidAudiences = new[]
        //    {
        //        builder.Configuration["AzureAd:ClientId"],
        //        builder.Configuration["AzureAd:ClientIds"]
        //    },
        //    ValidateIssuer = false,
        // Used this logic so that any version is allowed
        //    ValidIssuers = new[]
        //    {
        //        $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0",
        //        $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0/", // With slash
        //        $"https://sts.windows.net/{builder.Configuration["AzureAd:TenantId"]}/" // The v1 version just in case
        //    }
        //};
        options.Audience = builder.Configuration["AzureAd:ClientId"];
        //not sure about aud i am expecting aud with api as prefi but the micorsoft entra id is seding my with guid
        //to match i have to update clientIds
    });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//integration of db in the application
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
        // Enables automatic retries for transient failures
        sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,           // Maximum number of retry attempts
        maxRetryDelay: TimeSpan.FromSeconds(30), // Max delay between retries
        errorNumbersToAdd: null     // Additional SQL error codes to retry on
    );
    }));

builder.Services.AddControllers();

//This registers authorization services in the dependency injection container.
builder.Services.AddAuthorization();

//to make git detect changes

// swagger service 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// to add all the DI 
builder.Services.AddApplicationServices();

var app = builder.Build();

//This middleware checks and validates the token in the request.
app.UseAuthentication();

//This middleware enforces the [Authorize] attribute. 
app.UseAuthorization();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(options =>
{
   // Points to the generated JSON file
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");

  // This makes Swagger available at the root URL (e.g., http://localhost:5062/)
   options.RoutePrefix = string.Empty;
});
//}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//   app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    db.Database.Migrate(); // creates tables if they don't exist
//}

app.MapControllers();
app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
