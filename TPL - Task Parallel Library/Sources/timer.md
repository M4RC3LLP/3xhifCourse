## Datei Program.cs
```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starte Timer...");
            MyTimer t = new MyTimer() { Interval = 1000 };

            /* Das wird aufgerufen, wenn das Intervall erreicht
             * wurde. */
            t.OnInterval += (sender, eventargs) =>
            {
                Console.WriteLine("Tick");
            };

            /* Der Timer wird in einem neuen Task gestartet.
             * Machen wir das nicht, haben wir zwar events,
             * aber keinen 2 Thread --> Blockiert! */
            Task task = new Task(
                () => t.Start(),
                /* Wir sagen dem Scheduler, dass der Task
                 * nicht aufhören wird. */
                TaskCreationOptions.LongRunning);
            /* Jetzt wird der Task gestartet. Der Timer
             * beginnt zu arbeiten. */
            task.Start();
            Console.WriteLine("Jetzt kommt die Arbeit.");
            /* Ca. 2 Sekunden wird gearbeitet. Im Hintergrund
             * wirft der Timer alle 2 Sekunden ein Event. */
            for (long i = 0; i < 1e8; i++)
            {
                if (i % 1000000 == 0)
                {
                    Console.WriteLine("100 000 erreicht.");
                }
            }
            /* Das müssen WIR ALS PROGRAMMIERER implementieren.
             * Es gibt kein Task.Stop(), um den Abbruch müssen
             * wir uns selbst kümmern. */
            t.Stop();
            /* Wenn das nicht wäre, dann warten wir nicht, bis
             * unser Task auch sauber beendet wurde. Beim 
             * Schreiben von Files fatal! */
            task.Wait();
            Console.WriteLine("Task beendet, alles OK.");
            Console.ReadLine();

        }
    }
}
```

## Datei MyTimer.cs
```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    /// <summary>
    /// Umsetzung eines Timers, der periodisch das Event
    /// OnInterval wirft.
    /// </summary>
    class MyTimer
    {
        public event Action<object, EventArgs> OnInterval;
        public int Interval { get; set; }
        private bool isRunning = false;

        public void Start()
        {
            isRunning = true;
            while(isRunning)
            {
                System.Threading.Thread.Sleep(Interval);
                /* Event aufrufen. */
                OnInterval?.Invoke(this, null);
            }
        }

        public void Stop()
        {
            /* Dies beeinflusst die Schleife. Es wird aber erst
             * nach Sleep ausgewerter! */
            isRunning = false;
        }
    }
}
```
