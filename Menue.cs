namespace Kassensystem
{
    // Eine Klasse, die das Menü des Kassensystems verwaltet
    public class Menue
    {
        private readonly Datenbank datenbank; // Datenbankobjekt für den Zugriff auf die Datenbank

        // Konstruktor, der eine Datenbankinstanz benötigt
        public Menue(Datenbank datenbank)
        {
            this.datenbank = datenbank; // Datenbankobjekt setzen
        }

        // Methode, um das Menü anzuzeigen und Optionen zu verwalten
        public static void MenueVerwalten(Datenbank datenbank, string connectionString)
        {
            Menue menue = new Menue(datenbank); // Menüobjekt erstellen
            while (true) // Endlosschleife für das Menü
            {
                Console.Clear(); // Konsolenbildschirm löschen
                // Menüoptionen anzeigen
                Console.WriteLine("1. Produkt hinzufügen");
                Console.WriteLine("2. Produkt entfernen");
                Console.WriteLine("3. Alle Produkte anzeigen");
                Console.WriteLine("4. Bestellung für Tisch aufgeben");
                Console.WriteLine("5. Menü verlassen");

                Console.Write("Bitte wählen Sie eine Option: "); // Benutzer zur Eingabe auffordern
                string option = Console.ReadLine(); // Benutzereingabe lesen

                switch (option) // Optionen auswerten
                {
                    case "1": // Wenn Option 1 ausgewählt wurde
                        menue.ProduktHinzufügen(); // Methode zum Hinzufügen eines Produkts aufrufen
                        break;
                    case "2": // Wenn Option 2 ausgewählt wurde
                        menue.ProduktEntfernen(); // Methode zum Entfernen eines Produkts aufrufen
                        break;
                    case "3": // Wenn Option 3 ausgewählt wurde
                        menue.AlleProdukteAnzeigen(); // Methode zum Anzeigen aller Produkte aufrufen
                        break;
                    case "4": // Wenn Option 4 ausgewählt wurde
                        BestellungAufgeben(datenbank); // Methode zum Aufgeben einer Bestellung aufrufen
                        break;
                    case "5": // Wenn Option 5 ausgewählt wurde
                        Environment.Exit(0); // Programm beenden
                        break;
                    default: // Wenn eine ungültige Option ausgewählt wurde
                        Console.WriteLine("Ungültige Option. Bitte wählen Sie erneut."); // Benutzer benachrichtigen
                        break;
                }
            }
        }

        // Methode zum Hinzufügen eines neuen Produkts
        public void ProduktHinzufügen()
        {
            // Benutzer zur Eingabe von Produktinformationen auffordern
            Console.Write("Bitte geben Sie den Namen des neuen Produkts ein: ");
            string name = Console.ReadLine();
            decimal preis;
            do
            {
                Console.Write("Bitte geben Sie den Preis des neuen Produkts ein: ");
            }
            while (!decimal.TryParse(Console.ReadLine(), out preis) || preis <= 0);

            // Neues Produkt zur Datenbank hinzufügen
            datenbank.Speichern("Produkt", new Produkt { Name = name, Preis = preis });

            Console.WriteLine("Produkt erfolgreich hinzugefügt."); // Erfolgsmeldung anzeigen
        }

        // Weitere Methoden...
    }
}
