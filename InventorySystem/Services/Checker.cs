using System.Timers;

namespace InventorySystem.Services
{
    public class Checker
    {
        private static readonly Timer Timer = new Timer();

        public Checker(ElapsedEventHandler eh, int howOftenToCheck)
        {
            Timer.Elapsed += eh;
            Timer.Interval = howOftenToCheck;
            Timer.Start();
        }

    }
}