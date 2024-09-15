using System.ComponentModel.DataAnnotations;

namespace inmobiliaria_AT.Models
{

    public enum Roles
    {
        Administrador = 1,
        Empleado = 2,
    }

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
        //[NotMapped]//Para EF
        public IFormFile? AvatarFile { get; set; }// para que pueda ser nulo
                                                  //[NotMapped]//Para EF
                                                  //public byte[] AvatarFileContent { get; set; }
                                                  //[NotMapped]//Para EF
                                                  //public string AvatarFileName { get; set; }  
        [Required(ErrorMessage = "El rol es obligatorio.")]
        public Roles Rol { get; set; }
        public string RolNombre => Rol.ToString();
        // Propiedad auxiliar para gestionar la validación condicional
        public bool Estado { get; set; }//para dar de baja el usuario
    }
}