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
            var query = "SELECT Id, Descripcion FROM tipo WHERE Activo = TRUE;";
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
    public Tipo? ObtenerPorId(int id)
    {
        Tipo? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $"SELECT Id, Descripcion FROM tipo WHERE Id = {id}";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Tipo
                    {
                        Id = reader.GetInt32("Id"),
                        Descripcion = reader.GetString("Descripcion")
                    };
                }
                connection.Close();
            }
        }

        return res;
    }
    public int Alta(Tipo tipo)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            connection.Open();

            // Verificar si el tipo ya existe
            var query = "SELECT Activo FROM tipo WHERE Descripcion = @descripcion;";
            using (MySqlCommand checkCommand = new MySqlCommand(query, connection))
            {
                checkCommand.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                var result = checkCommand.ExecuteScalar();

                if (result != null)
                {
                    bool activo = Convert.ToBoolean(result);

                    if (activo)
                    {
                        // Si el tipo ya está activo, no hacer nada
                        return 0; // Indicar que el tipo ya existe y está activo
                    }
                    else
                    {
                        // Si el tipo está inactivo, activarlo
                        var updateQuery = "UPDATE tipo SET Activo = TRUE WHERE Descripcion = @descripcion;";
                        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                            res = updateCommand.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    // Si el tipo no existe, crear uno nuevo
                    var insertQuery = "INSERT INTO tipo (Descripcion, Activo) VALUES (@descripcion, TRUE);";
                    using (MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                        res = insertCommand.ExecuteNonQuery();
                    }
                }
            }

            connection.Close();
        }

        return res;
    }


    public int Modificar(Tipo tipo)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"UPDATE tipo SET {nameof(Tipo.Descripcion)} = @descripcion WHERE {nameof(Tipo.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", tipo.Id);
                command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public void Baja(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = "UPDATE tipo SET Activo = FALSE WHERE Id = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

}