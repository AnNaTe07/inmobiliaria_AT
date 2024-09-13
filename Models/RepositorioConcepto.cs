using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace inmobiliaria_AT.Models
{
    public class RepositorioConcepto
    {
        private readonly string _connectionString;
        private readonly ILogger<RepositorioConcepto> _logger;

        public RepositorioConcepto(ILogger<RepositorioConcepto> logger, String connectionString)
        {
            _connectionString = connectionString;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Concepto ObtenerPorId(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                Concepto? concepto = null;
                var sql = "SELECT Id, Nombre FROM concepto WHERE Id = @id;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        concepto = new Concepto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                        };
                    }
                    connection.Close();
                }
                return concepto;
            }
        }

        public List<Concepto> ObtenerTodos()
        {
            List<Concepto> conceptos = new List<Concepto>();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT Id, Nombre FROM concepto;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        conceptos.Add(new Concepto
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Nombre = reader.GetString(reader.GetOrdinal("Nombre"))
                        });
                    }
                    connection.Close();
                }
            }
            return conceptos;
        }
    }
}
