# C#

Programme:
- erstes Beispielprogramm mit Ein- und Ausgabe

```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Enter Your Name: ");
            string name = System.Console.ReadLine();
            System.Console.WriteLine("Enter your age: ");
            int age = int.Parse(System.Console.ReadLine());
            System.Console.WriteLine("Enter a decimal number: ");
            double komm = double.Parse(System.Console.ReadLine());

            System.Console.WriteLine("Name: " + name + "\tage: " + age + "\tdec: " + komm);



            System.Console.ReadKey();
        }
    }
}
```

- Beispielprogramm einfacher Rechner:

```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            double zahl1;
            double zahl2;

            System.Console.WriteLine("Gib die erste Zahl ein:");
            zahl1 = double.Parse(System.Console.ReadLine());

            System.Console.WriteLine("\nGib die zweite Zahl ein:");
            zahl2 = double.Parse(System.Console.ReadLine());

            System.Console.WriteLine("Summe: " + zahl1 + " + " + zahl2 + " = " + (zahl1 + zahl2));
            System.Console.WriteLine("Differenz: " + zahl1 + " - " + zahl2 + " = " + (zahl1 - zahl2));
            System.Console.WriteLine("Produkt: " + zahl1 + " * " + zahl2 + " = " + (zahl1 * zahl2));
            System.Console.WriteLine("Quotient: " + zahl1 + " / " + zahl2 + " = " + (zahl1 / zahl2));

        }
    }
}
```

- Beispielprogramm mit Strings

```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "Ei_nlang7esw+ort";
            Console.WriteLine(text);
            text = text.Replace("_", "");
            text = text.Replace("7", "");
            text = text.Replace("+", "");
            Console.WriteLine("1. " + text);

            text = text.Insert(3, " ");
            text = text.Insert(10, " ");
            Console.WriteLine("2. " + text);

            string[] woerter = text.Split(" ");
            text = woerter[0] + " " + woerter[1] + " " + woerter[2].Substring(0, 1).ToUpper() + woerter[2].Substring(1);
            Console.WriteLine("3. " + text);
            
        }
    }
}
```

- Aufgabe 3 - Klassen
```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            //1.
            Hund hund = new Hund("Schäferhund", "Sam", 5);

            Console.WriteLine(hund.Rasse);
            Console.WriteLine(hund.Name);
            Console.WriteLine(hund.Alter);
            hund.Bellen();

            //2.
            Berechnungen rechnung = new Berechnungen();
            Console.WriteLine(rechnung.Pythagoras(3, 4));
        }
    }

    //1.
    class Hund
    {
        public string Rasse { get; set; }
        public string Name { get; set; }
        public int Alter { get; set; }

        public Hund(string rasse, string name, int alter)
        {
            Rasse = rasse;
            Name = name;
            Alter = alter;
        }

        public void Bellen()
        {
            Console.WriteLine("Wuff");
        }
    }

    //2.
    class Berechnungen
    {
        public double Pythagoras(double a, double b)
        {
            return (a * a) + (b * b);
        }
    }
}
```

- Übung zu Switch-Case:
```C#
namespace Main
{
    class Program
    {
        public static void Ampel()
        {
            System.Console.WriteLine("\n");
            System.Console.WriteLine("Choose color:\n1: red\n2: green\n3: yellow");
            string? color = System.Console.ReadLine();
            switch(color)
            {
                case "1":
                    System.Console.WriteLine("\nStop!\n\n");
                    break;
                case "2":
                    System.Console.WriteLine("\nGo!\n\n");
                    break;
                case "3":
                    System.Console.WriteLine("\nWait!\n\n");
                    break;
                default:
                    System.Console.WriteLine("Invalid Imput");
                    break;
            }
        }

        public static void Run()
        {
            bool cont = true;
            while (cont)
            {
                System.Console.WriteLine("Continue? Y/N");
                string? input = Console.ReadLine();
                switch (input)
                {
                    case "Y":
                        Ampel();
                        break;
                    case "y":
                        Ampel();
                        break;
                    case "N":
                        System.Console.WriteLine("Bye\n");
                        cont = false;
                        break;
                    case "n":
                        System.Console.WriteLine("Bye\n");
                        cont = false;
                        break;
                    default:
                        System.Console.WriteLine("Invalid Input!\n");
                        cont = true;
                        break;
                }
            }
            return;
        }
        static void Main(string[] args)
        {
            Run();
        }
    }
}
```

