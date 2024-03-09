using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace propojeniSDS
{
    public class Zakaznik
    {
        public int Id { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string Email { get; set; }
        public DateTime DatumRegistrace { get; set; }

        public Zakaznik() { }

        public Zakaznik(int id, string jmeno, string prijmeni, string email, DateTime datumRegistrace)
        {
            Id = id;
            Jmeno = jmeno;
            Prijmeni = prijmeni;
            Email = email;
            DatumRegistrace = datumRegistrace;
        }
    }
}
