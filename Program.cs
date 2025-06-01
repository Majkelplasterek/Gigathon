using System;
using System.Diagnostics;
using System.Numerics;
using static Pasjans.Program;

namespace Pasjans
{
    internal class Program
    {
        
        //Klasa odpowiadająca za obsługę jednej karty
        public class Karta
        {
            public Karta(int numer, char typ, bool visible)
            {

                Numer = numer;
                Typ = typ;
                Visible = visible;
            }
            public int Numer { get; set; }
            public char Typ { get; set; }

            public bool Visible { get; set; }

        }

        //Klasa odpowiadająca za przechowanie stosów gry
        public class StosyKart
        {
            public List<StosGry> Grys = new List<StosGry>();
            public List<StosWygrywajacy> Wygrywajace = new List<StosWygrywajacy>();
        }

        //Klasa przechowująca jeden stos kart - te 7 głównych
        public class StosGry
        {

            public Stack<Karta> stosKart = new Stack<Karta>();

        }

        //Klasa przechowująca jeden stos kart - te 4 "Wygrywające"
        public class StosWygrywajacy
        {
            public Stack<Karta> stosWygrywajacy = new Stack<Karta>();
            public char Typ { get; set; }
        }

        //Zeby visual sie nie darł o niestatyczne pola xD
        static StosyKart stosyKart = new StosyKart();

        static Queue<Karta> RezerwowyStos = new Queue<Karta>();
        static bool flagaGry = true;

        //Metoda pozwalająca rozlosować układ kart - na początku i po wybraniu opcji restartu
        static void Rozlosuj(List<string> list)
        {
            stosyKart.Grys.Clear();
            stosyKart.Wygrywajace.Clear();
            RezerwowyStos.Clear();


            Random rnd = new Random();

            //Pętle tworzące 7 stosów głównych
            for (int i = 0; i < 7; i++)
            {
                StosGry stos = new StosGry();

                for (int j = 0; j < i + 1; j++)
                {
                    int los = rnd.Next(0, list.Count);

                    if (list[los].Length == 2)
                    {
                        if (j == i)
                        {
                            //Te takie '0' to konwersja z ASCII na inta 
                            stos.stosKart.Push(new Karta(list[los][0] - '0', list[los][1], true));
                        }
                        else
                        {
                            stos.stosKart.Push(new Karta(list[los][0] - '0', list[los][1], false));
                        }

                    }
                    else
                    {
                        if (j == i)
                        {
                            stos.stosKart.Push(new Karta((list[los][0] - '0') * 10 + list[los][1] - '0', list[los][2], true));
                        }
                        else
                        {
                            stos.stosKart.Push(new Karta((list[los][0] - '0') * 10 + list[los][1] - '0', list[los][2], false));
                        }

                    }

                    list.Remove(list[los]);


                }
                stosyKart.Grys.Add(stos);


            }
            char[] Tab = { '♥', '♠', '♦', '♣' };
            for (int i = 0; i < 4; i++)
            {
                StosWygrywajacy stosWyg = new StosWygrywajacy();
                stosWyg.Typ = Tab[i];
                stosyKart.Wygrywajace.Add(stosWyg);
            }


            //Rozlosowanie kolejności stosu rezerwowego
            while (list.Count != 0)
            {
                int los = rnd.Next(0, list.Count);

                if (list[los].Length == 2)
                {
                    RezerwowyStos.Enqueue(new Karta(list[los][0] - '0', list[los][1], true));
                }
                else
                {
                    RezerwowyStos.Enqueue(new Karta((list[los][0] - '0') * 10 + list[los][1] - '0', list[los][2], true));
                }

                list.RemoveAt(los);
            }

        }

