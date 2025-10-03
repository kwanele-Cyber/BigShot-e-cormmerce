using BigShotApi.Authentication;
using BigShotApi.Infrastructure;
using BigShotCore.Data.Services;
using BigShotCore.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BigShot API", Version = "v1" });

    // Add API Key authentication support
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Add 'X-Api-Key: {your_api_key}' in the header",
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "X-Api-Key",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DatabaseConnection"),
        b => b.MigrationsAssembly("BigShotApi"))); // migrations stay in Web project


builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IChatbotService, ChatbotService>();

//WebBased Services
//builder.Services.AddHttpClient<IChatbotService, OpenAiChatbotService>();

builder.Services.AddHttpClient<IChatbotService, OpenRouterChatbotService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// -------------------- Ensure Database Exists --------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Automatically create the database if it doesn't exist
    db.Database.EnsureCreated();

    // Optional: Automatically apply migrations (if you also want migrations)
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        //do-nothing
    }


    // 🔹 Seed users + products
    DbSeeder.SeedUsers(db);
    DbSeeder.SeedProducts(db);
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();
