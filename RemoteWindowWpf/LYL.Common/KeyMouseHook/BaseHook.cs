using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MouseKeyPlayback
{
    /// <summary>
    /// A low level system wide hook class allows making keyboard and mouse hooks easier.
    /// </summary>
    public abstract class BaseHook
    {
        private IntPtr hookHandle;
        private HookType hookType;
        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private HookProc hookDelegate;

        #region WinAPI
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetKeyState")]
        public static extern int GetKeyState(int nVirtKey);



        #endregion
            #region Constants

        public enum HookType
        {
            KeyboardHook = LibConstants.WH_KEYBOARD_LL,
            MouseHook = LibConstants.WH_MOUSE_LL
        }

        public enum KeyState
        {
            Keydown = 0,
            Keyup = 1
        }
        #endregion

        public BaseHook(HookType hookType)
        {
            this.hookType = hookType;
            this.hookDelegate = OnHookCall;
        }

        /// <summary>
        /// Installs the system wide global hook.
        /// </summary>
        public void Install()
        {
            if (Environment.Version.Major >= 4)
            {
                var hInstance = LoadLibrary("user32.dll");
                hookHandle = SetWindowsHookEx((int)hookType, hookDelegate, hInstance, 0);
            }
            else
            {
                hookHandle = SetWindowsHookEx((int)hookType, hookDelegate, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            }

            if (hookHandle == IntPtr.Zero)
                throw new Exception("Failed to setup hook, last win32 api error: " + Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Uninstalls the system wide global hook.
        /// </summary>
        public void Uninstall()
        {
            UnhookWindowsHookEx(hookHandle);
        }

        /// <summary>
        /// This method is called each time a hook call happened by the system.
        /// If you want to consume the key input so other applications will not capture it you must
        /// return CONSUME_KEY_INPUT.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        protected virtual IntPtr OnHookCall(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(hookHandle, nCode, (int)wParam, lParam);
        }
    }
}
