
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using inmobiliaria_AT.Models;
using Org.BouncyCastle.Crypto.Engines;
using PasswordUtils = inmobiliaria_AT.Utils.PasswordUtils;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace inmobiliaria_AT.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly RepositorioUsuario _repositorioUsuario;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UsuarioController(ILogger<UsuarioController> logger, RepositorioUsuario repositorioUsuario, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _repositorioUsuario = repositorioUsuario;
            _hostingEnvironment = hostingEnvironment;
        }

        //GET: Usuario/Index
        public IActionResult Index()
        {
            var usuarios = _repositorioUsuario.ObtenerTodos();
            return View(usuarios);
        }

        //GET: Usuario/Detalle/id
        public IActionResult Detalle(int id)
        {
            var usuario = _repositorioUsuario.ObtenerPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }
        //GET: Usuario/Crear
        [HttpGet]
        public IActionResult Crear()
        {
            try
            {
                var roles = Enum.GetValues(typeof(Rol)).Cast<Rol>().Select(x => new { Id = (int)x, Nombre = x.ToString() }).ToList();
                ViewBag.Roles = roles;
                //_logger.LogInformation("GET Crear - Roles cargados.");
                return View();
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Error en GET Crear: {ex.Message}");
                throw;
            }
        }
        //POST: Usuario/Crear
        [HttpPost]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Hash del password
                    var result = PasswordUtils.HashPassword(usuario.PasswordHash);
                    var hashedPassword = result.hashedPassword;
                    var salt = result.salt;

                    var user = new Usuario
                    {
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Email = usuario.Email,
                        PasswordHash = hashedPassword,
                        Salt = salt,
                        Rol = usuario.Rol
                    };

                    // Log de los detalles del usuario
                    // _logger.LogInformation("Nombre: {Nombre}, Apellido: {Apellido}, Email: {Email}, PasswordHash: {PasswordHash}, Salt: {Salt}, Rol: {Rol}",
                    //  user.Nombre, user.Apellido, user.Email, user.PasswordHash, user.Salt, user.Rol);

                    // Guarda el archivo del avatar si es necesario
                    if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                    {
                        var avatarFilePath = await SaveAvatarFileAsync(usuario.AvatarFile);
                        user.Avatar = avatarFilePath;
                    }

                    int id = _repositorioUsuario.Alta(user);
                    if (id > 0)
                    {
                        TempData["SuccessMessage"] = "Usuario creado exitosamente.";
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError("", "No se pudo dar de alta el usuario");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear el usuario: {ex.Message}";
                    _logger.LogError(ex, "Error al crear el usuario");
                }
            }
            else
            {
                // Log errors in ModelState
                foreach (var modelState in ModelState)
                {
                    var errors = modelState.Value.Errors;
                    foreach (var error in errors)
                    {
                        _logger.LogWarning("ModelState error: {0}", error.ErrorMessage);
                    }
                }
            }

            // Popular ViewBag con roles si ModelState no es válido
            ViewBag.Roles = Enum.GetValues(typeof(Rol)).Cast<Rol>().Select(x => new { Id = (int)x, Nombre = x.ToString() }).ToList();
            return View(usuario);
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

            var model = new Usuario
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Avatar = usuario.Avatar,
                Rol = usuario.Rol
            };

            ViewBag.Roles = Enum.GetValues(typeof(Rol)).Cast<Rol>().Select(x => new SelectListItem { Value = ((int)x).ToString(), Text = x.ToString(), Selected = x == usuario.Rol }).ToList();

            return View(model);
        }

        // POST: Usuario/Editar/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var user = _repositorioUsuario.ObtenerPorId(usuario.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Nombre = usuario.Nombre;
                user.Apellido = usuario.Apellido;
                user.Email = usuario.Email;
                user.Rol = usuario.Rol;

                if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
                {
                    var avatarFilePath = await SaveAvatarFileAsync(usuario.AvatarFile);
                    user.Avatar = avatarFilePath;
                }

                _repositorioUsuario.Editar(user);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = Enum.GetValues(typeof(Rol)).Cast<Rol>().Select(x => new SelectListItem { Value = ((int)x).ToString(), Text = x.ToString() }).ToList();
            return View(usuario);
        }

        // POST: Usuario/EliminarAvatar/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAvatar(int id)
        {
            var user = _repositorioUsuario.ObtenerPorId(id);
            if (user == null)
            {
                return NotFound();
            }

            // Borra el avatar físico del servidor
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                System.IO.File.Delete(Path.Combine(_hostingEnvironment.WebRootPath, user.Avatar));
            }

            // Elimina la ruta del avatar en la base de datos
            user.Avatar = null;
            _repositorioUsuario.Editar(user);

            return RedirectToAction(nameof(Editar), new { id = user.Id });
        }


        //POST: Usuario/Eliminar/id
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            _repositorioUsuario.Baja(id);
            return RedirectToAction("Index");
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


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // GET: Usuario/Salir
        [Route("Salir", Name = "Logout")]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