- Quiz Programm:
```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            Console.WriteLine("Quiz:\n-----\n\nWillst du starten? Y/N");
            while(run)
            {
                string? input = Console.ReadLine();
                if (input.Equals("Y") || input.Equals("y"))
                {
                    bool answer = true;
                    int punktzahl = 0;
                    Console.WriteLine("Quiz:\n-----\n\nWas ist 3 + 4?");
                    string loesung1 = Console.ReadLine();
                    if (loesung1.Equals("7"))
                    {
                        Console.WriteLine("Richtig :)\n\n");
                        punktzahl++;
                    }
                    else
                    {
                        Console.WriteLine("Leider Falsch :(\n\n");
                    }
                    Console.WriteLine("Quiz:\n-----\n\nWas ist 6 + 5?");
                    loesung1 = Console.ReadLine();
                    if (loesung1.Equals("11"))
                    {
                        Console.WriteLine("Richtig :)\n\n");
                        punktzahl++;
                    }
                    else
                    {
                        Console.WriteLine("Leider Falsch :(\n\n");
                    }
                    Console.WriteLine("Quiz:\n-----\n\nWas ist 4 * 9?");
                    loesung1 = Console.ReadLine();
                    if (loesung1.Equals("36"))
                    {
                        Console.WriteLine("Richtig :)\n\n");
                        punktzahl++;
                    }
                    else
                    {
                        Console.WriteLine("Leider Falsch :(\n\n");
                    }
                    Console.WriteLine("Quiz:\n-----\n\nWie heißt die Landeshauptstadt von Bayern?");
                    loesung1 = Console.ReadLine();
                    if (loesung1.Equals("München"))
                    {
                        Console.WriteLine("Richtig :)\n\n");
                        punktzahl++;
                    }
                    else
                    {
                        Console.WriteLine("Leider Falsch :(\n\n");
                    }
                    Console.WriteLine("Quiz:\n-----\n\nWie viele Bundesländer hat Deutschland?");
                    loesung1 = Console.ReadLine();
                    if (loesung1.Equals("16"))
                    {
                        Console.WriteLine("Richtig :)\n\n");
                        punktzahl++;
                    }
                    else
                    {
                        Console.WriteLine("Leider Falsch :(\n\n");
                    }
                    Console.WriteLine("Quiz:\n-----\n\nAuswertung:\n-----------");
                    if (punktzahl == 5)
                    {
                        Console.WriteLine("Du hast die volle Punktzahl (5/5 Punkten) erreicht, herzlichen Glückwunsch :)\n");
                        run = false;
                    }
                    else if (punktzahl == 4)
                    {
                        Console.WriteLine("Du hast das Quiz gewonnen mit 4/5 Punkten.\n");
                        run = false;
                    }
                    else
                    {
                        Console.WriteLine("Du hast leider nicht gewonnen.\nDeine Punktzahl war: " + punktzahl + "\nBitte versuche es nochmal");
                        Console.WriteLine("Willst du es nochmal versuchen? Y/N");
                        run = true;
                    }
                }
                else if(input.Equals("N") || input.Equals("n"))
                {
                    Console.WriteLine("Bis Bald.\n");
                    run = false;
                }
                else
                {
                    Console.WriteLine("ungültige Eingabe.\n Willst du Starten? Y/N");
                    run = true;
                }
            }
        }
    }
}
```

