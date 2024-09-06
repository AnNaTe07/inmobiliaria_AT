using Microsoft.AspNetCore.Mvc;
using inmobiliaria_AT.Models;
using inmobiliaria_AT.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace inmobiliaria_AT.Controllers
{
    public class AccountController : Controller
    {
        private readonly RepositorioUsuario _repositorioUsuario;

        public AccountController(RepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /*     [HttpPost]
            public IActionResult Login(Login model)
            {
                if (ModelState.IsValid)
                {
                    var usuario = _repositorioUsuario.ObtenerPorEmail(model.Usuario);
                    if (usuario != null && PasswordUtils.VerifyPassword(model.Password, usuario.PasswordHash, usuario.Salt))
                    {
                        // Código para autenticar al usuario
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Inicio de sesión inválido");
                }
                return View(model);
            }
     */
        [HttpGet]
        public IActionResult Register()//Es una interfaz que representa el resultado de una acción del controlador en ASP.NET Core. Permite devolver diferentes tipos de respuestas HTTP, como vistas, redirecciones, o contenido JSON.
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Login model)
        {
            if (ModelState.IsValid)
            {
                var (passwordHash, salt) = PasswordUtils.HashPassword(model.Password);
                var usuario = new Usuario
                {
                    Email = model.Usuario,
                    PasswordHash = passwordHash,
                    Salt = salt,
                    // Inicializa otros campos si es necesario
                };
                _repositorioUsuario.Alta(usuario);
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);// HttpContext.SignOutAsync se utiliza para cerrar la sesión del usuario
            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<IActionResult> Login(Login model)//Es un tipo que representa una operación asincrónica. Los métodos que devuelven Task permiten que el proceso se ejecute en segundo plano, sin bloquear el hilo principal. Esto es útil para operaciones que pueden tardar un tiempo en completarse, como llamadas a bases de datos o operaciones de red
        {
            if (ModelState.IsValid)
            {
                var usuario = _repositorioUsuario.ObtenerPorEmail(model.Usuario);
                if (usuario != null && PasswordUtils.VerifyPassword(model.Password, usuario.PasswordHash, usuario.Salt))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                // Agrega otros claims si es necesario
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        // Configura propiedades adicionales si es necesario
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Inicio de sesión inválido");
            }
            return View(model);
        }

    }
}