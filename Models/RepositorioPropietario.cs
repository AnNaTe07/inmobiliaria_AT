using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;

public class RepositorioPropietario
{
    private readonly string _connectionString;

    public RepositorioPropietario(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Propietario> ObtenerTodos()
    {
        List<Propietario> propietario = new List<Propietario>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT {nameof(Propietario.Id)},{nameof(Propietario.Nombre)},{nameof(Propietario.Apellido)},{nameof(Propietario.Documento)},{nameof(Propietario.Telefono)},{nameof(Propietario.Email)},{nameof(Propietario.Direccion)} FROM propietario;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        propietario.Add(new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Documento = reader.GetString(nameof(Propietario.Documento)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Direccion = reader.GetString(nameof(Propietario.Direccion))
                        });
                    }
                    connection.Close();
                }
                return propietario;
            }
        }
    }

    public Propietario? ObtenerPorId(int id)
    {
        Propietario? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT {nameof(Propietario.Id)},{nameof(Propietario.Nombre)},{nameof(Propietario.Apellido)},{nameof(Propietario.Documento)},{nameof(Propietario.Telefono)},{nameof(Propietario.Email)},{nameof(Propietario.Direccion)} FROM propietario WHERE {nameof(Propietario.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                {
                    if (reader.Read())
                    {
                        res = new Propietario
                        {
                            Id = reader.GetInt32(nameof(Propietario.Id)),
                            Nombre = reader.GetString(nameof(Propietario.Nombre)),
                            Apellido = reader.GetString(nameof(Propietario.Apellido)),
                            Documento = reader.GetString(nameof(Propietario.Documento)),
                            Telefono = reader.GetString(nameof(Propietario.Telefono)),
                            Email = reader.GetString(nameof(Propietario.Email)),
                            Direccion = reader.GetString(nameof(Propietario.Direccion))
                        };
                    }
                    connection.Close();
                }
                return res;
            }
        }
    }
    public int Alta(Propietario propietario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"INSERT INTO propietario ({nameof(Propietario.Nombre)},{nameof(Propietario.Apellido)},{nameof(Propietario.Documento)},{nameof(Propietario.Telefono)},{nameof(Propietario.Email)},{nameof(Propietario.Direccion)}) VALUES (@nombre,@apellido,@documento,@telefono,@email,@direccion);";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@documento", propietario.Documento);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                connection.Open();
                res = command.ExecuteNonQuery();  // Devuelve el número de filas afectadas
                connection.Close();
            }
        }
        return res;
    }
    public int Modificar(Propietario propietario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"UPDATE propietario 
                       SET {nameof(Propietario.Nombre)} = @nombre, 
                           {nameof(Propietario.Apellido)} = @apellido, 
                           {nameof(Propietario.Documento)} = @documento, 
                           {nameof(Propietario.Telefono)} = @telefono, 
                           {nameof(Propietario.Email)} = @email, 
                           {nameof(Propietario.Direccion)} = @direccion 
                       WHERE {nameof(Propietario.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                command.Parameters.AddWithValue("@documento", propietario.Documento);
                command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                command.Parameters.AddWithValue("@email", propietario.Email);
                command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                command.Parameters.AddWithValue("@id", propietario.Id);
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
            var query = $@"DELETE FROM propietario WHERE {nameof(Propietario.Id)} = @id;";
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

    public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Propietario> res = new List<Propietario>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))

        {
            var query = @$"
					SELECT 
                           {nameof(Propietario.Id)} = @id,
                           {nameof(Propietario.Nombre)} = @nombre, 
                           {nameof(Propietario.Apellido)} = @apellido, 
                           {nameof(Propietario.Documento)} = @documento, 
                           {nameof(Propietario.Telefono)} = @telefono, 
                           {nameof(Propietario.Email)} = @email, 
                           {nameof(Propietario.Direccion)} = @direccion 
					FROM 
                           Propietario
					ORDER BY 
                           {nameof(Propietario.Id)} ASC
					OFFSET {(paginaNro - 1) * tamPagina} ROW
					FETCH NEXT {tamPagina} ROWS ONLY
				";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Propietario propietario = new Propietario
                    {
                        Id = reader.GetInt32(nameof(Propietario.Id)),
                        Nombre = reader.GetString(nameof(Propietario.Nombre)),
                        Apellido = reader.GetString(nameof(Propietario.Apellido)),
                        Documento = reader.GetString(nameof(Propietario.Documento)),
                        Telefono = reader.GetString(nameof(Propietario.Telefono)),
                        Email = reader.GetString(nameof(Propietario.Email)),
                        Direccion = reader.GetString(nameof(Propietario.Direccion))
                        // Clave = reader.GetString("Clave"),
                    };
                    res.Add(propietario);
                }
                connection.Close();
            }

        }
        return res;
    }

    public Propietario ObtenerPorEmail(string email)
    {
        Propietario propietario = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))

        {
            string query = @"
                     SELECT 
                           {nameof(Propietario.Id)} = @id,
                           {nameof(Propietario.Nombre)} = @nombre, 
                             {nameof(Propietario.Apellido)} = @apellido, 
                           {nameof(Propietario.Documento)} = @documento, 
                           {nameof(Propietario.Telefono)} = @telefono, 
                           {nameof(Propietario.Email)} = @email, 
                           {nameof(Propietario.Direccion)} = @direccion 
					FROM 
                           Propietario
					WHERE {nameof(Propietario.Email)}=@email";
            using (MySqlCommand command = new MySqlCommand(query, connection))

            {
                command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    propietario = new Propietario
                    {
                        Id = reader.GetInt32(nameof(Propietario.Id)),
                        Nombre = reader.GetString(nameof(Propietario.Nombre)),
                        Apellido = reader.GetString(nameof(Propietario.Apellido)),
                        Documento = reader.GetString(nameof(Propietario.Documento)),
                        Telefono = reader.GetString(nameof(Propietario.Telefono)),
                        Email = reader.GetString(nameof(Propietario.Email)),
                        Direccion = reader.GetString(nameof(Propietario.Direccion))
                    };
                }
                connection.Close();
            }
        }
        return propietario;
    }



    public IList<Propietario> BuscarPorNombre(string nombre)
    {
        List<Propietario> lista = new List<Propietario>();
        Propietario propietario = null;
        nombre = "%" + nombre + "%";
        using (MySqlConnection connection = new MySqlConnection(_connectionString))

        {
            var query = @"
                    SELECT 
                           {nameof(Propietario.Id)} = @id,
                           {nameof(Propietario.Nombre)} = @nombre, 
                           {nameof(Propietario.Apellido)} = @apellido, 
                           {nameof(Propietario.Documento)} = @documento, 
                           {nameof(Propietario.Telefono)} = @telefono, 
                           {nameof(Propietario.Email)} = @email, 
                           {nameof(Propietario.Direccion)} = @direccion 
					FROM 
                           Propietario
					WHERE Nombre LIKE @nombre OR Apellido LIKE @apellido";
            using (MySqlCommand command = new MySqlCommand(query, connection))

            {
                command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    propietario = new Propietario
                    {
                        Id = reader.GetInt32(nameof(Propietario.Id)),
                        Nombre = reader.GetString(nameof(Propietario.Nombre)),
                        Apellido = reader.GetString(nameof(Propietario.Apellido)),
                        Documento = reader.GetString(nameof(Propietario.Documento)),
                        Telefono = reader.GetString(nameof(Propietario.Telefono)),
                        Email = reader.GetString(nameof(Propietario.Email)),
                        Direccion = reader.GetString(nameof(Propietario.Direccion))
                    };
                    lista.Add(propietario);
                }
                connection.Close();
            }

        }
        return lista;
    }

}