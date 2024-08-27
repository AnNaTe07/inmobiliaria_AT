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
        List<Inmueble> inmueble = new List<Inmueble>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT {nameof(Inmueble.Id)}, {nameof(Inmueble.Uso)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)},{nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, {nameof(Inmueble.Precio)}, {nameof(Inmueble.IdPropietario)} FROM inmueble;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                {
                    while (reader.Read())
                    {
                         var usoStr = reader.GetString(reader.GetOrdinal("Uso"));

                         if(!Enum.TryParse(usoStr,ignoreCase: true, out UsoInmueble uso))
                         {
                            uso = UsoInmueble.Comercial;
                         }
                        inmueble.Add(new Inmueble
                        {
                            Id = reader.GetInt32(nameof(Inmueble.Id)),
                            Uso = uso,
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario))
                        });
                    }
                    connection.Close();
                }
                return inmueble;
            }
        }
    }
    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? res = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = $@"SELECT  {nameof(Inmueble.Id)}, {nameof(Inmueble.Uso)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)},  {nameof(Inmueble.Precio)}, {nameof(Inmueble.IdPropietario)} FROM inmueble WHERE {nameof(Inmueble.Id)} = @id;";
            using (MySqlCommand command= new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {                       
                    return new Inmueble
                    {
                        Id = reader.GetInt32(nameof(Inmueble.Id)),
                        Uso = (UsoInmueble)Enum.Parse(typeof(UsoInmueble), reader.GetString(nameof(Inmueble.Uso))),
                        Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                        Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                        Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                        Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                        Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                        Precio = reader.GetDecimal(nameof(Inmueble.Precio)),
                        IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
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
var query=$@"INSERT INTO inmueble ({nameof(Inmueble.Uso)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)},{nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, {nameof(Inmueble.Precio)}) VALUES (@uso, @direccion, @tipo, @ambientes, @latitud,@longitud, @precio); SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@uso", inmueble.Uso);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                //command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
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

            var query=$@"UPDATE inmueble SET {nameof(Inmueble.Uso)} =@uso, {nameof(Inmueble.Direccion)}=@direccion, {nameof(Inmueble.Tipo)}=@tipo, {nameof(Inmueble.Ambientes)}=@ambientes, {nameof(Inmueble.Latitud)}=@latitud, {nameof(Inmueble.Longitud)}=@longitud, {nameof(Inmueble.Precio)}=@precio WHERE {nameof(Inmueble.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@uso", usoString);
                command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                command.Parameters.AddWithValue("@tipo", inmueble.Tipo);
                command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes); 
                command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                command.Parameters.AddWithValue("@precio", inmueble.Precio);
                //command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
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

}