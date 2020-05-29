using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static EncodeousCommon.Sys.Windows.User32;

namespace EncodeousCommon.Sys.Windows
{
    public class Desktop
    {
        #region Imports
        [DllImport("kernel32.dll")]
        private static extern int GetThreadId(IntPtr thread);

        [DllImport("kernel32.dll")]
        private static extern int GetProcessId(IntPtr process);

        //
        // Imported winAPI functions.
        //
        [DllImport("user32.dll")]
        private static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice, IntPtr pDevmode, int dwFlags, long dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll")]
        private static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenDesktop(string lpszDesktop, int dwFlags, bool fInherit, long dwDesiredAccess);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenInputDesktop(int dwFlags, bool fInherit, long dwDesiredAccess);

        [DllImport("user32.dll")]
        private static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll")]
        private static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll")]
        private static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, IntPtr pvInfo, int nLength, ref int lpnLengthNeeded);
        private delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
        private delegate bool EnumDesktopWindowsProc(IntPtr desktopHandle, IntPtr lParam);
        #endregion

        #region Enums
        public enum DESKTOP_ACCESS : uint
        {
            DESKTOP_NONE = 0,
            DESKTOP_READOBJECTS = 0x0001,
            DESKTOP_CREATEWINDOW = 0x0002,
            DESKTOP_CREATEMENU = 0x0004,
            DESKTOP_HOOKCONTROL = 0x0008,
            DESKTOP_JOURNALRECORD = 0x0010,
            DESKTOP_JOURNALPLAYBACK = 0x0020,
            DESKTOP_ENUMERATE = 0x0040,
            DESKTOP_WRITEOBJECTS = 0x0080,
            DESKTOP_SWITCHDESKTOP = 0x0100,

            GENERIC_ALL = (DESKTOP_READOBJECTS | DESKTOP_CREATEWINDOW | DESKTOP_CREATEMENU |
                           DESKTOP_HOOKCONTROL | DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK |
                           DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP),
        }


        #endregion

        #region Static Methods
        public static IntPtr OpenInputDesktop()
        {
            return  OpenInputDesktop(0, true, (uint)ACCESS_MASK.GENERIC_ALL);
        }
        private static bool DesktopProc(string lpszDesktop, IntPtr lParam)
        {
            // add the desktop to the collection.
            desktops.Add(lpszDesktop);

            return true;
        }
        public static bool Exists(string name)
        {
            // enumerate desktops.
            string[] desktops = Desktop.GetDesktops();

            // return true if desktop exists.
            foreach (string desktop in desktops)
            {
                if (desktop == name) return true;
            }

            return false;
        }
        public static string[] GetDesktops()
        {
            // attempt to enum desktops.
            IntPtr windowStation = GetProcessWindowStation();

            // check we got a valid handle.
            if (windowStation == IntPtr.Zero) return new string[0];

            string[] desktop;

            // lock the object. thread safety and all.
            lock (desktops = new StringCollection())
            {
                bool result = EnumDesktops(windowStation, new EnumDesktopProc(DesktopProc), IntPtr.Zero);

                // something went wrong.
                if (!result) return new string[0];

                //	// turn the collection into an array.
                desktop = new string[desktops.Count];
                for (int i = 0; i < desktop.Length; i++) desktops[i] = desktops[i];
            }

            return desktop;
        }
        public static IntPtr DesktopHandleOfCurrentThread()
        {
            return GetThreadDesktop((int)ProcessManager.GetCurrentThreadId());
        }
        public static IntPtr DesktopHandleOfThread(int threadID)
        {
            return GetThreadDesktop(threadID);
        }

