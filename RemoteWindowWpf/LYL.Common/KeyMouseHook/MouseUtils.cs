using System.Runtime.InteropServices;
using System.Threading;

namespace MouseKeyPlayback
{

    public class MouseUtils {

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void PerformMouseEvent(MouseHook.MouseEvents mEvent, CursorPoint location)
        {
            int x = (int)location.X;
            int y = (int)location.Y;
            SetCursorPos(x, y);

            switch (mEvent)
            {
                case MouseHook.MouseEvents.LeftDown:
                    mouse_event(Constants.MOUSEEVENT_LEFTDOWN, x, y, 0, 0);
                    break;
                case MouseHook.MouseEvents.LeftUp:
                    mouse_event(Constants.MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                    break;
                case MouseHook.MouseEvents.RightDown:
                    mouse_event(Constants.MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                    break;
                case MouseHook.MouseEvents.RightUp:
                    mouse_event(Constants.MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
                    break;
                case MouseHook.MouseEvents.ScrollDown:
                    mouse_event(Constants.MOUSEEVENTF_WHEEL, 0, 0, -120, 0);
                    break;
                case MouseHook.MouseEvents.ScrollUp:
                    mouse_event(Constants.MOUSEEVENTF_WHEEL, 0, 0, 120, 0);
                    break;
            }

            if (mEvent != MouseHook.MouseEvents.MouseMove)
                Thread.Sleep(30);
        }
    }
}