        //Metoda odpowiadająca za ten "przepiękny" interfejs
        static void Namaluj()
        {

            string[] strings = new string[13] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            Console.Clear();
            for (int i = 0; i < stosyKart.Grys.Count; i++)
            {
                var stos = stosyKart.Grys[i];
                Console.Write(i + 1 + ": ");

                foreach (var karta in stos.stosKart.Reverse())
                {
                    if (karta.Visible)
                    {
                        if (karta.Typ == '♥' || karta.Typ == '♦')
                        {

                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.Write(strings[karta.Numer - 1] + karta.Typ.ToString() + " ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("[]" + " ");
                    }

                }

                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nRezerwowy stos to: ");
            Console.ResetColor();
            //Dodatkowy Stos
            if (RezerwowyStos.Count > 0)
            {
                Karta kartaRezerwowa = RezerwowyStos.Peek();
                if (kartaRezerwowa.Typ == '♥' || kartaRezerwowa.Typ == '♦')
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(strings[kartaRezerwowa.Numer - 1] + kartaRezerwowa.Typ.ToString() + " ");
                Console.ResetColor();
            }
            else
            {
                Console.Write("Stos rezerwowy pusty\n");
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\nStosy do układania: ");
            Console.ResetColor();
            for (int i = 0; i < 4; i++)
            {
                if(i % 2 == 0) Console.ForegroundColor = ConsoleColor.Red;
                if (stosyKart.Wygrywajace[i].stosWygrywajacy.Count() == 0)
                {
                    Console.Write("[{0}]  ", stosyKart.Wygrywajace[i].Typ);
                }
                else
                {

                    Console.Write(strings[stosyKart.Wygrywajace[i].stosWygrywajacy.Peek().Numer - 1] + stosyKart.Wygrywajace[i].stosWygrywajacy.Peek().Typ.ToString() + "  ");
                }
                Console.ResetColor();
            }
            Console.WriteLine();

        }

        //Metoda pozwalająca pokazac kolejną kartę w stosie do dokładania
        static void PrzelozRezerwowy()
        {
            if (RezerwowyStos.Count() > 0)
            {
                Karta kartaRezerwowa = RezerwowyStos.Dequeue();
                RezerwowyStos.Enqueue(kartaRezerwowa);

                Namaluj();
            }
            else
            {
                Console.WriteLine("Blad");
            }

        }
        //Przydaje sie by zobaczyc czy kolorki sie zgadzają
        static bool SprawdzPoprawnosc(Karta przekladana, Karta podstawa)
        {
            char[] Tab = { '♥', '♠', '♦', '♣' };
            int indeksPrzekl = 0;
            int indeksPodsta = 0;
            for (int i = 0; i < Tab.Length; i++)
            {
                if (przekladana.Typ == Tab[i])
                {
                    indeksPrzekl = i;
                }
                if (podstawa.Typ == Tab[i])
                {
                    indeksPodsta = i;
                }
            }

            if ((podstawa.Numer == przekladana.Numer + 1) && (indeksPodsta % 2 != indeksPrzekl % 2))
            {
                return true;
            }
            return false;
        }

        //Przekladanie z rezerwowej kolejki do jednego z 7 stosów gry
        static void ZRezerwyNaStos(int indexStosu)
        {
            if (RezerwowyStos.Count() == 0)
            {
                Namaluj();
                return;

            }


            Karta przelozona = RezerwowyStos.Peek();

            //Krół ma troszke inne prawa
            if (przelozona.Numer == 13 && stosyKart.Grys[indexStosu - 1].stosKart.Count == 0)
            {

                RezerwowyStos.Dequeue();

                stosyKart.Grys[indexStosu - 1].stosKart.Push(przelozona);
                Namaluj();
            }
            else if (przelozona.Numer < 13 && stosyKart.Grys[indexStosu - 1].stosKart.Count > 0)
            {
                Karta poprzednia = stosyKart.Grys[indexStosu - 1].stosKart.Peek();
                if (SprawdzPoprawnosc(przelozona, poprzednia))
                {
                    if (RezerwowyStos.Count() != 0)
                    {
                        RezerwowyStos.Dequeue();
                    }
                    stosyKart.Grys[indexStosu - 1].stosKart.Push(przelozona);
                    Namaluj();
                }
                else
                {
                    Namaluj();
                    Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                    
                }
            }
            else
            {
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
            }

        }
        //Metoda pomocnicza, by w razie złego przełożenia oddać do stosu to co z niego zabrano podczas pobierania kart do sprawdzenia
        static void OddajStosowi(Stack<Karta> stos, Stack<Karta> stosDoOddania)
        {
            var lista = stosDoOddania.ToList(); 
            
                          
            stosDoOddania.Clear();                 
            foreach (var item in lista)
            {
                stosDoOddania.Push(item);          
            }


            while (stosDoOddania.Count > 0)
            {
                stos.Push(stosDoOddania.Pop());

            }
            stos.Peek().Visible = true;
        }
        
        //Metoda pomocnicza która stara sie zablokować, gdy uzytkownik poda zbyt duzo kart do przeniesienia 
        static int CheckIle(Stack<Karta> jajo)
        {
            Karta[] karty = jajo.ToArray();
            
            int j = 0;
            for (int i = karty.Length -1 ; i > 0; i--, j++)
            {
                if (karty[i].Visible == false)
                {

                    return j;
                }
            }

            return karty.Length;
        }

        //Przekładanie między stosami - różne ilosci kart
        static void ZStosuNaStos(int indexStosuZ, int indexStosuDo, int odElementu)
        {
            Stack<Karta> kartyPrzenoszone = new Stack<Karta>();
            int ileMoznaZabrac = CheckIle(new Stack<Karta>(stosyKart.Grys[indexStosuZ - 1].stosKart));
            Console.WriteLine(ileMoznaZabrac);
            


            if (odElementu > ileMoznaZabrac || indexStosuZ > 7 || indexStosuDo > 7 || indexStosuZ < 1 || indexStosuDo < 1 || stosyKart.Grys[indexStosuZ - 1].stosKart.Count == 0)
            {
                
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                return;
            }
            //Pobranie kart z stosu OD
            for(int i = 0;  i < odElementu; i++)
            {
                kartyPrzenoszone.Push(stosyKart.Grys[indexStosuZ - 1].stosKart.Pop());
            }
            Stack<Karta> kartyPrzenoszoneKopia = new Stack<Karta>(kartyPrzenoszone);

            //jak król to inne prawa
            if (kartyPrzenoszone.Peek().Numer == 13 && stosyKart.Grys[indexStosuDo - 1].stosKart.Count == 0)
            {
                while (kartyPrzenoszone.Count > 0)
                {
                    stosyKart.Grys[indexStosuDo - 1].stosKart.Push(kartyPrzenoszone.Pop());
                    
                }
                //Odkrywanie ostatniej karty
                if (stosyKart.Grys[indexStosuZ - 1].stosKart.Count > 0)
                {
                    Karta kartaPod = stosyKart.Grys[indexStosuZ - 1].stosKart.Peek();
                    kartaPod.Visible = true;
                }
                
                Namaluj();
                
                return;

            }
            else if(kartyPrzenoszone.Peek().Numer < 13 && stosyKart.Grys[indexStosuDo - 1].stosKart.Count > 0)
            {
                Karta kartaNa = stosyKart.Grys[indexStosuDo - 1].stosKart.Peek();

                if (SprawdzPoprawnosc(kartyPrzenoszone.Peek(), kartaNa))
                {

                    while (kartyPrzenoszone.Count > 0)
                    {
                        stosyKart.Grys[indexStosuDo - 1].stosKart.Push(kartyPrzenoszone.Pop());
                        
                    }
                    //Odkrywanie ostatniej karty
                    if (stosyKart.Grys[indexStosuZ - 1].stosKart.Count > 0)
                    {
                        Karta kartaPod = stosyKart.Grys[indexStosuZ - 1].stosKart.Peek();
                        kartaPod.Visible = true;
                    }

                    
                }
                else
                {
                    OddajStosowi(stosyKart.Grys[indexStosuZ - 1].stosKart, kartyPrzenoszoneKopia);
                    Namaluj();
                    Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                    return;
                }
                
                Namaluj();
                return;
            }
            else
            {
                OddajStosowi(stosyKart.Grys[indexStosuZ - 1].stosKart, kartyPrzenoszoneKopia);
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");

            }


        }
        //Po interakcji z stosem do odkładania nań kart sprawdzamy czy to juz koniec
        static bool CheckWygrywanie()
        {
            int suma = 0;
            for (int i = 0; i < 4; i++)
            {
                suma += stosyKart.Wygrywajace[i].stosWygrywajacy.Count;
            }

            if (suma == 52) return true;
            return false;
        }

        //Mozna przelozyc od razu z kolejki do wygrywających
        static void ZRezerwyNaWygrywajacy()
        {
            if(RezerwowyStos.Count() == 0)
            {
                Namaluj();
                return;
            }
            Karta przelozona = RezerwowyStos.Peek();
            int indexStosu = 0;
            for(int i = 0; i < 4; i++)
            {
                if(stosyKart.Wygrywajace[i].Typ == przelozona.Typ)
                {
                    indexStosu = i;
                }
            }

            //jak as to inne prawa
            if (przelozona.Numer == 1 && stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Count == 0)
            {
                if(RezerwowyStos.Count() != 0)
                {
                    RezerwowyStos.Dequeue();
                }
                
                stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Push(przelozona);
            }
            else if (przelozona.Numer > 1 && stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Count > 0) 
            {
                if(stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Peek().Numer + 1 == przelozona.Numer)
                {
                    if (RezerwowyStos.Count() != 0)
                    {
                        RezerwowyStos.Dequeue();
                    }
                    stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Push(przelozona);
                }
                else
                {
                    Namaluj();
                    Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                    return;
                }

            }
            else
            {
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                return;
            }
            if (CheckWygrywanie())
            {
                Console.Clear();
                Console.WriteLine("GRATULACJE WYGRANA!");
                flagaGry = false;
                return;
            }

            Namaluj();
        }

        //Przekładanie z stosów gry na wygrywające
        static void ZGryNaWygrywajacy(int index)
        {
            Karta przelozona = stosyKart.Grys[index - 1].stosKart.Peek();
            int indexStosu = 0;
            for (int i = 0; i < 4; i++)
            {
                if (stosyKart.Wygrywajace[i].Typ == przelozona.Typ)
                {
                    indexStosu = i;
                }
            }
            if (przelozona.Numer == 1 && stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Count == 0)
            {
                stosyKart.Grys[index - 1].stosKart.Pop();
                stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Push(przelozona);
            }
            else if (przelozona.Numer > 1 && stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Count > 0)
            {
                if (stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Peek().Numer + 1 == przelozona.Numer)
                {
                    stosyKart.Grys[index - 1].stosKart.Pop();
                    stosyKart.Wygrywajace[indexStosu].stosWygrywajacy.Push(przelozona);
                }
                else
                {
                    Namaluj();
                    Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                    return;
                }

            }
            else
            {
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                return;
            }

            if (stosyKart.Grys[index - 1].stosKart.Count > 0)
            {
                stosyKart.Grys[index - 1].stosKart.Peek().Visible = true;
            }
            if (CheckWygrywanie())
            {
                Console.Clear();
                Console.WriteLine("GRATULACJE WYGRANA!");
                flagaGry = false;
                return;
            }

            Namaluj();
        }
        
        //Czasem przyda sie odwrócić stan rzeczy
        static void ZWygrywajacegoNaGry(int indexWyg, int indexGry)
        {
            if (stosyKart.Wygrywajace[indexWyg - 1].stosWygrywajacy.Count > 0)
            {
                Karta Przekladana = stosyKart.Wygrywajace[indexWyg - 1].stosWygrywajacy.Peek();
                Karta Na = stosyKart.Grys[indexGry - 1].stosKart.Peek();


                if(SprawdzPoprawnosc(Przekladana, Na))
                {
                    stosyKart.Wygrywajace[indexWyg - 1].stosWygrywajacy.Pop();
                    stosyKart.Grys[indexGry - 1].stosKart.Push(Przekladana);
                }
                else
                {
                    Namaluj();
                    Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                    return;
                }

            }
            else
            {
                Namaluj();
                Console.WriteLine("\n\n NIEPRAWIDŁOWY RUCH! \n");
                return;
            }

            Namaluj();
            
        }

        static void Main(string[] args)
        {
            //Zeby konsola ladnie wyswietlała znaczki UTF8
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c chcp 65001",
                CreateNoWindow = true,
                UseShellExecute = false
            });

            Console.OutputEncoding = System.Text.Encoding.UTF8;


            Console.WriteLine("Oto gra w Pasjansa. Skróty: \nStos rezerwowy to: Rez\nStosy główne to: Gł\nStos do układania kart by wygrać: Ukl\n[enter]");
            Console.ReadLine();


            //Zapewne dało się mądrzej :)
            List<string> list = new List<string>{ "1♥", "2♥", "3♥",  "4♥", "5♥", "6♥", "7♥", "8♥", "9♥", "10♥", "11♥", "12♥", "13♥", "1♦", "2♦", "3♦", "4♦", "5♦", "6♦", "7♦", "8♦", "9♦", "10♦", "11♦", "12♦", "13♦", "1♠", "2♠", "3♠", "4♠", "5♠", "6♠", "7♠", "8♠", "9♠", "10♠", "11♠", "12♠", "13♠", "1♣", "2♣", "3♣", "4♣", "5♣", "6♣", "7♣", "8♣", "9♣", "10♣", "11♣", "12♣", "13♣" };

            Rozlosuj(new List<string>(list));
            Namaluj();


            string input = "";
            int index = 0;
            int index2 = 0;
            int ilosc = 0;
            do
            {
                if(flagaGry == false)
                {
                    break;
                }
                //Interfejs i przechwytywanie błędnie wprowadzonych danych
                try{
                    Console.Write("\nAKCJE:\nR: Przeloz stos Rezerwowy \n\nRG: Rez => Gł \nGG: Między Gł a Gł \nRU: Rez => Ukl \nGU: Gł => Ukl\nUG: Ukl => Gł\n\nT: Partia od nowa\nX: Zakoncz program\nOpcja:");
                    input = Console.ReadLine();
                    input = input.ToLower();

                    switch (input)
                    {
                        case "r": PrzelozRezerwowy(); break;
                        case "rg":
                            Console.Write("Do jakiego wiersza?: ");
                            index = int.Parse(Console.ReadLine());
                            ZRezerwyNaStos(index);
                            break;

                        case "gg":
                            Console.Write("Z jakiego wiersza?: ");
                            index = int.Parse(Console.ReadLine());
                            Console.Write("Do jakiego wiersza?: ");
                            index2 = int.Parse(Console.ReadLine());
                            Console.Write("Ile kart?: ");
                            ilosc = int.Parse(Console.ReadLine());
                            ZStosuNaStos(index, index2, ilosc);
                            break;
                        case "ru":
                            ZRezerwyNaWygrywajacy(); break;
                        case "gu":
                            Console.Write("Z ktorego wiersza?: ");
                            index = int.Parse(Console.ReadLine());
                            ZGryNaWygrywajacy(index);
                            break;
                        case "ug":
                            Console.Write("Z którego stosu?: ");
                            index = int.Parse(Console.ReadLine());
                            Console.Write("Do jakiego wiersza?: ");
                            index2 = int.Parse(Console.ReadLine());
                            ZWygrywajacegoNaGry(index, index2);

                            break;
                        case "t":
                            Rozlosuj(new List<string>(list));
                            Namaluj(); break;
                        default: Namaluj();break;

                    }
                }
                catch(Exception e){
                   
                    Namaluj();
                    Console.WriteLine("\n\n ZLE DANE! \n");
                }
                

            } while (input != "x");

        }
    }
}
//Di ęt
