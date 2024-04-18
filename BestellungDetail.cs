namespace Kassensystem
{
    public class BestellungDetail
    {
        public int ID { get; set; }
        public DateTime Bestellt { get; set; }
        public int ProduktID { get; set; }
        public int Menge { get; set; }
    }
}
