/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using System;
using System.Threading;

namespace StormKitty.Implant
{
    internal sealed class MutexControl
    {
        // Prevent the program from running twice
        public static void Check()
        {
            bool createdNew = false;
            Mutex currentApp = new Mutex(false, Config.Mutex, out createdNew);
            if (!createdNew)
                Environment.Exit(1);
        }
    }
}
