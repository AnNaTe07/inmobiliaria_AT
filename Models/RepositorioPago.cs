using System.ComponentModel.Design;
using System.Transactions;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models;
public class RepositorioPago
{

    private readonly string _connectionString;
    private readonly ILogger<RepositorioPago> _logger;
    private readonly ILogger<RepositorioConcepto> _loggerConcepto;
    private readonly ILogger<RepositorioInmueble> _loggerInmueble;

    private ILogger<RepositorioContrato> _loggerContrato;

    public RepositorioPago(ILogger<RepositorioPago> logger, ILogger<RepositorioContrato> loggerContrato, ILogger<RepositorioUsuario> loggerUsuario, ILogger<RepositorioInmueble> loggerInmueble, ILogger<RepositorioConcepto> loggerConcepto, String connectionString)
    {
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerConcepto = loggerConcepto ?? throw new ArgumentNullException(nameof(loggerConcepto));
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
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, {nameof(Pago.Nro)}, IdContrato 
                     FROM pago;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {

                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));

                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal("IdContrato")));

                    // Verifica si 'UsuAnulacion' es nulo
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                        ? new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
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
                        Concepto = concepto,
                        Nro = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Nro)))
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
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, {nameof(Pago.Nro)}, IdContrato 
                     FROM pago WHERE IdContrato = {id};";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {

                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal("IdContrato")));

                    // Verifica si 'UsuAnulacion' es nulo
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                        ? new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
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
                        Concepto = concepto,
                        Nro = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Nro)))
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
                     {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, {nameof(Pago.Nro)}, IdContrato
                     FROM pago WHERE {nameof(Pago.Id)} = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
                    var uPago = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuPago))));
                    var uAnulacion = !reader.IsDBNull(reader.GetOrdinal(nameof(Pago.UsuAnulacion)))
                       ? new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.UsuAnulacion))))
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
                        Concepto = concepto,
                        Nro = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Nro)))
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
             {nameof(Pago.FechaAnulacion)}, {nameof(Pago.Detalle)}, {nameof(Pago.Concepto)}, {nameof(Pago.UsuPago)}, {nameof(Pago.UsuAnulacion)}, {nameof(Pago.Nro)}
            FROM pago WHERE {nameof(Pago.Estado)} = 1 AND {nameof(Pago.Concepto)} = @id;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var concepto = new RepositorioConcepto(_loggerConcepto, _connectionString).ObtenerPorId(reader.GetInt32(reader.GetOrdinal(nameof(Pago.Concepto))));
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
                        Concepto = concepto,
                        Nro = reader.GetInt32(reader.GetOrdinal(nameof(Pago.Nro))),
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


        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                var query = @"
                INSERT INTO Pago (IdContrato, Fecha, Monto, Detalle, Concepto, Nro, usuPago) 
                VALUES (@IdContrato, @Fecha, @Monto, @Detalle, 1, @Nro, @usuPago); 
                SELECT LAST_INSERT_ID();";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdContrato", pago.Contrato.Id);
                    command.Parameters.AddWithValue("@Fecha", DateTime.Now);
                    command.Parameters.AddWithValue("@Monto", pago.Monto);
                    command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                    command.Parameters.AddWithValue("@Nro", ObtenerCantidadPagosPorContrato(pago.Contrato.Id) + 1);
                    command.Parameters.AddWithValue("@usuPago", pago.UsuPago.Id);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
        catch (MySqlException ex) when (ex.Number == 1062) // Código de error para duplicados
        {
            _logger.LogError($"Error al guardar el pago: {ex.Message}");
            throw new Exception("Ya existe un pago con este número para este contrato.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al guardar el pago: {ex.Message}");
            throw;
        }
        return res;
    }

    public int AltaMulta(int idContrato, string detalle, DateTime fecha, int userId)
    {
        int res = -1;
        Decimal monto = 0;
        var contrato = new RepositorioContrato(_loggerContrato, _loggerInmueble, _connectionString).ObtenerPorId(idContrato);



        DateTime? fechaFin = contrato.FechaFin;
        var pagosAdeudados = CalcularPagosAdeudados(contrato);
        var importeAdeudado = pagosAdeudados * contrato.Inmu.Precio;
        monto = CalcularMulta(contrato) + importeAdeudado;

        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            // Consulta SQL con parámetros
            var query = $@"INSERT INTO Pago (IdContrato, Fecha, Monto, Detalle, Concepto, UsuPago)
                       VALUES (@IdContrato, @Fecha, @Monto, @Detalle, 2, @UsuPago); 
                       SELECT LAST_INSERT_ID();";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // Asignación de valores a los parámetros
                command.Parameters.AddWithValue("@IdContrato", idContrato);
                command.Parameters.AddWithValue("@Fecha", fecha);
                command.Parameters.AddWithValue("@Monto", monto);
                command.Parameters.AddWithValue("@Detalle", detalle);
                command.Parameters.AddWithValue("@UsuPago", userId);

                // Abrir la conexión y ejecutar el comando
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
            }
        }
        return res;
    }

    public decimal CalcularMulta(Contrato contrato)
    {
        // Verificar que la fecha de finalización esté establecida
        if (contrato.FechaFin == null)
            throw new InvalidOperationException("La fecha de terminación no está establecida.");

        // Calcular la duración original del contrato
        TimeSpan duracionOriginal = contrato.FechaFin - contrato.FechaInicio;

        // Calcular la mitad de la duración del contrato
        DateTime mitadDuracion = contrato.FechaInicio.AddDays(duracionOriginal.TotalDays / 2);

        // Obtener la fecha actual
        DateTime fechaActual = DateTime.Now;

        if (duracionOriginal.TotalDays < 60) // Menos de 60 días
        {
            // Si es menor a dos meses, solo se cobrará un mes de alquiler
            return contrato.Inmu.Precio; // Pagar un mes de alquiler
        }
        // Calcular la multa según si se ha cumplido menos de la mitad del contrato
        decimal multa = 0;
        if (fechaActual < mitadDuracion)
        {
            multa = 2 * contrato.Inmu.Precio; // Dos meses de alquiler
        }
        else
        {
            multa = contrato.Inmu.Precio; // Un mes de alquiler
        }

        return multa;
    }

    public int CalcularPagosAdeudados(Contrato contrato)
    {
        // Calcular la duración total del contrato en días
        double duracionTotalDias = (contrato.FechaFin - contrato.FechaInicio).TotalDays;

        // Si la duración total es menor o igual a 30 días, el pago debe ser 1
        if (duracionTotalDias <= 30)
        {
            return 1;
        }

        // Calcular los meses desde la fecha de inicio hasta la fecha actual
        int mesesTranscurridos = (DateTime.Now.Year - contrato.FechaInicio.Year) * 12 + DateTime.Now.Month - contrato.FechaInicio.Month;
        // Si el contrato está activo, ajustar por días
        if (DateTime.Now < contrato.FechaFin)
        {
            // Ajustar solo el mes actual si hay menos de 30 días restantes
            if ((contrato.FechaFin - DateTime.Now).TotalDays < 30)
            {
                mesesTranscurridos--;
            }
        }
        // Asegurarse de contar el mes de inicio del contrato
        if (contrato.FechaInicio.Day > 1)
        {
            mesesTranscurridos++;
        }

        int pagosAbonados = ObtenerCantidadPagosPorContrato(contrato.Id);

        int pagosAdeudados = mesesTranscurridos - pagosAbonados;

        return Math.Max(pagosAdeudados, 0);
    }




    public int Anular(int id, int usuAnulacion)
    {
        var res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = @$"UPDATE pago SET Estado = 0, UsuAnulacion = {usuAnulacion}, FechaAnulacion = NOW() WHERE Id = {id};";
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


    public int Editar(Pago pago)
    {
        int res = -1;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var query = @"UPDATE pago SET
             IdContrato = @IdContrato, Fecha = @Fecha, Monto = @Monto,
              Detalle = @Detalle  WHERE Id = @Id;";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdContrato", pago.Contrato.Id);
                command.Parameters.AddWithValue("@Fecha", pago.Fecha);
                command.Parameters.AddWithValue("@Monto", pago.Monto);
                command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                command.Parameters.AddWithValue("@Id", pago.Id);

                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }

    public int ObtenerCantidadPagosPorContrato(int contratoId)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var query = "SELECT COUNT(*) FROM Pago WHERE IdContrato = @IdContrato";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@IdContrato", contratoId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    internal object ObtenerPorId(int? id)
    {
        throw new NotImplementedException();
    }
}



