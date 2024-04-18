namespace Kassensystem
{
    public class Menue
    {
        private readonly Datenbank datenbank;

        public Menue(Datenbank datenbank)
        {
            this.datenbank = datenbank;
        }

        public static void MenueVerwalten(Datenbank datenbank, string connectionString)
        {
            Menue menue = new Menue(datenbank);
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Produkt hinzufügen");
                Console.WriteLine("2. Produkt entfernen");
                Console.WriteLine("3. Alle Produkte anzeigen");
                Console.WriteLine("4. Bestellung für Tisch aufgeben");
                Console.WriteLine("5. Menü verlassen");

                Console.Write("Bitte wählen Sie eine Option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        menue.ProduktHinzufügen();
                        break;
                    case "2":
                        menue.ProduktEntfernen();
                        break;
                    case "3":
                        menue.AlleProdukteAnzeigen();
                        break;
                    case "4":
                        BestellungAufgeben(datenbank);
                        break;
                    case "5":
                        Environment.Exit(0); // Menü verlassen
                        break;
                    default:
                        Console.WriteLine("Ungültige Option. Bitte wählen Sie erneut.");
                        break;
                }
            }
        }


        public void ProduktHinzufügen() // Methode zum Hinzufügen eines Produkts
        {
            Console.Write("Bitte geben Sie den Namen des neuen Produkts ein: ");
            string name = Console.ReadLine();
            decimal preis;
            do
            {
                Console.Write("Bitte geben Sie den Preis des neuen Produkts ein: ");
            }
            while (!decimal.TryParse(Console.ReadLine(), out preis) || preis <= 0);

            // Produkt zur Datenbank hinzufügen
            datenbank.Speichern("Produkt", new Produkt { Name = name, Preis = preis });

            Console.WriteLine("Produkt erfolgreich hinzugefügt.");
        }


        public void ProduktEntfernen()// Methode zum Entfernen eines Produkts
        {
            Console.Write("Bitte geben Sie die ID des zu entfernenden Produkts ein: ");
            if (!int.TryParse(Console.ReadLine(), out int produktId))
            {
                Console.WriteLine("Ungültige Eingabe. Bitte geben Sie eine gültige ID ein.");
                return;
            }


            if (datenbank.ProduktExistiert(produktId))// Produkt aus der Datenbank entfernen, wenn es vorhanden ist
            {
                datenbank.Löschen("Produkt", produktId);
                Console.WriteLine("Produkt erfolgreich entfernt.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Produkt nicht gefunden.");
                Console.ReadLine();
            }
        }

        public void AlleProdukteAnzeigen()// Methode zum Anzeigen aller Produkte im Menü
        {
            var produkte = datenbank.LadenAlleProdukte("Produkt").OrderBy(p => p.Name).ToList();  // Alle Produkte aus der Datenbank laden und nach Namen sortieren
            Console.WriteLine("Alle Produkte im Menü:");
            if (produkte.Count == 0)// Überprüfen, ob das Menü leer ist
            {
                Console.WriteLine("Das Menü ist derzeit leer.");
            }
            else
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-10}", "ID", "Name", "Preis");// Überschriften für ID, Name und Preis ausgeben
                Console.WriteLine(new string('-', 35)); // Trennlinie ausgeben
                foreach (var produkt in produkte)// Produkte ausgeben
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-10}EUR", produkt.ID, produkt.Name, produkt.Preis);
                }
            }
            // Benutzeranweisung zum Fortfahren anzeigen
            Console.WriteLine("Drücken Sie eine beliebige Taste, um fortzufahren...");
            Console.ReadKey();
        }

        static void BestellungAufgeben(Datenbank datenbank)
        {
            Console.WriteLine("Bitte wählen Sie Ihren Tisch (1-10):");
            if (!int.TryParse(Console.ReadLine(), out int tableNumber) || tableNumber < 1 || tableNumber > 10)
            {
                Console.WriteLine("Ungültige Tischnummer. Bitte wählen Sie zwischen 1 und 10.");
                return;
            }

            Console.WriteLine($"Sie haben den Tisch Nr. {tableNumber} ausgewählt.");
            Console.WriteLine("Willkommen zur Bestellungsabwicklung!");

            while (true)
            {
                Console.WriteLine("Bitte geben Sie die ID des zu bestellenden Produkts ein:");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Ungültige Eingabe. Bitte geben Sie eine gültige ID ein.");
                    continue;
                }

                Console.WriteLine("Bitte geben Sie die Menge ein:");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                {
                    Console.WriteLine("Ungültige Eingabe. Bitte geben Sie eine positive Zahl ein.");
                    continue;
                }

                Produkt orderedProduct = datenbank.Laden<Produkt>("Produkt", productId);
                if (orderedProduct == null)
                {
                    Console.WriteLine("Produkt nicht gefunden.");
                    continue;
                }

                // Bestellung speichern
                BestellungDetail orderDetail = new BestellungDetail
                {
                    Bestellt = DateTime.Now,
                    ProduktID = orderedProduct.ID,
                    Menge = quantity
                };
                datenbank.Speichern("BestellungDetail", orderDetail);

                Console.WriteLine($"Produkt '{orderedProduct.Name}' ({quantity}x) erfolgreich bestellt für Tisch Nr. {tableNumber}.");
                Console.WriteLine("Möchten Sie weitere Produkte bestellen? (j/n)");
                string continueOrder = Console.ReadLine();
                if (continueOrder.ToLower() != "j")
                    break;
            }

            ShowTotalOrderCost(datenbank, tableNumber);
            Console.WriteLine("Bestellungsabwicklung beendet.");
            Console.ReadLine();
        }

        static void ShowTotalOrderCost(Datenbank datenbank, int tableNumber)
        {
            decimal totalCost = 0;
            var orders = datenbank.LadenAlleBestellungen("BestellungDetail");
            foreach (var order in orders)
            {
                Produkt product = datenbank.Laden<Produkt>("Produkt", order.ProduktID);
                totalCost += product.Preis * order.Menge;
            }
            Console.WriteLine($"Gesamtsumme für Tisch Nr. {tableNumber}: {totalCost}EUR");
        }
    }
}
