using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseKeyPlayback
{
    public class KeyboardUtils
    {
        #region Library
        [DllImportAttribute("user32.dll")]
        public static extern uint MapVirtualKeyW(uint uCode, uint uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        #endregion

        #region Local Constants
        private const uint MAPVK_VK_TO_CHAR = 2;
        private const int KEYEVENTF_EXTENDEDKEY = 0; //Key down flag;
        private const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        private const int VK_LCONTROL = 0xA2; //Left Control key code
        #endregion

        public static char KeyToChar(Keys key)
        {
            char myChar = unchecked((char)MapVirtualKeyW((uint)key, MAPVK_VK_TO_CHAR)); // Ignore high word.  
            return myChar;
        }

        public static void PerformKeyEvent(Keys key, string action)
        {
            switch (action)
            {
                case Constants.KEY_DOWN:
                    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
                    break;
                case Constants.KEY_UP:
                    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
                    break;
            }
            Thread.Sleep(7);
        }
    }
}
