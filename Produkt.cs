namespace Kassensystem
{
    // Eine Klasse, die ein Produkt repräsentiert, das im Kassensystem verkauft wird
    public class Produkt
    {
        public int ID { get; set; } // Die eindeutige ID des Produkts
        public string Name { get; set; } // Der Name des Produkts
        public decimal Preis { get; set; } // Der Preis des Produkts

        // Konstruktor, der es ermöglicht, ein Produkt mit allen Eigenschaften zu erstellen
        public Produkt(int id, string name, decimal preis)
        {
            ID = id;
            Name = name;
            Preis = preis;
        }

        // Parameterloser Konstruktor wird benötigt, wenn er verwendet wird
        public Produkt()
        {
        }
    }

}

