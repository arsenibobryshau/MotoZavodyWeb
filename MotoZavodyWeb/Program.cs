using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

// Nastavení CS na webu
var cultureInfo = new CultureInfo("cs-CZ");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// MVC
builder.Services.AddControllersWithViews();

// DB Context - Oracle
builder.Services.AddDbContext<ZavodyContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Session (pro login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// standardní middleware...
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing musí být pøed session
app.UseRouting();

// Session musí být PØED Authorization
app.UseSession();

app.UseAuthorization();

// redirect "/"
app.MapGet("/", context =>
{
    context.Response.Redirect("/Home/Index");
    return Task.CompletedTask;
});

// defaultní routování
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
