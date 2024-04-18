namespace Kassensystem
{
    // Die Hauptklasse des Programms
    class Program
    {
        // Die Hauptmethode, die beim Start des Programms ausgeführt wird
        static void Main(string[] args)
        {
            // Die Verbindungszeichenfolge zur SQLite-Datenbankdatei
            string connectionString = @"Data Source=C:\Users\ayyou\source\repos\Kassensystem\Kassensystem\Datenbank.db";

            // Eine Datenbankinstanz erstellen und die Datenbank initialisieren
            using (var dbContext = new Datenbank(connectionString))
            {
                dbContext.InitializeDatabase();

                // Das Menü verwalten und die Kontrolle an die Menüklasse übergeben
                Menue.MenueVerwalten(dbContext, connectionString);
            }
        }
    }
}