        public static void SetCurrentThreadDesktop(Desktop d)
        {
            SetThreadDesktop(d.Handle);
        }
        public static void SetCurrentThreadDesktop(IntPtr d)
        {
            SetThreadDesktop(d);
        }
        public static IntPtr CreateDesktopH(string name, DESKTOP_ACCESS desktopAccess)
        {
            return CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, (uint)desktopAccess, IntPtr.Zero);
        }
        public static Desktop DesktopOfCurrentThread()
        {
            return new Desktop(GetThreadDesktop((int)ProcessManager.GetCurrentThreadId()));
        }
        public static Desktop DesktopOfThread(int threadID)
        {
            return new Desktop(GetThreadDesktop(threadID));
            
        }

        public static Desktop CreateDesktop(string name, DESKTOP_ACCESS desktopAccess)
        {
            return new Desktop(CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, (uint)desktopAccess, IntPtr.Zero));
        }

        public static IntPtr OpenDesktopHandle(string name)
        {
            return OpenDesktop(name, 0, true, AccessRights);
        }

        public static Desktop OpenDesktop(string name)
        {
            return new Desktop(OpenDesktop(name, 0, true, AccessRights));
        }
        public static string GetDesktopName(IntPtr desktopHandle)
        {
            // check its not a null pointer.
            // null pointers wont work.
            if (desktopHandle == IntPtr.Zero) return null;

            // get the length of the name.
            int needed = 0;
            string name = String.Empty;
            GetUserObjectInformation(desktopHandle, 2, IntPtr.Zero, 0, ref needed);

            // get the name.
            IntPtr ptr = Marshal.AllocHGlobal(needed);
            bool result = GetUserObjectInformation(desktopHandle, 2, ptr, needed, ref needed);
            name = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            // something went wrong.
            if (!result) return null;

            return name;
        }


        public static bool SwitchToInputDesktop()
        {
            //return true;
            var inputDesktop = OpenInputDesktop();

            try
            {
                if (inputDesktop == IntPtr.Zero)
                {
                    return false;
                }

                if (!SetThreadDesktop(inputDesktop) || !SwitchDesktop(inputDesktop))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseDesktop(inputDesktop);
            }
        }

        #endregion

        public IntPtr Handle;
        public string DesktopName;
        private static StringCollection desktops;
        private const short SW_HIDE = 0;
        private const short SW_NORMAL = 1;
        private const int STARTF_USESTDHANDLES = 0x00000100;
        private const int STARTF_USESHOWWINDOW = 0x00000001;
        private const int UOI_NAME = 2;
        private const int STARTF_USEPOSITION = 0x00000004;
        private const int NORMAL_PRIORITY_CLASS = 0x00000020;
        private const long DESKTOP_CREATEWINDOW = 0x0002L;
        private const long DESKTOP_ENUMERATE = 0x0040L;
        private const long DESKTOP_WRITEOBJECTS = 0x0080L;
        private const long DESKTOP_SWITCHDESKTOP = 0x0100L;
        private const long DESKTOP_CREATEMENU = 0x0004L;
        private const long DESKTOP_HOOKCONTROL = 0x0008L;
        private const long DESKTOP_READOBJECTS = 0x0001L;
        private const long DESKTOP_JOURNALRECORD = 0x0010L;
        private const long DESKTOP_JOURNALPLAYBACK = 0x0020L;
        private const long AccessRights = DESKTOP_JOURNALRECORD | DESKTOP_JOURNALPLAYBACK | DESKTOP_CREATEWINDOW | DESKTOP_ENUMERATE | DESKTOP_WRITEOBJECTS | DESKTOP_SWITCHDESKTOP | DESKTOP_CREATEMENU | DESKTOP_HOOKCONTROL | DESKTOP_READOBJECTS;

        public Desktop(IntPtr handle)
        {
            Handle = handle;
            DesktopName = GetDesktopName(handle);
        }

        public Desktop(string name)
        {
            DesktopName = name;
            Handle = OpenDesktop(name, 0, true, AccessRights);
        }
        public bool Close()
        {
            // check there is a desktop open.
            if (Handle != IntPtr.Zero)
            {
                // close the desktop.
                bool result = CloseDesktop(Handle);

                if (result)
                {
                    Handle = IntPtr.Zero;

                    DesktopName = String.Empty;
                }

                return result;
            }

            // no desktop was open, so desktop is closed.
            return true;
        }
        public bool Show()
        {
            // make sure there is a desktop to open.
            if (Handle == IntPtr.Zero) return false;

            // attempt to switch desktops.
            bool result = SwitchDesktop(Handle);

            return result;
        }
    }
}
