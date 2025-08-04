using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using DotNetEnv;

// Load .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings from environment variables
builder.Services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb://localhost:27017";
    options.DatabaseName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "collecthub";
    
    // Add logging to debug
    Console.WriteLine($"MongoDB ConnectionString: {options.ConnectionString}");
    Console.WriteLine($"MongoDB DatabaseName: {options.DatabaseName}");
});

// Register services
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<GameService>(); // Add this line

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Test MongoDB connection on startup
try
{
    var mongoService = app.Services.GetRequiredService<MongoDbService>();
    var testCollection = mongoService.GetCollection<object>("test");
    Console.WriteLine("✅ MongoDB connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
}

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();