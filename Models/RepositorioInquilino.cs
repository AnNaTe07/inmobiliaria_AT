
using System.ComponentModel.Design;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;

public class RepositorioInquilino
{
    private readonly string _connectionString;

    public RepositorioInquilino(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Inquilino> ObtenerTodos()
    {
        List<Inquilino> inquilino = new List<Inquilino>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT {nameof(Inquilino.Id)},{nameof(Inquilino.Nombre)},{nameof(Inquilino.Apellido)}, {nameof(Inquilino.Documento)},{nameof(Inquilino.Telefono)},{nameof(Inquilino.Email)} FROM inquilino;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        inquilino.Add(new Inquilino
                        {
                            Id = reader.GetInt32(nameof(Inquilino.Id)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Documento = reader.GetString(nameof(Inquilino.Documento)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                        });
                    }
                    connection.Close();
                }
                return inquilino;
            }
        }
    }

    public Inquilino? ObtenerPorId(int id)
    {
        Inquilino? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT {nameof(Inquilino.Id)},{nameof(Inquilino.Nombre)},{nameof(Inquilino.Apellido)}, {nameof(Inquilino.Documento)},{nameof(Inquilino.Telefono)},{nameof(Inquilino.Email)} FROM inquilino WHERE {nameof(Inquilino.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Inquilino
                    {
                        Id = reader.GetInt32(nameof(Inquilino.Id)),
                        Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                        Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                        Documento = reader.GetString(nameof(Inquilino.Documento)),
                        Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                        Email = reader.GetString(nameof(Inquilino.Email)),
                    };
                }
                connection.Close();
            }

            return res;
        }
    }

    public int Alta(Inquilino inquilino)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query=$@"INSERT INTO inquilino ({nameof(Inquilino.Nombre)},{nameof(Inquilino.Apellido)}, {nameof(Inquilino.Documento)},{nameof(Inquilino.Telefono)},{nameof(Inquilino.Email)}) VALUES (@nombre,@apellido,@documento,@telefono,@email);
            SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                command.Parameters.AddWithValue("@documento", inquilino.Documento);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }

    public int Modificar(Inquilino inquilino)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query =$@"UPDATE inquilino SET {nameof(Inquilino.Nombre)} = @nombre, {nameof(Inquilino.Apellido)} = @apellido, {nameof(Inquilino.Documento)} = @documento,{nameof(Inquilino.Telefono)} = @telefono, {nameof(Inquilino.Email)} = @email WHERE {nameof(Inquilino.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", inquilino.Id);
                command.Parameters.AddWithValue("@nombre", inquilino.Nombre);   
                command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                command.Parameters.AddWithValue("@documento", inquilino.Documento);
                command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                command.Parameters.AddWithValue("@email", inquilino.Email);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"DELETE FROM inquilino WHERE {nameof(Inquilino.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
}