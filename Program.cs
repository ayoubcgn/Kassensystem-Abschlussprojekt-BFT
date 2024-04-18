namespace Kassensystem
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=C:\Users\ayyou\source\repos\Kassensystem\Kassensystem\Datenbank.db";

            using (var dbContext = new Datenbank(connectionString))
            {
                dbContext.InitializeDatabase();

                // Menü verwalten
                Menue.MenueVerwalten(dbContext, connectionString);
            }
        }
    }
}
