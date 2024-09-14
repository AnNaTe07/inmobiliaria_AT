using System.ComponentModel.Design;
using System.Transactions;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;
public class RepositorioPago
{

    private readonly string _connectionString;
    private readonly ILogger<RepositorioPago> _logger;
    private readonly ILogger<RepositorioUsuario> _loggerUsuario;
    private readonly ILogger<RepositorioConcepto> _loggerConcepto;
    private readonly ILogger<RepositorioInmueble> _loggerInmueble;

    private ILogger<RepositorioContrato> _loggerContrato;

    public RepositorioPago(ILogger<RepositorioPago> logger, ILogger<RepositorioContrato> loggerContrato, ILogger<RepositorioUsuario> loggerUsuario, ILogger<RepositorioInmueble> loggerInmueble, ILogger<RepositorioConcepto> loggerConcepto, String connectionString)
    {
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerConcepto = loggerConcepto ?? throw new ArgumentNullException(nameof(loggerConcepto));
        _loggerUsuario = loggerUsuario ?? throw new ArgumentNullException(nameof(loggerUsuario));
        _loggerInmueble = loggerInmueble ?? throw new ArgumentNullException(nameof(loggerInmueble));
        _loggerContrato = loggerContrato ?? throw new ArgumentNullException(nameof(loggerContrato));

    }


    public List<Pago> ObtenerTodos()
    {
        List<Pago> pagos = new List<Pago>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {                       //
            var sql = @$"SELECT {nameof(Pago.Id)}, {nameof(Pago.Fecha)}, {nameof(Pago.Monto)}, {nameof(Pago.Estado)}, 
                      {nameof(Pago.FechaAnulacion)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)},
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, IdContrato 
                     FROM pago;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {

                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal("IdContrato")));

                    // Verifica si 'UsuAnulacion' es nulo
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                        ? new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
                        : null;
                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Contrato = contrato,
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        FechaAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        ? reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        : (DateTime?)null,

                        Detalle = reader.GetString(reader.GetOrdinal(nameof(Pago.Detalle))),
                        Concepto = concepto
                    });
                }
                connection.Close();
                return pagos;
            }
        }
    }

    public List<Pago> ObtenerPorContrato(int id)
    {
        List<Pago> pagos = new List<Pago>();

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {                       //
            var sql = @$"SELECT {nameof(Pago.Id)}, {nameof(Pago.Fecha)}, {nameof(Pago.Monto)}, {nameof(Pago.Estado)}, 
                      {nameof(Pago.FechaAnulacion)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)},
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, IdContrato 
                     FROM pago WHERE IdContrato = {id};";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {

                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal("IdContrato")));

                    // Verifica si 'UsuAnulacion' es nulo
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                        ? new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
                        : null;
                    pagos.Add(new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Contrato = contrato,
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        FechaAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        ? reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        : (DateTime?)null,

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
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, IdContrato
                     FROM pago WHERE {nameof(Pago.Id)} = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                       ? new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
                       : null;
                    var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal("IdContrato")));
                    return new Pago
                    {
                        Id = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Id))),
                        Fecha = reader.GetDateTime(reader.GetOrdinal(nameof(Pago.Fecha))),
                        Monto = reader.GetDecimal(reader.GetOrdinal(nameof(Pago.Monto))),
                        Estado = reader.GetBoolean(reader.GetOrdinal(nameof(Pago.Estado))),
                        UsuAnulacion = uAnulacion,
                        UsuPago = uPago,
                        Contrato = contrato,
                        FechaAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        ? reader.GetDateTime(reader.GetOrdinal(nameof(Pago.FechaAnulacion)))
                        : (DateTime?)null,
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
                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = new RepositorioUsuario(_loggerUsuario, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))));


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


