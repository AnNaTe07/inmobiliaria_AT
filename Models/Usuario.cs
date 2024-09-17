using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_AT.Models
{

    public enum Roles
    {
        Administrador = 1,
        Empleado = 2,
    }


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
    public class Usuario
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "La contraseña es obligatoria.")]
        //[DataType(DataType.Password)]
        public string? Clave { get; set; }
        public string Salt { get; set; } = "";
        public string Avatar { get; set; } = "";

        public IFormFile? AvatarFile { get; set; }
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public Roles Rol { get; set; }
        public string RolNombre => Rol.ToString();

        public string NombreCompleto => $"{Nombre} {Apellido}";
        public bool Estado { get; set; }//para dar de baja el usuario

    }
}