- Übungsaufgabe 5 - Arrays und loops
- 1.
```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("bitte gib die Länge des Arrays an: ");
            int len = int.Parse(Console.ReadLine());

            int[] array = new int[len];
            for(int i = 0; i <= len - 1; i++)
            {
                System.Console.WriteLine("Gib dein " + (i + 1) + ". Element ein: ");
                array[i] = int.Parse(Console.ReadLine());
            }
            int sum = 0;
            for (int i = 0; i <= len - 1; i++)
            {
                sum = sum + array[i];
            }

            double a = (double)sum / (double)len;
            System.Console.WriteLine(a);

        }
    }
}

```
- 2.
```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Collections.Generic.Dictionary<string, string> words = new Dictionary<string, string>();

            words.Add("Baum", "tree");
            words.Add("Holz", "wood");
            words.Add("Wurzel", "root");

            foreach(string translation in words.Keys)
            {
                System.Console.WriteLine(translation + ":\t" + words[translation]);
            }
        }
    }
}

```

- Aufgabe Bauernhof
```C#
namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            Schaf schaf = new Schaf();
            Esel esel = new Esel();
            Console.WriteLine("* Willkommen im Bauernhof *");
            for(int i = 0; i < 3; i++)
            {
                schaf.Scheren();
            }
            Console.WriteLine("Aktueller Bestand Wolle: " + Lager.bestandWolle);
            Console.WriteLine("Was soll geliefert werden?");
            string eingabeText = Console.ReadLine();
            Console.WriteLine("Wieviel soll geliefert werden?");
            int eingabeZahl = int.Parse(Console.ReadLine());
            esel.Liefern(eingabeText, eingabeZahl);
            Console.WriteLine("Aktueller Bestand Wolle: " + Lager.bestandWolle);
            Console.WriteLine("Auf Wiedersehen...");

            Console.ReadKey();
        }
    }
    class Lager
    {
        public static int bestandWolle;
        public static int bestandEier;
        public static int bestandKuhmilch;
        public static int bestandZiegenmilch;
    }

    abstract class Milchtiere
    {
        public abstract void Melken();
    }

    abstract class Lasttiere : Lager
    {
        public void Liefern(string ware, int menge)
        {
            if (ware == "Wolle")
            {
                if (Lager.bestandWolle > menge)
                {
                    Lager.bestandWolle -= menge;
                }
                else
                {
                    AusgabeBestandUnzureichend(ware, Lager.bestandWolle);
                }
            }
            else if (ware == "Eier")
            {
                if (Lager.bestandEier > menge)
                {
                    Lager.bestandEier -= menge;
                }
                else
                {
                    AusgabeBestandUnzureichend(ware, Lager.bestandEier);
                }
            }
            else if (ware == "Kuhmilch")
            {
                if (Lager.bestandKuhmilch > menge)
                {
                    Lager.bestandKuhmilch -= menge;
                }
                else
                {
                    AusgabeBestandUnzureichend(ware, Lager.bestandKuhmilch);
                }
            }
            else if (ware == "Ziegenmilch")
            {
                if (Lager.bestandZiegenmilch > menge)
                {
                    Lager.bestandZiegenmilch -= menge;
                }
                else
                {
                    AusgabeBestandUnzureichend(ware, Lager.bestandZiegenmilch);
                }
            }
            else
            {
                Console.WriteLine("Angegebene Ware existiert nicht.");
            }
        }

        private void AusgabeBestandUnzureichend(string ware, int bestand)
        {
            Console.WriteLine("Bestand an: " + ware + " nicht aussreichend");
            Console.WriteLine("Bestand an: " + ware + " = " + bestand);
        }
    }

    class Schaf
    {
        public void Scheren()
        {
            Lager.bestandWolle++;
        }
    }

    class Huhn
    {
        public void EierLegen()
        {
            Lager.bestandEier++;
        }
    }

    class Kuh : Milchtiere
    {
        public override void Melken()
        {
            Lager.bestandKuhmilch++;
        }
    }
    class Ziege : Milchtiere
    {
        public override void Melken()
        {
            Lager.bestandZiegenmilch++;
        }
    }
    class Esel : Lasttiere { }
    class Ochse : Lasttiere { }
}
```

