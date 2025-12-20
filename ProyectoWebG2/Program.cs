var builder = WebApplication.CreateBuilder(args);

// ============ MVC ============
builder.Services.AddControllersWithViews();

// ============ HTTP CLIENT A LA API ============
var apiBaseUrl = builder.Configuration["Valores:UrlAPI"];
if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    apiBaseUrl = "https://localhost:7238/";
}

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// ============ SESSION ============
builder.Services.AddDistributedMemoryCache(); // ✅
builder.Services.AddSession();

var app = builder.Build();

// ============ PIPELINE ============
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/MostrarError");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();       // ✅ (después de routing)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
