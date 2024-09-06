using System.ComponentModel.Design;
using System.Transactions;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;
public class RepositorioPago
{

    private readonly string _connectionString;
    public RepositorioPago(string connectionString)
    {
        _connectionString = connectionString;
    }


    public List<Pago> ObtenerTodos()
    {
        List<Pago> pagos = new List<Pago>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {                       //
            var sql = @$"SELECT {nameof(Pago.Id)}, {nameof(Pago.Fecha)}, {nameof(Pago.Monto)}, {nameof(Pago.Estado)}, 
                      {nameof(Pago.FechaAnulacion)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)},
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)} 
                     FROM pago WHERE {nameof(Pago.Estado)} = 1;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {

                    var concepto = new RepositorioConcepto(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))));

                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        FechaAnulacion = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion))),
                        Detalle = reader.GetString(reader.GetOrdinal(nameof(Pago.Detalle))),
                        Concepto = concepto
                    });
                }
                connection.Close();
                return pagos;
            }
        }
    }


    public Pago ObtenerPorId(int id)
    {
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            Pago? pago = null;
            var sql = @$"SELECT {nameof(Pago.Id)}, {nameof(Pago.Fecha)}, {nameof(Pago.Monto)}, {nameof(Pago.Estado)}, 
                      {nameof(Pago.FechaAnulacion)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)},
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)} 
                     FROM pago WHERE {nameof(Pago.Estado)} = 1 AND {nameof(Pago.Id)} = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var concepto = new RepositorioConcepto(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))));

                    return new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        FechaAnulacion = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion))),
                        Detalle = reader.GetString(reader.GetOrdinal(nameof(Pago.Detalle))),
                        Concepto = concepto
                    };
                }
                connection.Close();
            }
            return pago;
        }
    }


    public List<Pago> ObtenerPorConcepto(int id)
    {
        List<Pago> pagos = new List<Pago>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = @$"SELECT {nameof(Pago.Id)}, {nameof(Pago.Fecha)}, {nameof(Pago.Monto)}, {nameof(Pago.Estado)},
             {nameof(Pago.FechaAnulacion)}, {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)}
            FROM pago WHERE {nameof(Pago.Estado)} = 1 AND {nameof(Pago.Concepto)} = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var concepto = new RepositorioConcepto(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))));

                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        FechaAnulacion = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion))),
                        Detalle = reader.GetString(reader.GetOrdinal(nameof(Pago.Detalle))),
                        Concepto = concepto
                    });
                }
                connection.Close();
                return pagos;
            }
        }
    }


    public int Alta(Pago pago)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            //
            var query = $@"INSERT INTO Pago Id, Fecha, Monto, Estado, UsuPago, Detalle, Concepto
             VALUES ( @Id, @Fecha, @Monto, @Estado, @UsuPago,
             @FechaAnulacion,@Detalle, @Concepto; SELECT LAST_INSERT_ID();";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Fecha", pago.Fecha);
                command.Parameters.AddWithValue("@Monto", pago.Monto);
                command.Parameters.AddWithValue("@Estado", pago.Estado);
                command.Parameters.AddWithValue("@UsuPago", pago.UsuPago.Id);
                command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                command.Parameters.AddWithValue("@Concepto", pago.Concepto.Id);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }


    public int Anular(int id)
    {
        var res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = @$"UPDATE pago SET Estado = 0 WHERE Id = {id};";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        return res;
    }


    public int Baja(int id)
    {
        var res = -1;
        MySqlConnection connection = new MySqlConnection(_connectionString);
        var sql = @$"DELETE FROM pago WHERE {nameof(Pago.Id)} = {id};";
        using (MySqlCommand command = new MySqlCommand(sql, connection))
        {
            connection.Open();
            res = command.ExecuteNonQuery();
            connection.Close();
        }
        return res;
    }


}


