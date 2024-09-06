using System.ComponentModel.DataAnnotations;


namespace inmobiliaria_AT.Models
{
    public class Login
    {
        [DataType(DataType.EmailAddress)]
        public string Usuario { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}