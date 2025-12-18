var builder = WebApplication.CreateBuilder(args);

// ============ MVC ============
builder.Services.AddControllersWithViews();

// ============ HTTP CLIENT A LA API ============
var apiBaseUrl = builder.Configuration["Valores:UrlAPI"];

// Si en appsettings no está configurado, ponemos como fallback la URL local del API
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    // Asegúrate de que coincida con el puerto HTTPS de tu API
    apiBaseUrl = "https://localhost:7238/";
}

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// ============ SESSION ============
builder.Services.AddSession();

var app = builder.Build();

// ============ PIPELINE ============

// Manejo de errores / HSTS según ambiente
if (!app.Environment.IsDevelopment())
{
    // En producción: página de error amigable y HSTS
    app.UseExceptionHandler("/Error/MostrarError");
    app.UseHsts();
}
else
{
    // En desarrollo puedes ver errores detallados
    app.UseDeveloperExceptionPage();
}

// Orden recomendado
app.UseHttpsRedirection();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

// Archivos estáticos (según el template nuevo de .NET)
app.MapStaticAssets();

// Ruta por defecto
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();

