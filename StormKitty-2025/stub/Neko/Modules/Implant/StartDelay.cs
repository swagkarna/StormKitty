using System;

namespace StormKitty.Implant
{
    internal sealed class StartDelay
    {
        // Sleep min, sleep max
        private static readonly int SleepMin = 0;
        private static readonly int SleepMax = 10;

        // Sleep
        public static void Run()
        {
            int SleepTime = new Random().Next(
                SleepMin * 1000,
                SleepMax * 1000
                );
            System.Threading.Thread.Sleep(SleepTime);
        }
    }
}
