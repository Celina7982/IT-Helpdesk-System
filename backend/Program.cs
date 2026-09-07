using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.Repositories;
using IThelpdesk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using IThelpdesk.Services.Pdf;
// Add this using near the other service usings




using QuestPDF.Infrastructure;

//using Microsoft.OpenApi;


using System.Text;



var builder = WebApplication.CreateBuilder(args);

// 1. Add Services(register repository and services)
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to see Minimal APIs



builder.Services.AddSwaggerGen();
//(options =>
//{
//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Enter your JWT token."
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

// Register the User repository and service with the Dependency Injection container.
// This decouples controllers from concrete implementations and improves testability.  

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IJobCardService, JobCardService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IJobCardRepository, JobCardRepository>();
builder.Services.AddScoped<IJobCardAuditRepository, JobCardAuditRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


builder.Services.AddScoped<IJobCardAuditService, JobCardAuditService>();
builder.Services.AddScoped<IJobCardPdfService, JobCardPdfService>();


/*
 When AuthController asks for an IAuthService



Dependency Injection container: "Whenever someone requests IAuthService, create an AuthService."

very NB line b/c without it,when you call the login endpoint you'll get a runtime error
*/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine("AUTH HEADER:");
                Console.WriteLine(context.Request.Headers.Authorization);
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("AUTH FAILED:");
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                Console.WriteLine("JWT CHALLENGE");
                return Task.CompletedTask;
            }
        };
    });








//SQL connection 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            )
            .CommandTimeout(60)
    )
);



QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        if (!context.Users.Any(u => u.Email == "admin@ithelpdesk.com"))
        {
            context.Users.Add(new User
            {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@ithelpdesk.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                Role = "Admin",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });

            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Database Migration Warning]: {ex.Message}");
    }
}


// 2. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Generates the Swagger JSON
    app.UseSwaggerUI(); // Serves the Swagger GUI
}

// app.UseHttpsRedirection(); // Commented out to prevent redirecting to port 7112

app.UseCors("AllowReact");

app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization(); // Add this line to enable authorization middleware

// 3. Map Endpoints

app.MapControllers();

app.MapGet("/weather", () => new[] { "Sunny", "Cloudy", "Rainy" });

app.Run();