using System;
using System.Collections.Concurrent; // Für ConcurrentBag
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography; // Für RNGCryptoServiceProvider
using System.Text;
using System.Threading.Tasks;

namespace TplTest {
    class Program {
        /* Die results Collection speichert die Anzahl der Würfe, die bis zum Eintreffen eines
         * Sechsers benötigt wurden.
         * Für gleichzeitigen schreibenden Zugriff verwenden wir den 
         * ConcurrentBag. Eine Liste liefert würde eine gewisse (kleine) Anzahl von
         * Experimenten mit 0 Würfen liefern, die aber nicht
         * vorkommen dürfen! Das entsteht, weil wir ohne Sperrung
         * results.Add () aufrufen. Dann überschreiben sich Daten.
         */
        static ConcurrentBag<int> results = new ConcurrentBag<int> ();

        /* Kryptografisch sichere Zufallszahlen. */
        static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider ();

        static void Main (string[] args) {
            /* SCHRITT 1: SINGLE THREAD */
            Console.WriteLine ("Klassisch hintereinander abarbeiten.");
            for (int count = 1; count <= 1000; count++) {
                WaitFor6 ();
            }
            PrintStat ();

            /* *************************************************************************************
             * SCHRITT 2 : TASKS manuell synchronisiert
             * *************************************************************************************
             */
            List<Task> taskList = new List<Task> (); // Zur Verwaltung der Tasks            
            Console.WriteLine ("Nun arbeiten wir mit Tasks, manuell aufgerufen.");
            for (int count = 1; count <= 1000000; count++) {
                Task t = new Task (WaitFor6);
                taskList.Add (t);
                t.Start ();
            }
            /* Hier wird gewartet, bis ALLE Tasks beendet sind. */
            Task.WaitAll (taskList.ToArray ());
            PrintStat ();

            /* *************************************************************************************
             * SCHRITT 3 : Noch komfortabler mit der Parallel Klasse. Hier werden auch nicht 
             * alle Tasks gleichzeitig gestartet, sondern nur so viele, wie es optimal ist.
             * *************************************************************************************
             */
            Console.WriteLine ("Mit der Parallel Klasse geht es am Besten.");
            /* Da For als 3. Argument Action<int> erwartet, kapseln wir unsere
             * Funktion in eine Lambda Expresion, die einen int bekommt. Der wird
             * aber nie verwendet. 
             */
            Parallel.For (1, 1000000, (count) => { WaitFor6 (); });

            PrintStat ();
            Console.ReadKey ();
        }

        /// <summary>
        /// Gibt die Statistik aus, wie oft jede Anzahl vorkommt (das Histogramm).
        /// </summary>
        static void PrintStat () {
            /* Hier wird die Liste der Anzahl für 0, 1, 2, ... Würfe generiert */
            int maxWuerfe = results.Max ();
            for (int wuerfe = 0; wuerfe <= maxWuerfe; wuerfe++) {
                int count = results.Where (r => r == wuerfe).Count ();
                Console.WriteLine ("{0} kommt {1}x vor", wuerfe, count);
            }
        }
        /// <summary>
        /// Simuliert das Mensch ärgere dich nicht Spiel: Warten auf den ersten Sechser.
        /// </summary>
        static void WaitFor6 () {
            /* Der Random Generator befüllt immer ein byte Array mit Zahlen.  */
            byte[] randomNumber = new byte[1];
            for (int i = 1;; i++) {
                rngCsp.GetBytes (randomNumber);
                /* Die Wahrscheinlichkeit für einen Sechser ist 1/6.
                 * Also nehmen wir das unterste Sechstel des Wertebereichs als 6er. */
                if (randomNumber[0] < 256 / 6) {
                    results.Add (i);
                    return;
                }
            }
        }
    }
}
