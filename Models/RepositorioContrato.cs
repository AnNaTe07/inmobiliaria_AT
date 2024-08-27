using System.ComponentModel.Design;
using MySql.Data.MySqlClient;


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
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.IdInquilino)},{nameof(Contrato.IdPropietario)}
        ,{nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)}
        ,{nameof(Contrato.Tipo)},{nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
        {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)} FROM contrato;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();

                var reader = command.ExecuteReader();
                {
                    while (reader.Read())
                    {
                        contrato.Add(new Contrato
                        {
                            Id = reader.GetInt32(nameof(Contrato.Id)),
                            IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                            IdPropietario = reader.GetInt32(nameof(Contrato.IdPropietario)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Estado = reader.GetString(nameof(Contrato.Estado)),
                            Tipo = reader.GetString(nameof(Contrato.Tipo)),
                            Descripcion = reader.GetString(nameof(Contrato.Descripcion)),
                            Plazo = reader.GetInt32(nameof(Contrato.Plazo)),
                            PorcentajeActualizacion = reader.GetDecimal(nameof(Contrato.PorcentajeActualizacion)),
                            PeriodoActualizacion = reader.GetInt32(nameof(Contrato.PeriodoActualizacion)),
                            Observaciones = reader.GetString(nameof(Contrato.Observaciones))

                        });
                    }

                    connection.Close();
                }
                return contrato;
            }
        }
    }


    public Contrato ObtenerPorId()
    {
        Contrato? cont = null;
        using (MySqlConnection connection = new MySqlConnection(_connectionString))
        {
            var sql = $@"SELECT {nameof(Contrato.Id)},{nameof(Contrato.IdInquilino)},{nameof(Contrato.IdPropietario)},
            {nameof(Contrato.FechaInicio)},{nameof(Contrato.FechaFin)},{nameof(Contrato.Monto)},{nameof(Contrato.Estado)},
            {nameof(Contrato.Tipo)},{nameof(Contrato.Descripcion)},{nameof(Contrato.Plazo)},{nameof(Contrato.PorcentajeActualizacion)},
            {nameof(Contrato.PeriodoActualizacion)},{nameof(Contrato.Observaciones)} FROM contrato WHERE {nameof(Contrato.Id)} = @id;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Contrato
                    {
                        Id = reader.GetInt32(nameof(Contrato.Id)),
                        IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                        IdPropietario = reader.GetInt32(nameof(Contrato.IdPropietario)),
                        FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                        FechaFin = reader.GetDateTime(nameof(Contrato.FechaFin)),
                        Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                        Estado = reader.GetString(nameof(Contrato.Estado)),
                        Tipo = reader.GetString(nameof(Contrato.Tipo)),
                        Descripcion = reader.GetString(nameof(Contrato.Descripcion)),
                        Plazo = reader.GetInt32(nameof(Contrato.Plazo)),
                        PorcentajeActualizacion = reader.GetDecimal(nameof(Contrato.PorcentajeActualizacion)),
                        PeriodoActualizacion = reader.GetInt32(nameof(Contrato.PeriodoActualizacion)),
                        Observaciones = reader.GetString(nameof(Contrato.Observaciones)),

                    };

                }
                connection.Close();
            }
            return cont;
        }
    }
}




