using Microsoft.EntityFrameworkCore;
using MotoZavodyWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ZavodyContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

//// DbContext – napojení na connection string z appsettings.json
//builder.Services.AddDbContext<ZavodyContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("MotoZavodyConnection")));

var app = builder.Build();

// standardní middleware...
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
