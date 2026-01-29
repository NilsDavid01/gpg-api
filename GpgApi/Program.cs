using GpgApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<GpgService>();

var app = builder.Build();

app.MapControllers();
app.Run();
