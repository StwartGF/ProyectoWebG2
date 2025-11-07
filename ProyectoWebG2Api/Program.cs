var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(); // swagger minimal

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler("/api/Error/RegistrarError");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
