namespace Kassensystem
{
    public class Produkt
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Preis { get; set; }

        // Konstruktor
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
