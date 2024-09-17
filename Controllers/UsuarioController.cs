using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using inmobiliaria_AT.Models;
using Org.BouncyCastle.Crypto.Engines;
using inmobiliaria_AT.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliaria_AT.Controllers;


public class UsuarioController : Controller
{
    private readonly IConfiguration configuration;
    private readonly IWebHostEnvironment environment;
    private readonly RepositorioUsuario _repositorioUsuario;

    public UsuarioController(IConfiguration configuration, IWebHostEnvironment environment, RepositorioUsuario repositorio)
    {
        this.configuration = configuration;
        this.environment = environment;
        this._repositorioUsuario = repositorio;
    }
    // GET: Usuarios
    [Authorize(Policy = "Administrador")]
    public IActionResult Index()
    {
        var usuarios = _repositorioUsuario.ObtenerTodos();
        return View(usuarios);
    }
    //GET: Usuario/Detalle/id
    [Authorize(Policy = "Administrador")]
    public IActionResult Detalle(int id)
    {
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        return View(usuario);
    }

    // GET: Usuarios/Crear
    [Authorize(Policy = "Administrador")]
    public ActionResult Crear()
    {
        ViewBag.Roles = Enum.GetValues(typeof(Roles))
                    .Cast<Roles>()
                    .Select(x => new SelectListItem
                    {
                        Value = ((int)x).ToString(),
                        Text = x.ToString()
                    })
                    .ToList();

        return View();
    }

    // POST: Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Administrador")]
    public async Task<IActionResult> Crear(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            foreach (var state in ModelState)
            {
                // Console.WriteLine($"Key: {state.Key}");
                foreach (var error in state.Value.Errors)
                {
                    //Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            ViewBag.Roles = Enum.GetValues(typeof(Roles))
                      .Cast<Roles>()
                      .Select(x => new SelectListItem
                      {
                          Value = ((int)x).ToString(),
                          Text = x.ToString()
                      })
                      .ToList();
            return View(usuario);
        }

        try
        {
            if (string.IsNullOrEmpty(usuario.Clave))
            {
                ModelState.AddModelError("PasswordHash", "El campo Password es obligatorio.");
                ViewBag.Roles = Enum.GetValues(typeof(Roles))
                     .Cast<Roles>()
                     .Select(x => new SelectListItem
                     {
                         Value = ((int)x).ToString(),
                         Text = x.ToString()
                     })
                     .ToList();
                return View(usuario);
            }

            var result = PasswordUtils.HashPassword(usuario.Clave);
            var hashedPassword = result.hashedPassword;
            var salt = result.salt;

            var user = new Usuario
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Clave = hashedPassword,
                Salt = salt,
                Rol = usuario.Rol,
                Estado = true
            };

            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                var avatarFilePath = await SaveAvatarFileAsync(usuario.AvatarFile);
                user.Avatar = avatarFilePath;
            }

            int id = _repositorioUsuario.Alta(user);
            //  Console.WriteLine($"ID retornado por Alta: {id}");

            if (id > 0)
            {
                TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "No se pudo dar de alta el usuario");
            // Console.WriteLine("No se pudo dar de alta el usuario");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al crear el usuario: {ex.Message}";
            // Console.WriteLine($"Error: {ex.Message}");
        }

        ViewBag.Roles = Enum.GetValues(typeof(Roles))
                      .Cast<Roles>()
                      .Select(x => new SelectListItem
                      {
                          Value = ((int)x).ToString(),
                          Text = x.ToString()
                      })
                      .ToList();

