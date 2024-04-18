using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kassensystem
{
    // Eine Klasse, die den Zugriff auf eine SQLite-Datenbank ermöglicht
    public class Datenbank : DbContext
    {
        string connectionString; // Die Zeichenfolge, die die Verbindung zur Datenbank definiert

        // Konstruktor der Datenbankklasse
        public Datenbank(string connectionString) : base()
        {
            this.connectionString = connectionString; // Die Verbindungszeichenfolge setzen
        }

        // Methode zum Vorbereiten der Datenbank: Tabellen erstellen, wenn sie nicht existieren
        public void InitializeDatabase()
        {
            // Verbindung zur Datenbank herstellen
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open(); // Datenbankverbindung öffnen
                ErstellenTabelleWennNichtExistiert(connection); // Methode aufrufen, um Tabellen zu erstellen
            }
        }

        // Methode zum Erstellen von Tabellen, wenn sie nicht existieren
        public void ErstellenTabelleWennNichtExistiert(SqliteConnection connection)
        {
            // SQL-Abfrage zum Erstellen der Tisch-Tabelle
            string createTischTableQuery = @"
                CREATE TABLE IF NOT EXISTS Tisch (ID INTEGER PRIMARY KEY)";
            using (var command = new SqliteCommand(createTischTableQuery, connection))
            {
                command.ExecuteNonQuery(); // SQL-Abfrage ausführen
            }

            // Weitere Tabellen werden ähnlich erstellt...
        }

        // Methode zum Speichern von Daten in die Datenbank
        public void Speichern(string tableName, object daten)
        {
            // Verbindung zur Datenbank herstellen
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open(); // Datenbankverbindung öffnen
                var command = connection.CreateCommand(); // Befehlsobjekt erstellen

                // Eigenschaften des Objekts abrufen, ID ausschließen
                var properties = daten.GetType().GetProperties().Where(p => p.Name != "ID");

                // Spaltennamen und Platzhalter für Werte erstellen
                var columns = string.Join(", ", properties.Select(p => p.Name));
                var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

                // SQL-Insert-Befehl erstellen
                command.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

                // Parameter für den Befehl hinzufügen
                foreach (var property in properties)
                {
                    command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(daten));
                }
                command.ExecuteNonQuery(); // Insert-Befehl ausführen
            }
        }

        // Weitere Methoden...
    }
}
