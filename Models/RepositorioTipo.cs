using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;


public class RepositorioTipo
{
    private readonly string _connectionString;

    public RepositorioTipo(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Tipo> ObtenerTodos()
    {
        var tipos = new List<Tipo>();

        using (var connection = new MySqlConnection(_connectionString))
        {
            var query = "SELECT Id, Descripcion FROM tipo";
            using (var command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tipos.Add(new Tipo
                    {
                        Id = reader.GetInt32("Id"),
                        Descripcion = reader.GetString("Descripcion")
                    });
                }
                connection.Close();
            }
        }

        return tipos;
    }
}