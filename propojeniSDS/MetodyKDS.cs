using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Xml.Linq;

namespace propojeniSDS
{
    public class MetodyKDS
    {

        public List<Zakaznik> zakaznici = new List<Zakaznik>();

        public List<Produkt> produkty = new List<Produkt>();

        /// <summary>
        ///     Metoda pro zjištění, jestli vybrané id se nachází v databázi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int? VyberZakaznikaPodleID(int id)
        {
            if (zakaznici.Any(z => z.Id == id)) 
            {
                Console.WriteLine("Zákazník se shoduje ");
                return id;
            }
            else
            {
                Console.WriteLine("Zákazník s tímto ID nebyl nalezen.");
                return null;
            }
        }
        /// <summary>
        ///     Metoda vypíše všechny zákazníky a přidá je do listu zákazníků
        /// </summary>
        public void NacistZakazniky()
        {
            string query = "SELECT * from Zakaznik";
            DataTable dt = PripojeniKDS.Pripojeni.ExecuteQuery(query);

            zakaznici.Clear();

            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["id"]);
                string jmeno = Convert.ToString(dr["jmeno"]);
                string prijmeni = Convert.ToString(dr["prijmeni"]);
                string email = Convert.ToString(dr["email"]);
                DateTime datumReg = Convert.ToDateTime(dr["datum_registrace"]);

                Console.WriteLine(id + ". " + jmeno + " " + prijmeni);
                Zakaznik zak = new Zakaznik(id, jmeno, prijmeni, email, datumReg);

                zakaznici.Add(zak);

            }
        }

        /// <summary>
        /// Metoda z XML souboru, vybere zákazníky a přidá je do databáze
        /// </summary>
        /// <param name="filePath"></param>
        public void VlozitDoZakaznikaZXML(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var zakaznici = doc.Root.Elements("Zakaznik");

            foreach (var zakaznik in zakaznici)
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Jmeno", zakaznik.Element("Jmeno").Value),
                    new SqlParameter("@Prijmeni", zakaznik.Element("Prijmeni").Value),
                    new SqlParameter("@Email", zakaznik.Element("Email").Value),
                    new SqlParameter("@DatumRegistrace", DateTime.Parse(zakaznik.Element("DatumRegistrace").Value))
                };

                string query = "INSERT INTO Zakaznik (jmeno, prijmeni, email, datum_registrace) VALUES (@Jmeno, @Prijmeni, @Email, @DatumRegistrace);";

                int result = PripojeniKDS.Pripojeni.ExecuteNonQuery(query, parameters);

                if (result > 0)
                {
                    Console.WriteLine("Zákazník " + zakaznik.Element("Jmeno").Value + " " + zakaznik.Element("Prijmeni").Value + " byl úspěšně vložen.");
                }
                else
                {
                    Console.WriteLine("Při vkládání zákazníka " + zakaznik.Element("Jmeno").Value + " " + zakaznik.Element("Prijmeni").Value + " došlo k chybě.");
                }

            }
        }
        /// <summary>
        ///     Metoda z XML souboru, vybere produkty a přidá je do databáze
        /// </summary>
        /// <param name="filePath"></param>
        public void VlozitDoProduktZXML(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var produkty = doc.Root.Elements("Produkt");

            foreach (var produkt in produkty)
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Nazev", produkt.Element("Nazev").Value),
                    new SqlParameter("@Popis", produkt.Element("Popis").Value),
                    new SqlParameter("@Cena", produkt.Element("Cena").Value),
                    new SqlParameter("@Skladem", produkt.Element("Skladem").Value)
                };
                


                string query = "INSERT INTO Produkt (nazev, popis, cena, skladem) VALUES (@Nazev, @Popis, @Cena, @Skladem);";

                int result = PripojeniKDS.Pripojeni.ExecuteNonQuery(query, parameters);
                
                if (result > 0)
                {
                    Console.WriteLine("Prodkut " + produkt.Element("Nazev").Value + " : " + produkt.Element("Popis").Value + " za :" + produkt.Element("Cena").Value + " byl úspěšně vložen.");
                }
                else
                {
                    Console.WriteLine("Prodkut " + produkt.Element("Nazev").Value + " " + produkt.Element("Popis").Value + " byl úspěšně vložen.");
                }
            }
        }

        /// <summary>
        ///     Metoda přidá zákazníka do databáze
        /// </summary>
        /// <param name="zakaznik"></param>
        /// <returns></returns>
        public int VlozeniJednohoZakaznika(Zakaznik zakaznik)
        {
            string query = @"INSERT INTO Zakaznik (jmeno, prijmeni, email, datum_registrace) 
                     OUTPUT 
                        INSERTED.id
                     VALUES (@Jmeno, @Prijmeni, @Email, @DatumRegistrace)";

            // Vytvoření pole parametrů pro parametrizovaný dotaz
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Jmeno", zakaznik.Jmeno),
                new SqlParameter("@Prijmeni", zakaznik.Prijmeni),
                new SqlParameter("@Email", zakaznik.Email),
                new SqlParameter("@DatumRegistrace", zakaznik.DatumRegistrace)
            };
            int zakaznikId = PripojeniKDS.Pripojeni.ExecuteScalarInt(query, parameters);
            zakaznik.Id = zakaznikId;
            

            if (zakaznikId > 0)
            {
                Console.WriteLine("Nový zákazník byl přidělen k objednávce");
            } else Console.WriteLine("Nového zákazníka se nepovedlo přidat");

            return zakaznikId;
        }


        /// <summary>
        ///     Metoda přidá celou objednávku do databáze
        /// </summary>
        /// <param name="datumOb"></param>
        /// <param name="zakaznikId"></param>
        /// <param name="celkovaC"></param>
        /// <param name="stav"></param>
        public void VlozeniObjednavky(string datumOb, int zakaznikId, decimal celkovaC, bool stav)
        {
            // Parametrizovaný SQL dotaz
            string query = "INSERT INTO Objednavka (datum_objednavky,zakaznik_id, celkova_cena, stav_objednavky) VALUES (@datumObjednavky,@zakaznikId, @celkovaCena, @stavObjednavky)";

            // Vytvoření pole SQL parametrů
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@datumObjednavky", datumOb),
                new SqlParameter("@zakaznikId", zakaznikId),
                new SqlParameter("@celkovaCena", celkovaC),
                new SqlParameter("@stavObjednavky", stav)
            };

            // Spuštění dotazu s parametry
            int dt = PripojeniKDS.Pripojeni.ExecuteNonQuery(query, parameters);
            if (dt > 0)
            {
                Console.WriteLine($"Vkládám objednávku: \n" +
                    $"Datum objednávky: {datumOb}, Zakazník id: {zakaznikId}, Cena objednávky: {celkovaC}, stav objednávky {stav}");
            }
            else
            {
                Console.WriteLine($" Při vkládání objednávky: Datum objednávky: {datumOb}, Zakazník id: {zakaznikId}, Cena objednávky: {celkovaC}, stav objednávky {stav} došlo k chybě");
            }
        }
    }
}

