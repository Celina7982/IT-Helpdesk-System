using IThelpdesk.Data;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Repositories;
using Microsoft.EntityFrameworkCore;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services(register repository and services)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger to see Minimal APIs
builder.Services.AddSwaggerGen();           // Required for Swagger to see Controllers
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


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

app.UseHttpsRedirection();
app.UseAuthorization();

// 3. Map Endpoints
app.MapGet("/weather", () => new[] { "Sunny", "Cloudy", "Rainy" });
app.MapControllers();

app.Run();