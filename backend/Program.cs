using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerUI;
using IThelpdesk.Services;
using IThelpdesk.Repositories;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Data;
using IThelpdesk.Interfaces.Services;




var builder = WebApplication.CreateBuilder(args);

// 1. Add Services(register repository and services)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to see Minimal APIs
builder.Services.AddSwaggerGen();           // Required for Swagger to see Controllers

// Register the User repository and service with the Dependency Injection container.
// This decouples controllers from concrete implementations and improves testability.  
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

/*
 When AuthController asks for an IAuthService

Dependency Injection container: "Whenever someone requests IAuthService, create an AuthService."

very NB line b/c without it,when you call the login endpoint you'll get a runtime error
*/
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITicketService, TicketService>();


builder.Services.AddScoped<ITicketRepository, TicketRepository>();


//authentication method using JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            
            ValidateIssuer = true, // did my app issue it,
            ValidateAudience = true, //was it created for my app
            ValidateLifetime = true,// is it still valid(not expired)
            ValidateIssuerSigningKey = true, //verify token isnt tampered with

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });


// Configure SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 2. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // Generates the Swagger JSON
    app.UseSwaggerUI(); // Serves the Swagger GUI
}

app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS




app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization(); // Add this line to enable authorization middleware

// 3. Map Endpoints
app.MapGet("/weather", () => new[] { "Sunny", "Cloudy", "Rainy" });
app.MapControllers();

app.Run();