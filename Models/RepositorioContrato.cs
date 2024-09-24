using System.ComponentModel.Design;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;


namespace inmobiliaria_AT.Models;

public class RepositorioContrato
{
    private readonly string _connectionString;
    private readonly ILogger<RepositorioContrato> _logger;
    private readonly ILogger<RepositorioInmueble> _loggerInmueble;
    private ILogger<RepositorioPago> logger;
    private string connectionString;

    public RepositorioContrato(ILogger<RepositorioContrato> logger, ILogger<RepositorioInmueble> loggerInmueble, string connectionString)
    {
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerInmueble = loggerInmueble ?? throw new ArgumentNullException(nameof(loggerInmueble));
    }


    // ------------ EstadoS ==> 0: --, 1: ACTIVO, 2: VENCIDO, 3: RESCINDIDO, 4: RENOVADO  ---------------------

    public List<Contrato> ObtenerTodos()
    {
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Estado, Descripcion, Observaciones, UsuarioCreacion, UsuarioAnulacion, Pagos FROM contrato
            WHERE Estado = 1 OR Estado = 4;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idInquilino = reader.GetInt32("Inqui");
                    int idInmueble = reader.GetInt32("Inmu");
                    int idPropietario = reader.GetInt32("Prop");
                    var inquilino = new RepositorioInquilino(_connectionString).ObtenerPorId(idInquilino);
                    var inmueble = new RepositorioInmueble(_loggerInmueble, _connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);
                    var creacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioCreacion"));
                    var pagos = reader.GetInt32("Pagos");
                    var anulacion = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacion"))
                    ? null
                    : new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioAnulacion"));

                    if (inquilino == null || inmueble == null || propietario == null)
                    {
                        Console.WriteLine("Datos incompletos: Inquilino: " + inquilino + ", Inmueble: " + inmueble + ", Propietario: " + propietario);
                        continue;
                    }

                    // Crear el contrato
                    var contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Estado = reader.GetInt32("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Observaciones = reader.GetString("Observaciones"),
                        UsuCreacion = creacion,
                        UsuAnulacion = anulacion
                    };

                    contratos.Add(contrato);

                }
                connection.Close();
            }
        }
        return contratos;
    }


    public List<Contrato> ObtenerVencidos()
    { //Estado DE VENCIDO = 2
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Estado, Descripcion,Observaciones, UsuarioCreacion, UsuarioAnulacion, Pagos FROM contrato
            WHERE Estado = 2 OR Estado = 3;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idInquilino = reader.GetInt32("Inqui");
                    int idInmueble = reader.GetInt32("Inmu");
                    int idPropietario = reader.GetInt32("Prop");
                    var inquilino = new RepositorioInquilino(_connectionString).ObtenerPorId(idInquilino);
                    var inmueble = new RepositorioInmueble(_loggerInmueble, _connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);
                    var pagos = reader.GetInt32("Pagos");
                    var creacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioCreacion"));
                    var anulacion = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacion"))
                    ? null
                    : new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioAnulacion"));
                    if (inquilino == null || inmueble == null || propietario == null)
                    {
                        Console.WriteLine("Datos incompletos: Inquilino: " + inquilino + ", Inmueble: " + inmueble + ", Propietario: " + propietario);
                        continue;
                    }

                    var contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Estado = reader.GetInt32("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Observaciones = reader.GetString("Observaciones"),
                        UsuCreacion = creacion,
                        UsuAnulacion = anulacion,
                        Pagos = pagos

                    };

                    contratos.Add(contrato);
                }
                connection.Close();
            }
        }
        return contratos;
    }


    public Contrato ObtenerPorId(int id)
    {//AGREGAR TIPO
        Contrato? cont = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.Inqui)},{nameof(Contrato.Inmu)}
            ,{nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Estado)}
            ,{nameof(Contrato.Descripcion)},{nameof(Contrato.Observaciones)},
            {nameof(Contrato.Prop)}, UsuarioCreacion, UsuarioAnulacion, Pagos FROM contrato WHERE {nameof(Contrato.Id)} = {id};";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {

                    RepositorioInquilino repositorioInquilino = new RepositorioInquilino(_connectionString);
                    RepositorioInmueble repositorioInmueble = new RepositorioInmueble(_loggerInmueble, _connectionString);
                    RepositorioPropietario repositorioPropietario = new RepositorioPropietario(_connectionString);
                    int idInquilino = reader.GetInt32(nameof(Contrato.Inqui));
                    int Inmu = reader.GetInt32(nameof(Contrato.Inmu));
                    int Prop = reader.GetInt32(nameof(Contrato.Prop));
                    Inquilino inquilino = repositorioInquilino.ObtenerPorId(idInquilino);
                    Inmueble inmueble = repositorioInmueble.ObtenerPorId(Inmu);
                    Propietario propietario = repositorioPropietario.ObtenerPorId(Prop);
                    var creacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioCreacion"));
                    int pagos = reader.GetInt32("Pagos");
                    var anulacion = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacion"))
                        ? null
                        : new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioAnulacion")); if (inquilino == null || inmueble == null || propietario == null)

                    {
                        return null;

                    }
                    else
                    {
                        return new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            Inqui = inquilino,
                            Inmu = inmueble,
                            Prop = propietario,
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            Estado = reader.GetInt32(nameof(Contrato.Estado)),
                            Descripcion = reader.GetString(nameof(Contrato.Descripcion)),
                            Observaciones = reader.GetString(nameof(Contrato.Observaciones)),
                            UsuCreacion = creacion,
                            UsuAnulacion = anulacion,
                            Pagos = pagos
                        };
                    }

                }
                connection.Close();
            }
            return cont;
        }
    }


    public int Alta(Contrato contrato)
    {
        var repo_Inm = new RepositorioInmueble(_loggerInmueble, _connectionString);
        var id_prop = repo_Inm.ObtenerPorId(contrato.Inmu.Id).IdPropietario;
        int res = -1;

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    var sql = $@"INSERT INTO contrato 
                    ({nameof(Contrato.Inqui)}, {nameof(Contrato.Inmu)}, {nameof(Contrato.FechaInicio)}, 
                    {nameof(Contrato.FechaFin)}, {nameof(Contrato.Descripcion)},   
                    {nameof(Contrato.Observaciones)}, {nameof(Contrato.Prop)}, UsuarioCreacion, Pagos) 
                    VALUES (@Inqui, @Inmu, @fechaInicio, @fechaFin, @descripcion, 
                    @observaciones, @Prop, @UsuarioCreacion, @Pagos); 
                    SELECT LAST_INSERT_ID();";

                    using (MySqlCommand command = new MySqlCommand(sql, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inqui", contrato.Inqui.Id);
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@descripcion", contrato.Descripcion);
                        command.Parameters.AddWithValue("@observaciones", contrato.Observaciones);
                        command.Parameters.AddWithValue("@UsuarioCreacion", contrato.UsuCreacion.Id);
                        command.Parameters.AddWithValue("@Prop", id_prop);
                        command.Parameters.AddWithValue("@Pagos", contrato.Pagos);
                        res = Convert.ToInt32(command.ExecuteScalar());
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            conn.Close();
        }
        return res;
    }




    public int Baja(int id)
    {
        int res = -1;
        var repoCont = new RepositorioContrato(_logger, _loggerInmueble, _connectionString);
        var contrato = repoCont.ObtenerPorId(id);

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {

            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    var sql = $@"DELETE FROM contrato WHERE {nameof(Contrato.Id)} = @id;";
                    using (MySqlCommand command = new MySqlCommand(sql, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        res = command.ExecuteNonQuery();

                    }
                    transaction.Commit();

                }
                catch
                {
                    conn.Close();

                    transaction.Rollback();

                    throw;
                }
            }


        }
        return res;
    }


    public int Anular(int id, String observ, int usuAnulacion)
    {
        int res = -1;
        var repoCont = new RepositorioContrato(_logger, _loggerInmueble, _connectionString);
        var contrato = repoCont.ObtenerPorId(id);

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {

            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    var sql = $@"UPDATE contrato SET Estado = '3', observaciones = '{observ}', UsuarioAnulacion = '{usuAnulacion}' WHERE {nameof(Contrato.Id)} = @id;";
                    using (MySqlCommand command = new MySqlCommand(sql, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        res = command.ExecuteNonQuery();

                    }

                    transaction.Commit();

                }
                catch
                {
                    conn.Close();

                    transaction.Rollback();

                    throw;
                }
            }


        }
        return res;
    }


    public int Modificar(Contrato contrato)
    {

        var Inm = new RepositorioInmueble(_loggerInmueble, _connectionString).ObtenerPorId(contrato.Inmu.Id);
        var id_prop = Inm.IdPropietario;


        //OBTENGO EL ID DEL INMUEBLE QUE TIENE EL CONTRATO ANTES DE  MODIFICAR
        var repoCont = new RepositorioContrato(_logger, _loggerInmueble, _connectionString);
        Contrato cont = repoCont.ObtenerPorId(contrato.Id);
        var id_inmu = cont.Inmu.Id;

        int res = -1;

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    var query = $@"UPDATE contrato SET 
                {nameof(Contrato.Inqui)} = @Inqui, 
                {nameof(Contrato.Inmu)} = @Inmu,
                {nameof(Contrato.FechaInicio)} = @fechaInicio,
                {nameof(Contrato.FechaFin)} = @fechaFin,
                {nameof(Contrato.Descripcion)} = @descripcion,
                {nameof(Contrato.Observaciones)} = @observaciones,
                {nameof(Contrato.Prop)} = @Prop,
                {nameof(Contrato.Pagos)} = @Pagos


                    WHERE {nameof(Contrato.Id)} = @id;";

                    using (MySqlCommand command = new MySqlCommand(query, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inqui", contrato.Inqui.Id);
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@descripcion", contrato.Descripcion);
                        command.Parameters.AddWithValue("@observaciones", contrato.Observaciones);
                        command.Parameters.AddWithValue("@id", contrato.Id);
                        command.Parameters.AddWithValue("@Prop", id_prop);
                        command.Parameters.AddWithValue("@Pagos", contrato.Pagos);
                        res = command.ExecuteNonQuery();
                    }


                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    conn.Close();

                    throw;
                }
            }
        }
        return res;
    }



    public int Renovar(Contrato contrato)

    {
        var repo_Inm = new RepositorioInmueble(_loggerInmueble, _connectionString);
        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();
            //----HAGO UNA TRANSACCION POR LAS DUDAS QUE ALGUNA DE LAS CONSULTAS FALLE
            using (MySqlTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    var sql = $@"INSERT INTO contrato 
                    ({nameof(Contrato.Inqui)}, {nameof(Contrato.Inmu)}, {nameof(Contrato.FechaInicio)}, 
                    {nameof(Contrato.FechaFin)}, {nameof(Contrato.Descripcion)},   
                    {nameof(Contrato.Observaciones)}, {nameof(Contrato.Prop)}, UsuarioCreacion, Pagos) 
                    VALUES (@Inqui, @Inmu, @fechaInicio, @fechaFin, @descripcion, 
                    @observaciones, @Prop, @UsuarioCreacion, @Pagos); 
                    SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(sql, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inqui", contrato.Inqui.Id);
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@descripcion", contrato.Descripcion);
                        command.Parameters.AddWithValue("@observaciones", contrato.Observaciones);
                        command.Parameters.AddWithValue("@Prop", contrato.Prop.Id);
                        command.Parameters.AddWithValue("@UsuarioCreacion", contrato.UsuCreacion.Id);
                        command.Parameters.AddWithValue("@Pagos", contrato.Pagos);
                        res = Convert.ToInt32(command.ExecuteScalar());
                    }

                    //CUANDO Renuevo UN CONTRATO, LE CAMBIO EL Estado A RENOVADO (||4||)
                    var sqlEstado = $@"UPDATE contrato SET Estado = '4' WHERE id = @Id ";
                    using (MySqlCommand command = new MySqlCommand(sqlEstado, conn, transaction)) //transaccion asegura que esta consulta sea parte de la misma
                    {
                        command.Parameters.AddWithValue("@Id", contrato.Id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            conn.Close();
        }
        return res;
    }

    //COMPARA FECHAS DE LOS CONTRATOS Y SI SE VENCE EL CONTRATO, CAMBIO EL Estado A 0
    public void vigenciaContrato()
    {

        List<Contrato> contratos = ObtenerTodos();
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            conn.Open();

            foreach (var item in contratos)
            {
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {

                        if (item.FechaFin <= DateTime.Now)
                        {

                            // BAJA LOGICA DEL CONTRATO DE ACUERDO A LA FECHA DE VENCIMIENTO Y LA ACTUAL
                            var EstadoContrato = $@"UPDATE contrato SET Estado = '2' WHERE id = @id;";
                            using (MySqlCommand command = new MySqlCommand(EstadoContrato, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@id", item.Id);
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();

                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        conn.Close();
                        throw;
                    }
                }
            }
        }

    }


    public List<Contrato> PorFechaTodos(DateTime desde, DateTime hasta)
    {
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = @"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Estado, Descripcion, Observaciones, UsuarioCreacion, UsuarioAnulacion
                    FROM contrato 
                    WHERE DATE(FechaFin) BETWEEN @desde AND @hasta;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@desde", desde.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@hasta", hasta.ToString("yyyy-MM-dd"));

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idInquilino = reader.GetInt32("Inqui");
                    int idInmueble = reader.GetInt32("Inmu");
                    int idPropietario = reader.GetInt32("Prop");
                    var inquilino = new RepositorioInquilino(_connectionString).ObtenerPorId(idInquilino);
                    var inmueble = new RepositorioInmueble(_loggerInmueble, _connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);
                    var creacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioCreacion"));
                    var anulacion = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacion"))
                    ? null
                    : new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioAnulacion"));


                    var contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Estado = reader.GetInt32("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Observaciones = reader.GetString("Observaciones"),
                        UsuCreacion = creacion,
                        UsuAnulacion = anulacion
                    };

                    contratos.Add(contrato);
                }
                connection.Close();
            }
        }
        return contratos;
    }


    public Contrato PorFechaId(DateTime desde, DateTime hasta, int id)
    {
        Contrato contrato = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = @"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Estado, Descripcion,
                    PorcentajeActualizacion, PeriodoActualizacion, Observaciones, Tipo , UsuarioCreacion, UsuarioAnulacion
                    FROM contrato 
                    WHERE Id = @id AND DATE(FechaFin) BETWEEN @desde AND @hasta;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@desde", desde.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@hasta", hasta.ToString("yyyy-MM-dd"));

                connection.Open();
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int idInquilino = reader.GetInt32("Inqui");
                    int idInmueble = reader.GetInt32("Inmu");
                    int idPropietario = reader.GetInt32("Prop");
                    var inquilino = new RepositorioInquilino(_connectionString).ObtenerPorId(idInquilino);
                    var inmueble = new RepositorioInmueble(_loggerInmueble, _connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);

                    var creacion = new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioCreacion"));
                    var anulacion = reader.IsDBNull(reader.GetOrdinal("UsuarioAnulacion"))
                    ? null
                    : new RepositorioUsuario(_connectionString).ObtenerPorId(reader.GetInt32("UsuarioAnulacion"));
                    contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Estado = reader.GetInt32("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Observaciones = reader.GetString("Observaciones"),
                        UsuCreacion = creacion,
                        UsuAnulacion = anulacion
                    };
                }
                connection.Close();
            }
        }
        return contrato;
    }

    public bool disponiblePorFechas(int inmuebleId, DateTime fechaInicio, DateTime fechaFin)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            var query = @"SELECT COUNT(*) FROM contrato 
                      WHERE Inmu = @InmuebleId 
                      AND (FechaInicio < @FechaFin AND FechaFin > @FechaInicio)";

            var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
            command.Parameters.AddWithValue("@FechaFin", fechaFin);
            command.Parameters.AddWithValue("@FechaInicio", fechaInicio);

            var result = command.ExecuteScalar();
            int count = result != null ? Convert.ToInt32(result) : 0;

            // Si el conteo es 0, significa que está disponible
            return count == 0;
        }
    }



}