using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace propojeniSDS
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Vložit produkty do databáze: 1");
            Console.WriteLine("Vložit zákazníky do databáze: 2");
            Console.WriteLine("Vložit Objednávku: 3");
            string vstup = Console.ReadLine();
        
            // Získání cesty k aktuálnímu adresáři aplikace
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Složení cesty k souboru ve složce Data
            string pathProdukty = Path.Combine(basePath, "Data", "Produkty.xml");

            // Kontrola, zda soubor existuje
            if (File.Exists(pathProdukty))
            {
                Console.WriteLine($"Soubor byl nalezen: {pathProdukty}");
            }
            else
            {
                Console.WriteLine("Soubor nebyl nalezen.");
            }

            if (vstup == "1")
            {
                MetodyKDS dsmet = new MetodyKDS();
                dsmet.VlozitDoProduktZXML(pathProdukty);
            }

            string pathZakaznici = Path.Combine(basePath, "Data", "Zakaznici.xml");

            if (File.Exists(pathZakaznici))
            {
                Console.WriteLine($"Soubor byl nalezen: {pathZakaznici}");
            }
            else
            {
                Console.WriteLine("Soubor nebyl nalezen.");
            }

            if (vstup == "2")
            {
                MetodyKDS metodyKDS = new MetodyKDS();

                for (int i = 0; i < 1; i++)
                {
                    metodyKDS.VlozitDoZakaznikaZXML(pathZakaznici);

                }
            }

            
            if (vstup == "3")
            {
                
                string datumObjednavky = GetDatumObjednavky();

                int idZak = GetIdZakaznika();

                decimal cenaObjednavky = GetCenuObjednavky();
                
                bool stavObjednavky = GetStavObjednavky();


                MetodyKDS metodyKDS = new MetodyKDS();
                metodyKDS.VlozeniObjednavky(datumObjednavky, idZak, cenaObjednavky, stavObjednavky);
            }
        }

        /// <summary>
        ///     Metoda, které ohlídá, že uživatel vložil datum objednávky ve správném formátu
        /// </summary>
        /// <returns></returns>
        static string GetDatumObjednavky()
        {
            string datumObjednavky;
            Regex regexDatum = new Regex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}$");

            while (true)
            {
                Console.WriteLine("Zadejte datum objednávky (RRRR-MM-DD HH:MM:SS.fff):");
                datumObjednavky = Console.ReadLine();

                if (regexDatum.IsMatch(datumObjednavky))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Datum objednávky není ve správném formátu, zkuste to prosím znovu.");
                }
            }

            return datumObjednavky;
        }

        /// <summary>
        ///      Metoda, které ohlídá, že uživatel vložil cenu objednávky ve správném formátu
        /// </summary>
        /// <returns></returns>
        static decimal GetCenuObjednavky()
        {
            decimal cenaObjednavky;
            Regex regexCena = new Regex(@"^\d+(\.\d{1,2})?$");

            while (true)
            {
                Console.WriteLine("Zadejte cenu objednávky (kladné číslo, může obsahovat desetinná místa):");
                string inputCena = Console.ReadLine();

                if (regexCena.IsMatch(inputCena) && decimal.TryParse(inputCena, out cenaObjednavky) && cenaObjednavky > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Cena objednávky není ve správném formátu nebo je záporná, zkuste to prosím znovu.");
                }
            }

            return cenaObjednavky;
        }
        /// <summary>
        ///  Metoda, které ohlídá, že uživatel vložil stav objednávky ve správném formátu
        /// </summary>
        /// <returns></returns>
        static bool GetStavObjednavky()
        {
            while (true)
            {
                Console.WriteLine("Zadejte stav objednávky (1 pro 'Doručena' nebo 0 pro 'Na cestě/Vyřizuje se'):");
                string inputBool = Console.ReadLine();

                if (inputBool == "1")
                {
                    return true;
                }
                else if (inputBool == "0")
                {
                    return false; 
                }
                else
                {
                    Console.WriteLine("Stav objednávky není ve správném formátu, zkuste to prosím znovu.");
                }
            }
        }

        /// <summary>
        ///  Metoda, kterou uživatel vloží id zákazníka. Buďto vybere zákazníka do objednávky přímo z databáze (tabulka uživatelů) a 
        ///  nebo vloží nového uživatele
        /// </summary>
        /// <returns></returns>
        static int GetIdZakaznika()
        {
            MetodyKDS metodyKDS = new MetodyKDS();

            while (true)
            {
                Console.WriteLine("Zadej příjmení zákazníka \n 1. Vytvořit nového zákazníka \n 2. Vložit zákazníka z databáze");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    Console.WriteLine("Zadej jméno zákazníka");
                    string jmeno = Console.ReadLine();
                    Console.WriteLine("Zadej příjmení zákazníka");
                    string prijmeni = Console.ReadLine();
                    Console.WriteLine("Zadej email zákazníka");
                    string email = Console.ReadLine();

                    DateTime dt = DateTime.Now;

                    Zakaznik novyZakaznik = new Zakaznik()
                    {
                        Jmeno = jmeno,
                        Prijmeni = prijmeni,
                        Email = email,
                        DatumRegistrace = dt
                    };

                    int zakaznikId = metodyKDS.VlozeniJednohoZakaznika(novyZakaznik);
                    
                    Console.WriteLine("\nVyber zákazníka: \n");
                    MetodyKDS mtdkds = new MetodyKDS();
                    mtdkds.NacistZakazniky();
                    string index = Console.ReadLine();


                    int number;
                    bool success = int.TryParse(input, out number);

                    if (success)
                    {

                        int? vybraneID = mtdkds.VyberZakaznikaPodleID(number);

                        return vybraneID.Value;


                    } else Console.WriteLine("Zadaný řetězec není platné číslo.");

                    break;
                }
               

                else if (input == "2")
                {
                    Console.WriteLine("Vyber zákazníka: \n");
                    MetodyKDS mtdkds = new MetodyKDS();
                    mtdkds.NacistZakazniky();
                    string index = Console.ReadLine();
                    int number;
                    bool success = int.TryParse(input, out number);

                    if (success)
                    {

                        int? vybraneID = mtdkds.VyberZakaznikaPodleID(number);

                        return vybraneID.Value;


                    }
                    else Console.WriteLine("Zadaný řetězec není platné číslo.");

                    continue;
                }
                    
                else Console.WriteLine("Špatně zadaná hodnota \n 1. Pro vytvoření nového uživetele \n 2. pro načtení zákazníka z databáze");
                break;
                
            }
            return -1;
        }
    }
}