using System.ComponentModel.Design;
using MySql.Data.MySqlClient; //Importa la biblioteca de MySQL


namespace inmobiliaria_AT.Models;

public class RepositorioContrato
{
    private readonly string _connectionString;

    public RepositorioContrato(string connectionString)
    {
        _connectionString = connectionString;
    }



    public bool TestConnection()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("Conexión exitosa.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error de conexión: {ex.Message}");
            return false;
        }
    }




    /*
       public List<Contrato> ObtenerTodos()
    {
         Console.WriteLine("Ejecutando ObtenerTodos...");
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.Inqui)},{nameof(Contrato.Inmu)},{nameof(Contrato.Prop)},
                    {nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)},
                    {nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
                    {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)}, {nameof(Contrato.Tipo)} FROM contrato;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    RepositorioInquilino repositorioInquilino = new RepositorioInquilino(_connectionString);
                    RepositorioInmueble repositorioInmueble = new RepositorioInmueble(_connectionString);
                    RepositorioPropietario repositorioPropietario = new RepositorioPropietario(_connectionString);

                    while (reader.Read())
                    {
                        int idInquilino = reader.GetInt32(nameof(Contrato.Inqui));
                        int idInmueble = reader.GetInt32(nameof(Contrato.Inmu));
                        int idPropietario = reader.GetInt32(nameof(Contrato.Prop));

                        var inquilino = repositorioInquilino.ObtenerPorId(idInquilino);
                        var inmueble = repositorioInmueble.ObtenerPorId(idInmueble);
                        var propietario = repositorioPropietario.ObtenerPorId(idPropietario);

                        if (inquilino == null || inmueble == null || propietario == null)
                        {
                            Console.WriteLine($"Datos incompletos: Inquilino: {inquilino}, Inmueble: {inmueble}, Propietario: {propietario}");
                            continue;
                        }

                        var tipoStr = reader.GetString(reader.GetOrdinal(nameof(Contrato.Tipo)));
                        if (!Enum.TryParse(tipoStr, ignoreCase: true, out TipoContrato tipo))
                        {
                            tipo = TipoContrato.Arrendamiento;
                        }

                        var contrato = new Contrato
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
                            Tipo = tipo
                        };

                        Console.WriteLine($"Contrato: Id={contrato.Id}, Inmueble={contrato.Inmu?.Direccion}, Propietario={contrato.Prop?.Nombre}, Inquilino={contrato.Inqui?.Nombre}");

                        contratos.Add(contrato);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en ObtenerTodos: {ex.Message}");
                    // Considera lanzar o manejar la excepción según tu caso
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        return contratos;
    }
    */



    public List<Contrato> ObtenerTodos()
    {
        List<Contrato> contratos = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT Id, Inqui, Inmu, Prop, FechaInicio, FechaFin, Monto, Estado, Descripcion, Plazo, PorcentajeActualizacion, PeriodoActualizacion, Observaciones, Tipo FROM contrato;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int idInquilino = reader.GetInt32("Inqui");
                    int idInmueble = reader.GetInt32("Inmu");
                    int idPropietario = reader.GetInt32("Prop");

                    Console.WriteLine($"Leyendo contrato: Id={reader.GetInt32("Id")}, Inqui={idInquilino}, Inmu={idInmueble}, Prop={idPropietario}");

                    // Obtén inquilino, inmueble y propietario
                    var inquilino = new RepositorioInquilino(_connectionString).ObtenerPorId(idInquilino);
                    var inmueble = new RepositorioInmueble(_connectionString).ObtenerPorId(idInmueble);
                    var propietario = new RepositorioPropietario(_connectionString).ObtenerPorId(idPropietario);

                    // Mostrar los datos obtenidos
                    Console.WriteLine($"Inquilino: {inquilino?.NombreCompleto}, Inmueble: {inmueble?.Direccion}, Propietario: {propietario?.NombreCompleto}");

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
        Console.WriteLine($"Número de contratos recuperados: {contratos.Count}");
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
                    RepositorioInmueble repositorioInmueble = new RepositorioInmueble(_connectionString);
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
                        // O lanzar una excepción   {nameof(Contrato.Tipo)}
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
    // command.Parameters.AddWithValue("@tipo", contrato.Tipo);

    public int Alta(Contrato contrato)

    {
        var repo_Inm = new RepositorioInmueble(_connectionString);
        var id_prop = repo_Inm.ObtenerPorId(contrato.Inmu.Id).IdPropietario;

        if (contrato.Inqui == null)
        {
            throw new ArgumentNullException(nameof(contrato.Inqui), "El inquilino no puede ser null.");
        }
        if (contrato.Inmu == null)
        {
            throw new ArgumentNullException(nameof(contrato.Inmu), "El inmueble no puede ser null.");
        }


        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            var sql = $@"INSERT INTO contrato 
        ({nameof(Contrato.Inqui)}, {nameof(Contrato.Inmu)}, {nameof(Contrato.FechaInicio)}, 
        {nameof(Contrato.FechaFin)}, {nameof(Contrato.Monto)}, {nameof(Contrato.Estado)}, 
        {nameof(Contrato.Descripcion)}, {nameof(Contrato.Plazo)}, {nameof(Contrato.PorcentajeActualizacion)}, 
        {nameof(Contrato.PeriodoActualizacion)}, {nameof(Contrato.Observaciones)}, {nameof(Contrato.Tipo)}, {nameof(Contrato.Prop)}) 
        VALUES (@Inqui, @Inmu, @fechaInicio, @fechaFin, @monto, @estado, @descripcion, 
        @plazo, @porcentajeActualizacion, @periodoActualizacion, @observaciones, @tipo, @Prop); 
        SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(sql, conn))
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
                conn.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
            }
        }
        return res;
    }


    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            var sql = $@"DELETE FROM contrato WHERE {nameof(Contrato.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@id", id);
                conn.Open();
                res = command.ExecuteNonQuery();
                conn.Close();

            }

        }
        return res;
    }



    public int Modificar(Contrato contrato)
    {
        var repo_Inm = new RepositorioInmueble(_connectionString);
        var id_prop = repo_Inm.ObtenerPorId(contrato.Inmu.Id).IdPropietario;

        if (contrato == null)
        {
            throw new ArgumentNullException(nameof(contrato), "El contrato no puede ser null.");
        }

        if (contrato.Inqui == null)
        {
            throw new ArgumentNullException(nameof(contrato.Inqui), "El inquilino no puede ser null.");
        }
        if (contrato.Inmu == null)
        {
            throw new ArgumentNullException(nameof(contrato.Inmu), "El inmueble no puede ser null.");
        }


        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
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

            using (MySqlCommand command = new MySqlCommand(query, conn))
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
                conn.Open();
                res = command.ExecuteNonQuery();
                conn.Close();
            }
        }
        return res;
    }





}