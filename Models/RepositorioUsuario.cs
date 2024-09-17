using System.ComponentModel.Design;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;

public class RepositorioUsuario
{
    private readonly string _connectionString;

    public RepositorioUsuario(string connectionString)
    {
        _connectionString = connectionString;
    }
    public int Alta(Usuario usuario)
    {
        int res = -1;

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
                INSERT INTO Usuario 
					({nameof(Usuario.Nombre)}, 
                    {nameof(Usuario.Apellido)}, 
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.Clave)},                     
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)},
                    {nameof(Usuario.Salt)})
				VALUES (@nombre, @apellido, @email, @clave,@avatar, @rol,@salt);
				SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@clave", usuario.Clave);
                command.Parameters.AddWithValue("@avatar", usuario.Avatar);
                command.Parameters.AddWithValue("@rol", (int)usuario.Rol);
                command.Parameters.AddWithValue("@salt", usuario.Salt);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                usuario.Id = res;


            }
        }


        return res;
    }
    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            // Cambia el estado del usuario a false (o 0)
            string sql = "UPDATE Usuario SET Estado = @estado WHERE Id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@estado", false); // O 0 si el campo es tipo entero
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
    public int Editar(Usuario e)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            string sql = @"UPDATE Usuario 
					SET Nombre=@nombre, Apellido=@apellido,  Email=@email,Clave=@clave,Salt=@salt,Avatar=@avatar, Rol=@rol
					WHERE Id = @id";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", e.Nombre);
                command.Parameters.AddWithValue("@apellido", e.Apellido);
                command.Parameters.AddWithValue("@email", e.Email);
                command.Parameters.AddWithValue("@clave", e.Clave);
                command.Parameters.AddWithValue("@salt", e.Salt);
                command.Parameters.AddWithValue("@avatar", e.Avatar);
                command.Parameters.AddWithValue("@rol", e.Rol);
                command.Parameters.AddWithValue("@id", e.Id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public IList<Usuario> ObtenerTodos()
    {
        List<Usuario> usuarios = new List<Usuario>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {

            var query = $@"	
                SELECT  
                    {nameof(Usuario.Id)}, 
                    {nameof(Usuario.Nombre)}, 
                    {nameof(Usuario.Apellido)},  
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.Clave)}, 
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)}
				FROM 
                    Usuario
                WHERE 
                    {nameof(Usuario.Estado)} = 1;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                    while (reader.Read())
                    {

                        usuarios.Add(new Usuario
                        {
                            Id = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Id))),
                            Nombre = reader.GetString(reader.GetOrdinal(nameof(Usuario.Nombre))),
                            Apellido = reader.GetString(reader.GetOrdinal(nameof(Usuario.Apellido))),
                            Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                            Clave = reader.GetString(reader.GetOrdinal(nameof(Usuario.Clave))),
                            Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            Rol = (Roles)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol)))
                        });
                    }
                connection.Close();
            }
        }
        return usuarios;
    }

    public Usuario ObtenerPorId(int id)
    {
        Usuario? usuario = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
                SELECT 
					{nameof(Usuario.Id)}, 
                    {nameof(Usuario.Nombre)}, 
                    {nameof(Usuario.Apellido)},  
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.Clave)}, 
                    {nameof(Usuario.Salt)},
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)}
				FROM 
                    Usuario WHERE Id=@id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Usuario
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Id))),
                        Nombre = reader.GetString(reader.GetOrdinal(nameof(Usuario.Nombre))),
                        Apellido = reader.GetString(reader.GetOrdinal(nameof(Usuario.Apellido))),
                        Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                        Clave = reader.GetString(reader.GetOrdinal(nameof(Usuario.Clave))),
                        Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(nameof(Usuario.Avatar)),
                        Salt = reader.GetString(reader.GetOrdinal(nameof(Usuario.Salt))),
                        Rol = (Roles)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol)))

                    };
                }
                connection.Close();
            }
        }
        return usuario;
    }

    public Usuario ObtenerPorEmail(string email)
    {
        Usuario? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
                SELECT
                    {nameof(Usuario.Id)},
					{nameof(Usuario.Nombre)}, 
                    {nameof(Usuario.Apellido)}, 
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.Clave)}, 
                    {nameof(Usuario.Salt)},
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)}
                FROM Usuario
                WHERE 
                    {nameof(Usuario.Email)}=@email";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = new Usuario
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Id))),
                        Nombre = reader.GetString(reader.GetOrdinal(nameof(Usuario.Nombre))),
                        Apellido = reader.GetString(reader.GetOrdinal(nameof(Usuario.Apellido))),
                        Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                        Clave = reader.GetString(reader.GetOrdinal(nameof(Usuario.Clave))),
                        Salt = reader.GetString(reader.GetOrdinal(nameof(Usuario.Salt))),
                        Avatar = reader.GetString(reader.GetOrdinal(nameof(Usuario.Avatar))),
                        Rol = (Roles)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol))),

                    };
                }
                connection.Close();
            }
        }
        return res;
    }
}
