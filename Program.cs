using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using inmobiliaria_AT.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión y verificar si no es null
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada.");

// Configure services
builder.Services.AddScoped<RepositorioPropietario>(provider => new RepositorioPropietario(connectionString));
builder.Services.AddScoped<RepositorioInquilino>(provider => new RepositorioInquilino(connectionString));

builder.Services.AddScoped<RepositorioContrato>(provider => new RepositorioContrato(connectionString));
builder.Services.AddScoped<RepositorioTipo>(provider => new RepositorioTipo(connectionString));

builder.Services.AddScoped<RepositorioInmueble>(provider => new RepositorioInmueble(connectionString));
builder.Services.AddScoped<RepositorioUsuario>(provider => new RepositorioUsuario(connectionString));

// agrego servicios de autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Add controllers with views
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuario", action = "Login" });
app.Run();