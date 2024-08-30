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


    public List<Contrato> ObtenerTodos()
    {
        List<Contrato> contrato = new List<Contrato>();
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.Inqui)},{nameof(Contrato.Inmu)},{nameof(Contrato.Prop)}
            ,{nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)}
            ,{nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
            {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)}, {nameof(Contrato.Tipo)} FROM contrato;";

            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();

                var reader = command.ExecuteReader();
                {
         

                    RepositorioInquilino repositorioInquilino = new RepositorioInquilino(_connectionString);
                    RepositorioInmueble repositorioInmueble = new RepositorioInmueble(_connectionString);
                    RepositorioPropietario repositorioPropietario = new RepositorioPropietario(_connectionString);
                    while (reader.Read())
                    {
                        int idInquilino = reader.GetInt32("Inqui");
                        int idInmueble = reader.GetInt32("Inmu");
                        int idPropietario = reader.GetInt32("Prop");

                        Inquilino inquilino = repositorioInquilino.ObtenerPorId(idInquilino);
                        Inmueble inmueble = repositorioInmueble.ObtenerPorId(idInmueble);
                        Propietario propietario = repositorioPropietario.ObtenerPorId(idPropietario);

                        if (inquilino == null || inmueble == null || propietario == null)
                        {
                            // Manejar el caso en que alguno de los objetos sea null
                            continue; // O lanzar una excepción
                        }
                        var tipoStr = reader.GetString(reader.GetOrdinal("Tipo"));

                       if (!Enum.TryParse(tipoStr, ignoreCase: true, out TipoContrato tipo))
                       {
                           tipo = TipoContrato.Arrendamiento;
                       }


                        contrato.Add(new Contrato
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
                            Observaciones = reader.GetString("Observaciones")
                            , Tipo = tipo

                        });
                    }

                    connection.Close();
                }
                return contrato;
            }
        }
    }

    //Tipo = (TipoContrato)Enum.Parse(typeof(TipoContrato), reader.GetString(nameof(Contrato.Tipo))),

    public Contrato? ObtenerPorId(int id)
    {//AGREGAR TIPO
        Contrato? cont = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.Inqui)},{nameof(Contrato.Inmu)},{nameof(Contrato.Prop)}
            ,{nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)}
            ,{nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
            {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)},{nameof(Contrato.Tipo)} FROM contrato WHERE {nameof(Contrato.Id)} = {id};";
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
                    int idInmueble = reader.GetInt32(nameof(Contrato.Inmu));
                    int idPropietario = reader.GetInt32(nameof(Contrato.Prop));
                    Inquilino inquilino = repositorioInquilino.ObtenerPorId(idInquilino);
                    Inmueble inmueble = repositorioInmueble.ObtenerPorId(idInmueble);
                    Propietario propietario = repositorioPropietario.ObtenerPorId(idPropietario);
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
        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            var sql = $@"INSERT INTO contrato 
            ({nameof(Contrato.Inqui)}, {nameof(Contrato.Inmu)}, {nameof(Contrato.Prop)}, {nameof(Contrato.FechaInicio)}, 
            {nameof(Contrato.FechaFin)}, {nameof(Contrato.Monto)}, {nameof(Contrato.Estado)}, 
            {nameof(Contrato.Descripcion)}, {nameof(Contrato.Plazo)}, {nameof(Contrato.PorcentajeActualizacion)}, 
            {nameof(Contrato.PeriodoActualizacion)}, {nameof(Contrato.Observaciones)}, {nameof(Contrato.Tipo)}) 
            VALUES (@idInquilino, @idInmueble, @idPropietario, @fechaInicio, @fechaFin, @monto, @estado, @descripcion, 
            @plazo, @porcentajeActualizacion, @periodoActualizacion, @observaciones, @tipo); 
            SELECT LAST_INSERT_ID();";
            using (MySqlCommand command = new MySqlCommand(sql, conn))
            {
                command.Parameters.AddWithValue("@idInquilino", contrato.Inqui.Id);
                command.Parameters.AddWithValue("@idInmueble", contrato.Inmu.Id);
                command.Parameters.AddWithValue("@idPropietario", contrato.Prop.Id);
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
                conn.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
            }

            conn.Close();

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
                //command.Parameters.AddWithValue("@tipo", contrato.Tipo.ToString());


    public int Modificar(Contrato contrato)
    {
        int res = -1;
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        {
            var query = $@"UPDATE contrato SET {nameof(Contrato.Inqui)} = @idInquilino, {nameof(Contrato.Inmu)} = @idInmueble,
             {nameof(Contrato.Prop)} = @idPropietario,{nameof(contrato.FechaInicio)} = @fechaInicio,{nameof(contrato.FechaFin)} = @fechaFin,
             {nameof(contrato.Monto)} = @monto,{nameof(contrato.Estado)} = @estado ,{nameof(contrato.Descripcion)} = @descripcion,
             {nameof(contrato.Plazo)} = @plazo,{nameof(contrato.PorcentajeActualizacion)} = @porcentajeActualizacion,{nameof(contrato.PeriodoActualizacion)} = @periodoActualizacion,
             {nameof(contrato.Observaciones)} = @observaciones, {contrato.Tipo} = @tipo WHERE {nameof(Contrato.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@idInquilino", contrato.Inqui.Id);
                command.Parameters.AddWithValue("@idInmueble", contrato.Inmu.Id);
                command.Parameters.AddWithValue("@idPropietario", contrato.Prop.Id);
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
                conn.Open();
                res = Convert.ToInt32(command.ExecuteNonQuery());
                conn.Close();
            }
        }
        return res;
    }




}