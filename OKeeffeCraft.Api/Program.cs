using OKeeffeCraft.Authorization;
using OKeeffeCraft.Authorization.Interfaces;
using OKeeffeCraft.Authorization.Services;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Core.Services;
using OKeeffeCraft.Database;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.ExternalServiceProviders.Services;
using OKeeffeCraft.Helpers;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
var services = builder.Services;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var env = builder.Environment;
services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://kevokeeffe.ie", "https://www.kevokeeffe.ie", "http://localhost:4200", "https://postmarkapp.com", "https://api.postmarkapp.com")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                      });
});

services.AddControllers().AddJsonOptions(x =>
{
    // serialize enums as strings in api responses (e.g. Role)
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddSwaggerGen();

// configure strongly typed settings object

services.AddSingleton<MongoDataContext>();
services.AddHttpContextAccessor();

// configure DI for application services scoped
services.AddScoped<IJwtUtils, JwtUtils>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<ILogService, LogService>();
services.AddScoped<IChatGptService, ChatGptService>();
services.AddScoped<IContactMessageService, ContactMessageService>();
services.AddScoped<IAMLAssistantService, AMLAssistantService>();
services.AddScoped<IMongoDBService, MongoDBService>();
services.AddScoped<IPostmarkEmailServiceProvider, PostmarkEmailServiceProvider>();
services.AddScoped<IGamesService, GamesService>();

// configure DI for external services transients
services.AddTransient<IAuthIdentityService, AuthIdentityService>();


var app = builder.Build();

// configure HTTP request pipeline
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET Kev O'Keeffe Api"); x.RoutePrefix = string.Empty; });

    // global cors policy
    app.UseCors(MyAllowSpecificOrigins);

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

app.Run();