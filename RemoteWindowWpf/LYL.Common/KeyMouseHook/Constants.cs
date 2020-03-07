using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseKeyPlayback
{
    public static class Constants
    {
        public const string KEYBOARD = "Keyboard";
        public const string MOUSE = "Mouse";
		public const string WAIT = "WAIT";

		public const int MOUSEEVENT_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_WHEEL = 0x0800;

        public const string KEY_DOWN = "KEY_DOWN";
        public const string KEY_UP = "KEY_UP";
    }
}
