using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace propojeniSDS
{
    public class Produkt
    {
        public int Id { get; set; }
        public string Nazev { get; set; }
        public string Popis { get; set; }
        public float Cena { get; set; }
        public bool Skladem { get; set; }
        
        public Produkt() { }

        public Produkt(int id, string nazev, string popis, float cena, bool skladem)
        {
            Id = id;
            Nazev = nazev;
            Popis = popis;
            Cena = cena;
            Skladem = skladem;
        }
    }
}
