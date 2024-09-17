using System.ComponentModel.DataAnnotations; // Necesario para [Required] y [EmailAddress]
using Microsoft.AspNetCore.Http; // Necesario para IFormFile (para el avatar)

namespace inmobiliaria_AT.Models;


public enum Rol
{
    Administrador = 1,
    Empleado = 2,
}

public class Usuario
{
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellido { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string PasswordHash { get; set; }
    public string Salt { get; set; } = "";
    public string Avatar { get; set; } = "";
    public IFormFile? AvatarFile { get; set; }// para que pueda ser nulo
    public Rol Rol { get; set; }
    public string RolNombre => Rol.ToString();

    public string NombreCompleto => $"{Nombre} {Apellido}";

    /* 
        public static string ObtenerRol(int rolValue)
        {
            // Verifica si el valor del rol está en el rango válido de la enumeración
            if (Enum.IsDefined(typeof(Rol), rolValue))
            {
                // Convierte el valor entero a la descripción del rol
                Rol rol = (Rol)rolValue;
                return rol.ToString();
            }
            else
            {
                // Retorna un valor predeterminado o un mensaje de error si el valor no es válido
                return "Rol no válido";
            }
        } */
}