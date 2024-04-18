namespace Kassensystem
{
    // Eine Klasse, die Details einer Bestellung repräsentiert
    public class BestellungDetail
    {
        public int ID { get; set; } // Die eindeutige ID der Bestelldetails
        public DateTime Bestellt { get; set; } // Das Datum und die Uhrzeit, zu der die Bestellung aufgegeben wurde
        public int ProduktID { get; set; } // Die ID des bestellten Produkts
        public int Menge { get; set; } // Die Menge des bestellten Produkts
    }
}
