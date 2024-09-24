using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using inmobiliaria_AT.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión y verificar si no es null
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

// Configure services
builder.Services.AddScoped<RepositorioPropietario>(provider => new RepositorioPropietario(connectionString));
builder.Services.AddScoped<RepositorioInquilino>(provider => new RepositorioInquilino(connectionString));

builder.Services.AddScoped<RepositorioContrato>(provider => new RepositorioContrato(provider.GetRequiredService<ILogger<RepositorioContrato>>(), provider.GetRequiredService<ILogger<RepositorioInmueble>>(), connectionString));
builder.Services.AddScoped<RepositorioTipo>(provider => new RepositorioTipo(connectionString));

builder.Services.AddScoped<RepositorioInmueble>(provider => new RepositorioInmueble(provider.GetRequiredService<ILogger<RepositorioInmueble>>(), // Inyección del logger
        connectionString));
builder.Services.AddScoped<RepositorioUsuario>(provider => new RepositorioUsuario(
        connectionString));


builder.Services.AddScoped<RepositorioPago>(provider => new RepositorioPago(provider.GetRequiredService<ILogger<RepositorioPago>>(), provider.GetRequiredService<ILogger<RepositorioContrato>>(), provider.GetRequiredService<ILogger<RepositorioUsuario>>(), provider.GetRequiredService<ILogger<RepositorioInmueble>>(), provider.GetRequiredService<ILogger<RepositorioConcepto>>(), connectionString));
builder.Services.AddScoped<RepositorioConcepto>(provider => new RepositorioConcepto(provider.GetRequiredService<ILogger<RepositorioConcepto>>(), connectionString));



// agrego servicios de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Usuario/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("AdminEmpleado", policy => policy.RequireRole("Empleado", "Administrador"));
});

var cultureInfo = new CultureInfo("en-US");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// controladores de vista
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Ruta específica para login
    endpoints.MapControllerRoute(
        name: "login",
        pattern: "Account/Login",
        defaults: new { controller = "Usuario", action = "Login" });
});

app.Run();
