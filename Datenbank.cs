using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Kassensystem
{
    public class Datenbank : DbContext
    {
        string connectionString;

        public Datenbank(string connectionString) : base()
        {
            this.connectionString = connectionString;
        }

        public void InitializeDatabase() // Initialisieren der Datenbank mit den erforderlichen Tabellen
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                ErstellenTabelleWennNichtExistiert(connection);
            }
        }

        public void ErstellenTabelleWennNichtExistiert(SqliteConnection connection)// Erstellen der Tabellen, falls sie nicht existieren
        {
            string createTischTableQuery = @"
                CREATE TABLE IF NOT EXISTS Tisch (ID INTEGER PRIMARY KEY)";
            using (var command = new SqliteCommand(createTischTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            string createProduktTableQuery = @"
                CREATE TABLE IF NOT EXISTS Produkt (
                    ID INTEGER PRIMARY KEY,
                    Name NVARCHAR(100),
                    Preis DECIMAL(18, 2))";
            using (var command = new SqliteCommand(createProduktTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            string createBestellungDetailTableQuery = @"
                CREATE TABLE IF NOT EXISTS BestellungDetail (
                    ID INTEGER PRIMARY KEY,
                    Bestellt DATETIME,
                    ProduktID INT,
                    Menge INT,
                    FOREIGN KEY (ProduktID) REFERENCES Produkt(ID))";
            using (var command = new SqliteCommand(createBestellungDetailTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            string createUmsatzTableQuery = @"
                CREATE TABLE IF NOT EXISTS Umsatz (
                    ID INTEGER PRIMARY KEY,
                    BestellungDetailID INT,
                    Gesamtumsatz DECIMAL(18, 2),
                    FOREIGN KEY (BestellungDetailID) REFERENCES BestellungDetail(ID))";
            using (var command = new SqliteCommand(createUmsatzTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Speichern(string tableName, object daten)// Methode zum Speichern von Daten in die Datenbank
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                var properties = daten.GetType().GetProperties().Where(p => p.Name != "ID"); //Die ID-Eigenschaft ausschließen
                var columns = string.Join(", ", properties.Select(p => p.Name));
                var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
                command.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
                foreach (var property in properties)
                {
                    command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(daten));
                }
                command.ExecuteNonQuery();
            }
        }


        public List<Produkt> LadenAlleProdukte(string tableName)
        {
            List<Produkt> produkteListe = new List<Produkt>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var produkt = new Produkt
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Preis = reader.GetDecimal(reader.GetOrdinal("Preis"))
                        };
                        produkteListe.Add(produkt);
                    }
                }
            }
            return produkteListe;
        }

        public bool ProduktExistiert(int produktId)
        {
            // Produkt aus der Datenbank laden
            var produkt = Laden<Produkt>("Produkt", produktId);

            // Überprüfen, ob das Produkt existiert
            return produkt != null;
        }

        public T Laden<T>(string tableName, int id) where T : new()// Methode zum Laden eines Objekts aus der Datenbank basierend auf der ID
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName} WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var daten = new T();
                        var properties = typeof(T).GetProperties();
                        foreach (var property in properties)
                        {
                            // Überprüfen, ob die Eigenschaft eine ID ist
                            if (property.Name == "ID")
                            {
                                int value = Convert.ToInt32(reader[property.Name]); // Wert aus der Datenbank als Int64 lesen und in Int32 konvertieren
                                property.SetValue(daten, value);
                            }
                            else if (property.Name == "Preis")
                            {
                                decimal value = reader.GetDecimal(reader.GetOrdinal(property.Name));// Wert aus der Datenbank als Decimal lesen
                                property.SetValue(daten, value);
                            }
                            else
                            {
                                property.SetValue(daten, reader[property.Name]);
                            }
                        }
                        return daten;
                    }
                }
            }
            return default;
        }

        public List<BestellungDetail> LadenAlleBestellungen(string tableName)
        {
            List<BestellungDetail> bestellungenListe = new List<BestellungDetail>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var bestellung = new BestellungDetail
                        {
                            ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            Bestellt = reader.GetDateTime(reader.GetOrdinal("Bestellt")),
                            ProduktID = reader.GetInt32(reader.GetOrdinal("ProduktID")),
                            Menge = reader.GetInt32(reader.GetOrdinal("Menge"))
                        };
                        bestellungenListe.Add(bestellung);
                    }
                }
            }
            return bestellungenListe;
        }

        public void Löschen(string tableName, int id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"DELETE FROM {tableName} WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
