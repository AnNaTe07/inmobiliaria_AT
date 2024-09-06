using System.ComponentModel.Design;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;

public class RepositorioInmueble
{
    private readonly string _connectionString;

    public RepositorioInmueble(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Inmueble> ObtenerTodos()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
        SELECT 
            i.{nameof(Inmueble.Id)} AS InmuebleId, 
            i.{nameof(Inmueble.Uso)}, 
            i.{nameof(Inmueble.Direccion)}, 
            i.{nameof(Inmueble.TipoId)}, 
            t.{nameof(Tipo.Descripcion)} AS TipoDescripcion, 
            i.{nameof(Inmueble.Ambientes)}, 
            i.{nameof(Inmueble.Latitud)}, 
            i.{nameof(Inmueble.Longitud)}, 
            i.{nameof(Inmueble.Superficie)},
            i.{nameof(Inmueble.Precio)},
            i.{nameof(Inmueble.IdPropietario)},
            i.{nameof(Inmueble.Estado)},
            p.{nameof(Propietario.Nombre)} AS PropietarioNombre, 
            p.{nameof(Propietario.Apellido)} AS PropietarioApellido
        FROM 
            inmueble i
        JOIN 
            tipo t ON i.{nameof(Inmueble.TipoId)} = t.{nameof(Tipo.Id)}
        INNER JOIN 
            Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.Id)};
        ";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usoStr = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Uso)));

                        if (!Enum.TryParse(usoStr, ignoreCase: true, out UsoInmueble uso))
                        {
                            uso = UsoInmueble.Comercial;
                        }
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                            Uso = uso,
                            Direccion = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
                            TipoId = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.TipoId))),
                            TipoDescripcion = reader.GetString(reader.GetOrdinal("TipoDescripcion")),
                            Ambientes = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Ambientes))),
                            Latitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
                            Longitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
                            Superficie = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Superficie))),
                            Precio = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Precio))),
                            IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                            Estado = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Estado))) == 1,
                            PropietarioInmueble = new Propietario
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                                Nombre = reader.GetString(reader.GetOrdinal("PropietarioNombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("PropietarioApellido"))
                            }
                        });
                    }
                    connection.Close();
                }
                return inmuebles;
            }
        }
    }

    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
            SELECT 
                i.{nameof(Inmueble.Id)}, 
                i.{nameof(Inmueble.Uso)}, 
                i.{nameof(Inmueble.Direccion)}, 
                i.{nameof(Inmueble.TipoId)},
                t.{nameof(Tipo.Descripcion)} AS TipoDescripcion, 
                i.{nameof(Inmueble.Ambientes)},
                i.{nameof(Inmueble.Latitud)}, 
                i.{nameof(Inmueble.Longitud)}, 
                i.{nameof(Inmueble.Superficie)},
                i.{nameof(Inmueble.Precio)}, 
                i.{nameof(Inmueble.IdPropietario)},
                i.{nameof(Inmueble.Estado)},
                p.{nameof(Propietario.Nombre)} AS PropietarioNombre, 
                p.{nameof(Propietario.Apellido)} AS PropietarioApellido
            FROM 
                inmueble i
            JOIN 
            tipo t ON i.{nameof(Inmueble.TipoId)} = t.{nameof(Tipo.Id)}
        INNER JOIN 
            Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.Id)} 
            WHERE 
                i.{nameof(Inmueble.Id)} = @id;
        ";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Inmueble
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Id))),
                        Uso = (UsoInmueble)Enum.Parse(typeof(UsoInmueble), reader.GetString(reader.GetOrdinal(nameof(Inmueble.Uso)))),
                        Direccion = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
                        TipoId = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.TipoId))),
                        TipoDescripcion = reader.GetString(reader.GetOrdinal("TipoDescripcion")),
                        Ambientes = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Ambientes))),
                        Latitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
                        Longitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
                        Superficie = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Superficie))),
                        Precio = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Precio))),
                        IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                        Estado = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Estado))) == 1,

                        PropietarioInmueble = new Propietario
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                            Nombre = reader.GetString(reader.GetOrdinal("PropietarioNombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("PropietarioApellido"))
                        }
                    };
                }
                connection.Close();
            }
            return res;
        }
    }

    public int Alta(Inmueble inmueble)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var usoString = inmueble.Uso.ToString();

            var query = $@"INSERT INTO inmueble ({nameof(Inmueble.Uso)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.TipoId)}, {nameof(Inmueble.Ambientes)},{nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)},{nameof(Inmueble.Superficie)}, {nameof(Inmueble.Precio)}, {nameof(Inmueble.IdPropietario)}, {nameof(Inmueble.Estado)}) VALUES (@uso, @direccion, @tipoId, @ambientes, @latitud,@longitud,@superficie, @precio, @idPropietario); SELECT LAST_INSERT_ID();";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@uso", usoString);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@tipoId", inmueble.TipoId);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@estado", inmueble.Estado ? 1 : 0);
                command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }
    public int Modificar(Inmueble inmueble)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            // Convierto el valor del enum a string
            var usoString = inmueble.Uso.ToString();

            var query = $@"UPDATE inmueble SET {nameof(Inmueble.Uso)} =@uso, {nameof(Inmueble.Direccion)}=@direccion, {nameof(Inmueble.TipoId)}=@tipoId, {nameof(Inmueble.Ambientes)}=@ambientes, {nameof(Inmueble.Latitud)}=@latitud, {nameof(Inmueble.Longitud)}=@longitud,{nameof(Inmueble.Superficie)}=@superficie, {nameof(Inmueble.Precio)}=@precio, {nameof(Inmueble.IdPropietario)}=@idPropietario, {nameof(Inmueble.Estado)}=@estado WHERE {nameof(Inmueble.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@uso", usoString);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@tipoId", inmueble.TipoId);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                command.Parameters.AddWithValue("@estado", inmueble.Estado ? 1 : 0);
                command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                command.Parameters.AddWithValue("@id", inmueble.Id);
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
            var query = $@"DELETE FROM inmueble WHERE {nameof(Inmueble.Id)} = @id;";
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
    public List<Inmueble> ObtenerDispo()
    {
        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"
        SELECT 
            i.{nameof(Inmueble.Id)} AS InmuebleId, 
            i.{nameof(Inmueble.Uso)}, 
            i.{nameof(Inmueble.Direccion)}, 
            i.{nameof(Inmueble.TipoId)}, 
            t.{nameof(Tipo.Descripcion)} AS TipoDescripcion, 
            i.{nameof(Inmueble.Ambientes)}, 
            i.{nameof(Inmueble.Latitud)}, 
            i.{nameof(Inmueble.Longitud)}, 
            i.{nameof(Inmueble.Superficie)},
            i.{nameof(Inmueble.Precio)}, 
            i.{nameof(Inmueble.IdPropietario)},
            i.{nameof(Inmueble.Estado)}, 
            p.{nameof(Propietario.Nombre)} AS PropietarioNombre, 
            p.{nameof(Propietario.Apellido)} AS PropietarioApellido
        FROM 
            inmueble i
        JOIN 
            tipo t ON i.{nameof(Inmueble.TipoId)} = t.{nameof(Tipo.Id)}
        INNER JOIN 
            Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.Id)} WHERE estado = true;
        ";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usoStr = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Uso)));

                        if (!Enum.TryParse(usoStr, ignoreCase: true, out UsoInmueble uso))
                        {
                            uso = UsoInmueble.Comercial;
                        }
                        inmuebles.Add(new Inmueble
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                            Uso = uso,
                            Direccion = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
                            TipoId = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.TipoId))),
                            TipoDescripcion = reader.GetString(reader.GetOrdinal("TipoDescripcion")),
                            Ambientes = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Ambientes))),
                            Latitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
                            Longitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
                            Superficie = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Superficie))),
                            Precio = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Precio))),
                            IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                            Estado = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Estado))) == 1,
                            PropietarioInmueble = new Propietario
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                                Nombre = reader.GetString(reader.GetOrdinal("PropietarioNombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("PropietarioApellido"))
                            }
                        });
                    }
                    connection.Close();
                }
                return inmuebles;
            }
        }
    }
    public List<Inmueble> BuscarPorPropietario(int idPropietario)
    {

        List<Inmueble> inmuebles = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))

        {
            var query = @$"
             SELECT 
            i.{nameof(Inmueble.Id)},
            i.{nameof(Inmueble.Uso)}, 
            i.{nameof(Inmueble.Direccion)}, 
            i.{nameof(Inmueble.TipoId)}, 
            t.{nameof(Tipo.Descripcion)} AS TipoDescripcion, 
            i.{nameof(Inmueble.Ambientes)}, 
            i.{nameof(Inmueble.Latitud)}, 
            i.{nameof(Inmueble.Longitud)}, 
            i.{nameof(Inmueble.Superficie)},
            i.{nameof(Inmueble.Precio)}, 
            i.{nameof(Inmueble.IdPropietario)},
            i.{nameof(Inmueble.Estado)},
            p.{nameof(Propietario.Nombre)} AS PropietarioNombre, 
            p.{nameof(Propietario.Apellido)} AS PropietarioApellido
        FROM 
            inmueble i
        JOIN 
            tipo t ON i.{nameof(Inmueble.TipoId)} = t.{nameof(Tipo.Id)}
        INNER JOIN 
            Propietario p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.Id)} PropietarioId=@idPropietario";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var usoStr = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Uso)));

                    if (!Enum.TryParse(usoStr, ignoreCase: true, out UsoInmueble uso))
                    {
                        uso = UsoInmueble.Comercial;
                    }
                    inmuebles.Add(new Inmueble
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("InmuebleId")),
                        Uso = uso,
                        Direccion = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
                        TipoId = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.TipoId))),
                        TipoDescripcion = reader.GetString(reader.GetOrdinal("TipoDescripcion")),
                        Ambientes = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Ambientes))),
                        Latitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
                        Longitud = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
                        Superficie = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Superficie))),
                        Precio = reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Precio))),
                        IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Inmueble.Estado))),
                        PropietarioInmueble = new Propietario
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("IdPropietario")),
                            Nombre = reader.GetString(reader.GetOrdinal("PropietarioNombre")),
                            Apellido = reader.GetString(reader.GetOrdinal("PropietarioApellido"))
                        }
                    });
                }
                connection.Close();
            }
            return inmuebles;
        }
    }

}