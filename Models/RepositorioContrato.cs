using System.ComponentModel.Design;
using System.Transactions;
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


    public List<Contrato> ObtenerTodos()
    {
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Monto, Estado, Descripcion, Plazo, PorcentajeActualizacion, PeriodoActualizacion, Observaciones, Tipo FROM contrato
            WHERE estado = 1;";

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
                        Monto = reader.GetDecimal("Monto"),
                        Estado = reader.GetBoolean("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Plazo = reader.GetInt32("Plazo"),
                        PorcentajeActualizacion = reader.GetDecimal("PorcentajeActualizacion"),
                        PeriodoActualizacion = reader.GetInt32("PeriodoActualizacion"),
                        Observaciones = reader.GetString("Observaciones"),
                        Tipo = (TipoContrato)Enum.Parse(typeof(TipoContrato), reader.GetString("Tipo"))
                    };

                    contratos.Add(contrato);
                }
                connection.Close();
            }
        }
        return contratos;
    }


    public Contrato? ObtenerPorId(int id)
    {//AGREGAR TIPO
        Contrato? cont = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.Inqui)},{nameof(Contrato.Inmu)}
            ,{nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)}
            ,{nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
            {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)},{nameof(Contrato.Tipo)},{nameof(Contrato.Prop)} FROM contrato WHERE {nameof(Contrato.Id)} = {id};";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {

                    RepositorioInquilino repositorioInquilino = new RepositorioInquilino(_connectionString);
                    RepositorioInmueble repositorioInmueble = new RepositorioInmueble(_loggerInmueble,_connectionString);
                    RepositorioPropietario repositorioPropietario = new RepositorioPropietario(_connectionString);
                    int idInquilino = reader.GetInt32(nameof(Contrato.Inqui));
                    int Inmu = reader.GetInt32(nameof(Contrato.Inmu));
                    int Prop = reader.GetInt32(nameof(Contrato.Prop));
                    Inquilino inquilino = repositorioInquilino.ObtenerPorId(idInquilino);
                    Inmueble inmueble = repositorioInmueble.ObtenerPorId(Inmu);
                    Propietario propietario = repositorioPropietario.ObtenerPorId(Prop);
                    if (inquilino == null || inmueble == null || propietario == null)
                    {
                        // Manejar el caso en que alguno de los objetos sea null
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
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            Descripcion = reader.GetString(nameof(Contrato.Descripcion)),
                            Plazo = reader.GetInt32(nameof(Contrato.Plazo)),
                            PorcentajeActualizacion = reader.GetDecimal(nameof(Contrato.PorcentajeActualizacion)),
                            PeriodoActualizacion = reader.GetInt32(nameof(Contrato.PeriodoActualizacion)),
                            Observaciones = reader.GetString(nameof(Contrato.Observaciones)),
                            Tipo = (TipoContrato)Enum.Parse(typeof(TipoContrato), reader.GetString(nameof(Contrato.Tipo)))

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
        var repo_Inm = new RepositorioInmueble(_loggerInmueble,_connectionString);
        var id_prop = repo_Inm.ObtenerPorId(contrato.Inmu.Id).IdPropietario;
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
                        {nameof(Contrato.FechaFin)}, {nameof(Contrato.Monto)}, {nameof(Contrato.Estado)}, 
                        {nameof(Contrato.Descripcion)}, {nameof(Contrato.Plazo)}, {nameof(Contrato.PorcentajeActualizacion)}, 
                        {nameof(Contrato.PeriodoActualizacion)}, {nameof(Contrato.Observaciones)}, {nameof(Contrato.Tipo)}, {nameof(Contrato.Prop)}) 
                        VALUES (@Inqui, @Inmu, @fechaInicio, @fechaFin, @monto, @estado, @descripcion, 
                        @plazo, @porcentajeActualizacion, @periodoActualizacion, @observaciones, @tipo, @Prop); 
                        SELECT LAST_INSERT_ID();";
                    using (MySqlCommand command = new MySqlCommand(sql, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inqui", contrato.Inqui.Id);
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        //command.Parameters.AddWithValue("@Prop", contrato.Prop.Id);
                        command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@monto", contrato.Monto);
                        command.Parameters.AddWithValue("@estado", contrato.Estado);
                        command.Parameters.AddWithValue("@descripcion", contrato.Descripcion);
                        command.Parameters.AddWithValue("@plazo", contrato.Plazo);
                        command.Parameters.AddWithValue("@porcentajeActualizacion", contrato.PorcentajeActualizacion);
                        command.Parameters.AddWithValue("@periodoActualizacion", contrato.PeriodoActualizacion);
                        command.Parameters.AddWithValue("@observaciones", contrato.Observaciones);
                        command.Parameters.AddWithValue("@tipo", contrato.Tipo.ToString());
                        command.Parameters.AddWithValue("@Prop", id_prop);
                        res = Convert.ToInt32(command.ExecuteScalar());
                    }

                    //CUANDO DOY DE ALTA UN CONTRATO, LE CAMBIO EL ESTADO AL INMUEBLE PARA Q NO FIGURE EN NUEVOS CONTRATOS
                    var sqlEstado = $@"UPDATE inmueble SET estado = '0' WHERE id = @Inmu ";
                    using (MySqlCommand command = new MySqlCommand(sqlEstado, conn, transaction)) //transaccion asegura que esta consulta sea parte de la misma
                    {
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    //SI ALGUN COMANDO NO SE EJECUTA, HAGO UN ROLLBACK
                    transaction.Rollback();
                    throw;

                }
            }
        }
        return res;
    }


    public int Baja(int id)
    {
        int res = -1;
        var repoCont = new RepositorioContrato(_logger, _loggerInmueble,_connectionString);
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
                    var sqlEstado = $@"UPDATE inmueble SET estado = '1' WHERE id = @Inmu ";
                    using (MySqlCommand command = new MySqlCommand(sqlEstado, conn, transaction)) //transaccion asegura que esta consulta sea parte de la misma
                    {
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.ExecuteNonQuery();
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
        var repo_Inm = new RepositorioInmueble(_loggerInmueble,_connectionString);
        var id_prop = repo_Inm.ObtenerPorId(contrato.Inmu.Id).IdPropietario;

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
                {nameof(Contrato.Monto)} = @monto,
                {nameof(Contrato.Estado)} = @estado,
                {nameof(Contrato.Descripcion)} = @descripcion,
                {nameof(Contrato.Plazo)} = @plazo,
                {nameof(Contrato.PorcentajeActualizacion)} = @porcentajeActualizacion,
                {nameof(Contrato.PeriodoActualizacion)} = @periodoActualizacion,
                {nameof(Contrato.Observaciones)} = @observaciones,
                {nameof(Contrato.Tipo)} = @tipo,
                {nameof(Contrato.Prop)} = @Prop

                    WHERE {nameof(Contrato.Id)} = @id;";

                    using (MySqlCommand command = new MySqlCommand(query, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inqui", contrato.Inqui.Id);
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.Parameters.AddWithValue("@fechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@monto", contrato.Monto);
                        command.Parameters.AddWithValue("@estado", contrato.Estado);
                        command.Parameters.AddWithValue("@descripcion", contrato.Descripcion);
                        command.Parameters.AddWithValue("@plazo", contrato.Plazo);
                        command.Parameters.AddWithValue("@porcentajeActualizacion", contrato.PorcentajeActualizacion);
                        command.Parameters.AddWithValue("@periodoActualizacion", contrato.PeriodoActualizacion);
                        command.Parameters.AddWithValue("@observaciones", contrato.Observaciones);
                        command.Parameters.AddWithValue("@tipo", contrato.Tipo.ToString());
                        command.Parameters.AddWithValue("@id", contrato.Id);
                        command.Parameters.AddWithValue("@Prop", id_prop);
                        res = command.ExecuteNonQuery();
                    }

                    var sqlEstado = $@"UPDATE inmueble SET estado = '0' WHERE id = @Inmu ";

                    //transaccion asegura que esta consulta sea parte de la mismo
                    using (MySqlCommand command = new MySqlCommand(sqlEstado, conn, transaction))
                    {
                        command.Parameters.AddWithValue("@Inmu", contrato.Inmu.Id);
                        command.ExecuteNonQuery();
                    }

                    //SI QUIERO MODIFICAR EL INMUEBLE DEL CONTRATO, AL INMUEBLE QUE ESTABA ANTES, LO VUELVO A DEJAR DISPONIBLE PARA ALQUILAR
                    if (id_inmu != contrato.Inmu.Id)
                    {
                        var inmuAnt = $@"UPDATE inmueble SET estado = '1' WHERE id = @Inmu ";
                        using (MySqlCommand command = new MySqlCommand(inmuAnt, conn, transaction))
                        {
                            command.Parameters.AddWithValue("@Inmu", id_inmu);
                            command.ExecuteNonQuery();
                        }

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



    //COMPARA FECHAS DE LOS CONTRATOS Y SI SE VENCE EL CONTRATO, CAMBIO EL ESTADO A 0
    public void vigenciaContrato()
    {

        List<Contrato> contratos = ObtenerTodos();
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            foreach (var item in contratos)
            {
                conn.Open();
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {

                        if (item.FechaFin <= DateTime.Now)
                        {

                            // BAJA LOGICA DEL CONTRATO DE ACUERDO A LA FECHA DE VENCIMIENTO Y LA ACTUAL
                            var estadoContrato = $@"UPDATE contrato SET estado = '0' WHERE id = @id;";
                            using (MySqlCommand command = new MySqlCommand(estadoContrato, conn, transaction))
                            {
                                command.Parameters.AddWithValue("@id", item.Id);
                                command.ExecuteNonQuery();
                            }

                            //EL INMUEBLE DEL CONTRATO VUELVE A QUEDAR DISPONIBLE
                            var estadoInmueble = $@"UPDATE inmueble SET estado = '1' WHERE id = @Inmu;";
                            using (MySqlCommand command = new MySqlCommand(estadoInmueble, conn, transaction))
                            {

                                command.Parameters.AddWithValue("@Inmu", item.Inmu.Id);
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
            var sql = @"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Monto, Estado, Descripcion, Plazo,
                    PorcentajeActualizacion, PeriodoActualizacion, Observaciones, Tipo 
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
                    var inmueble = new RepositorioInmueble(_loggerInmueble,_connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);


                    // Crear el contrato
                    var contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Monto = reader.GetDecimal("Monto"),
                        Estado = reader.GetBoolean("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Plazo = reader.GetInt32("Plazo"),
                        PorcentajeActualizacion = reader.GetDecimal("PorcentajeActualizacion"),
                        PeriodoActualizacion = reader.GetInt32("PeriodoActualizacion"),
                        Observaciones = reader.GetString("Observaciones"),
                        Tipo = (TipoContrato)Enum.Parse(typeof(TipoContrato), reader.GetString("Tipo"))
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
            var sql = @"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Monto, Estado, Descripcion, Plazo,
                    PorcentajeActualizacion, PeriodoActualizacion, Observaciones, Tipo 
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
                    var inmueble = new RepositorioInmueble(_loggerInmueble,_connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);


                    contrato = new Contrato
                    {
                        Id = reader.GetInt32("Id"),
                        Inqui = inquilino,
                        Inmu = inmueble,
                        Prop = propietario,
                        FechaInicio = reader.GetDateTime("FechaInicio"),
                        FechaFin = reader.GetDateTime("FechaFin"),
                        Monto = reader.GetDecimal("Monto"),
                        Estado = reader.GetBoolean("Estado"),
                        Descripcion = reader.GetString("Descripcion"),
                        Plazo = reader.GetInt32("Plazo"),
                        PorcentajeActualizacion = reader.GetDecimal("PorcentajeActualizacion"),
                        PeriodoActualizacion = reader.GetInt32("PeriodoActualizacion"),
                        Observaciones = reader.GetString("Observaciones"),
                        Tipo = (TipoContrato)Enum.Parse(typeof(TipoContrato), reader.GetString("Tipo"))
                    };
                }
                connection.Close();
            }
        }
        return contrato;
    }







}