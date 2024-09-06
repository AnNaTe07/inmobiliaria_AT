using MySql.Data.MySqlClient;
using inmobiliaria_AT.Utils;

namespace inmobiliaria_AT.Models;


public class RepositorioUsuario
{
    private readonly string _connectionString;

    public RepositorioUsuario(string connectionString)
    {
        _connectionString = connectionString;
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
                    {nameof(Usuario.PasswordHash)}, 
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)}
				FROM 
                    Usuario";
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
                            PasswordHash = reader.GetString(reader.GetOrdinal(nameof(Usuario.PasswordHash))),
                            Avatar = reader.GetString(reader.GetOrdinal(nameof(Usuario.Avatar))),
                            Rol = (Rol)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol)))
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
                    {nameof(Usuario.PasswordHash)}, 
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
                        Avatar = reader.GetString(reader.GetOrdinal(nameof(Usuario.Avatar))),
                        Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                        PasswordHash = reader.GetString(reader.GetOrdinal(nameof(Usuario.PasswordHash))),
                        Rol = (Rol)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol)))

                    };
                }
                connection.Close();
            }
        }
        return usuario;
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
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.PasswordHash)}, 
                    {nameof(Usuario.Salt)}, 
                    {nameof(Usuario.Avatar)},
                    {nameof(Usuario.Rol)}) 
				VALUES (@nombre, @apellido, @avatar, @email, @password,@salt,@avatar, @rol);
				SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@password", usuario.PasswordHash);
                command.Parameters.AddWithValue("@salt", usuario.Salt);
                command.Parameters.AddWithValue("@avatar", usuario.Avatar);
                command.Parameters.AddWithValue("@rol", usuario.Rol);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                usuario.Id = res;//asigna el id generado
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
            var query = $@"
                DELETE
                FROM Usuario 
                WHERE {nameof(Usuario.Id)},  = @id";

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


    public int Modificacion(Usuario usuario)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
                UPDATE Usuario 
                SET Nombre=@nombre, 
                    Apellido=@apellido, 
                    Email=@email, 
                    PasswordHash=@passwordHash, 
                    Avatar=@avatar, 
                    Rol=@rol
                WHERE Id = @id";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                command.Parameters.AddWithValue("@email", usuario.Email);
                command.Parameters.AddWithValue("@passwordHash", usuario.PasswordHash);
                command.Parameters.AddWithValue("@avatar", usuario.Avatar);
                command.Parameters.AddWithValue("@rol", usuario.Rol);
                command.Parameters.AddWithValue("@id", usuario.Id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }


    public Usuario ObtenerPorEmail(string email)
    {
        Usuario? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
                SELECT
					{nameof(Usuario.Nombre)}, 
                    {nameof(Usuario.Apellido)}, 
                    {nameof(Usuario.Email)}, 
                    {nameof(Usuario.PasswordHash)}, 
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
                        Avatar = reader.GetString(reader.GetOrdinal(nameof(Usuario.Avatar))),
                        Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                        PasswordHash = reader.GetString(reader.GetOrdinal(nameof(Usuario.PasswordHash))),
                        Rol = (Rol)reader.GetInt32(reader.GetOrdinal(nameof(Usuario.Rol))),

                    };
                }
                connection.Close();
            }
        }
        return res;
    }
}