        return View(usuario);
    }



    //metodo para guardar el archivo del avatar
    private async Task<string> SaveAvatarFileAsync(IFormFile avatarFile)
    {
        if (avatarFile == null || avatarFile.Length == 0)
        {
            return string.Empty;
        }

        var fileName = Path.GetFileNameWithoutExtension(avatarFile.FileName);
        var extension = Path.GetExtension(avatarFile.FileName);
        var uniqueFileName = $"{fileName}-{Guid.NewGuid()}{extension}";
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/avatars");
        var filePath = Path.Combine(uploads, uniqueFileName);

        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await avatarFile.CopyToAsync(fileStream);
        }

        return $"/images/avatars/{uniqueFileName}"; // Ruta del archivo que se almacena en la base de datos
    }


    // GET: Usuario/Editar/id
    [HttpGet]
    public IActionResult Editar(int id)
    {
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        // Obtengo el ID del usuario actual
        var currentUserId = HttpContext.User.FindFirst("UserId")?.Value;
        var isAdmin = HttpContext.User.IsInRole("Administrador");

        // Verifica si el usuario actual es el propietario del perfil o un administrador
        if (usuario.Id.ToString() != currentUserId && !isAdmin)
        {
            return Forbid(); // O redirige a una página de acceso denegado
        }

        // Creo un modelo para la vista de edición
        var model = new Usuario
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            Avatar = usuario.Avatar,
            Rol = usuario.Rol
        };

        // Obtengo los roles para el ViewBag
        ViewBag.Roles = Enum.GetValues(typeof(Roles))
            .Cast<Roles>()
            .Select(x => new SelectListItem
            {
                Value = ((int)x).ToString(),
                Text = x.ToString(),
                Selected = x == (Roles)usuario.Rol
            })
            .ToList();

        return View(model);
    }



    // POST: Usuario/Editar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(Usuario usuario, IFormFile? avatarFile)
    {
        // Console.WriteLine("Inicio del método Editar.");
        if (!ModelState.IsValid)
        {
            //Console.WriteLine("Modelo no válido.");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                //Console.WriteLine(error.ErrorMessage);
            }
        }
        if (ModelState.IsValid)
        {
            var user = _repositorioUsuario.ObtenerPorId(usuario.Id);
            if (user == null)
            {
                Console.WriteLine($"Usuario con ID {usuario.Id} no encontrado.");
                return NotFound();
            }

            // Obtengo el ID del usuario actual
            var currentUserId = HttpContext.User.FindFirst("UserId")?.Value;
            var isAdmin = HttpContext.User.IsInRole("Administrador");

            // Verifica si el usuario actual es el propietario del perfil o un administrador
            if (user.Id.ToString() != currentUserId && !isAdmin)
            {
                return Forbid(); // O redirige a una página de acceso denegado
            }

            // Actualizo los datos del usuario
            user.Nombre = usuario.Nombre;
            user.Apellido = usuario.Apellido;
            user.Email = usuario.Email;
            // user.Rol = usuario.Rol;
            // Antes de actualizar
            //Console.WriteLine($"Salt actual antes de la actualización: {user.Salt}");
            // Solo actualizo la contraseña si hay una nueva
            if (!string.IsNullOrEmpty(usuario.Clave))
            {
                var result = PasswordUtils.HashPassword(usuario.Clave);
                user.Clave = result.hashedPassword; // Guardar el hash de la nueva contraseña
                user.Salt = result.salt; // Guardar el salt para la nueva contraseña
                //Console.WriteLine("Contraseña actualizada.");
                //Console.WriteLine($"Nuevo salt después de la actualización: {result.salt}");
            }
            else
            {/*
                  user.Salt = user.Salt;
                  Console.WriteLine("No se proporcionó una nueva contraseña, el salt se mantiene igual.");
              } */
                //Console.WriteLine($"Salt mantenido después de la actualización: {user.Salt}");
            }

            // Manejo del archivo del avatar
            if (avatarFile != null && avatarFile.Length > 0)
            {
                Console.WriteLine("Archivo de avatar detectado, guardando...");
                // Guardo el nuevo avatar y actualizo la ruta en el usuario
                var avatarPath = await SaveAvatarFileAsync(avatarFile);
                user.Avatar = avatarPath;
                // Console.WriteLine($"Nuevo avatar guardado en {avatarPath}.");
            }

            // Solo el administrador puede cambiar el rol
            if (User.IsInRole("Administrador"))
            {
                user.Rol = usuario.Rol; // cambia el rol solo si el usuario es administrador
            }

            // Guardo los cambios en el repositorio
            _repositorioUsuario.Editar(user);
            // Console.WriteLine("Usuario actualizado exitosamente.");

            TempData["SuccessMessage"] = "Datos de Usuario actualizado correctamente.";
            // Redirigir según el rol del usuario
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "Usuario");
            }
            else if (User.IsInRole("Empleado"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // Si el modelo no es válido, volver a cargar los roles para la vista
        ViewBag.Roles = Enum.GetValues(typeof(Roles))
            .Cast<Roles>()
            .Select(x => new SelectListItem
            {
                Value = ((int)x).ToString(),
                Text = x.ToString()
            })
            .ToList();
        Console.WriteLine("El modelo no es válido. Volviendo a la vista de edición.");
        return View(usuario);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarAvatar(int id)
    {
        var usuario = _repositorioUsuario.ObtenerPorId(id);
        if (usuario == null)
        {
            return NotFound();
        }

        // Eliminar el archivo de avatar si existe
        if (!string.IsNullOrEmpty(usuario.Avatar))
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", usuario.Avatar.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // Limpiar la propiedad Avatar en el modelo
        usuario.Avatar = null;

        // Actualizar el usuario en la base de datos
        _repositorioUsuario.Editar(usuario);

        TempData["SuccessMessage"] = "Avatar eliminado correctamente.";
        return RedirectToAction(nameof(Editar), new { id = usuario.Id });
    }


    // GET: Usuarios/Delete/5
    //  [Authorize(Policy = "Administrador")]
    /*  public ActionResult Delete(int id)
     {
         return View();
     } */

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Administrador")]
    //POST: Usuario/Eliminar/id

    public IActionResult Eliminar(int id)
    {
        _repositorioUsuario.Baja(id);
        return RedirectToAction("Index");
    }



    [HttpGet]
    public IActionResult Login()
    {
        return View("~/Views/Usuario/Login.cshtml");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);// HttpContext.SignOutAsync se utiliza para cerrar la sesión del usuario
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login model)//Es un tipo que representa una operación asincrónica. Los métodos que devuelven Task permiten que el proceso se ejecute en segundo plano, sin bloquear el hilo principal. Esto es útil para operaciones que pueden tardar un tiempo en completarse, como llamadas a bases de datos o operaciones de red
    {
        // Verifico si el modelo es válido
        if (ModelState.IsValid)
        {
            // Recupero el usuario de la base de datos usando el email proporcionado
            var usuario = _repositorioUsuario.ObtenerPorEmail(model.Usuario);

            // Verifica si el usuario existe
            if (usuario != null)
            {
                //verifico valores de entrada y almacenados
                Console.WriteLine($"Input Password: {model.Password}");
                Console.WriteLine($"Stored Hashed Password: {usuario.Clave}");
                Console.WriteLine($"Stored Salt: {usuario.Salt}");

                // Verifica la contraseña usando el método de Utils/PasswordUtils
                if (PasswordUtils.VerifyPassword(model.Password, usuario.Clave, usuario.Salt))
                {
                    // Creo una lista de claims para la autenticación
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.RolNombre),
                    new Claim("UserId", usuario.Id.ToString())
                };

                    // Crea una identidad de claims
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Configura las propiedades de autenticación
                    var authProperties = new AuthenticationProperties
                    {
                        // Configura propiedades adicionales si es necesario
                    };

                    // Inicia sesión con los claims creados
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirige a la página principal después del inicio de sesión exitoso
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Mensaje de depuración si la verificación falla
                    Console.WriteLine("Inicio de sesión inválido: contraseña incorrecta.");
                }
            }
            else
            {
                // Mensaje de depuración si el usuario no existe
                Console.WriteLine("Inicio de sesión inválido: usuario no encontrado.");
            }

            // Agrega un error al modelo para mostrar en la vista
            ModelState.AddModelError("", "Inicio de sesión inválido");
        }

        // Devuelve la vista del login con el modelo para mostrar errores
        return View("~/Views/Usuario/Login.cshtml", model);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}

