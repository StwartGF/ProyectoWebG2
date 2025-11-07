var builder = WebApplication.CreateBuilder(args);

// MVC (Views + Controllers)
builder.Services.AddControllersWithViews();

// HttpClient para consumir la API
builder.Services.AddHttpClient();

// Session para mantener datos del usuario logueado
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        // <- importante: antes de MapControllerRoute
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
