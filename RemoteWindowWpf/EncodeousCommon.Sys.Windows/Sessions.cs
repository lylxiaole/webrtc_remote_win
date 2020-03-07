using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace EncodeousCommon.Sys.Windows
{
    public class Sessions
    {
        /*
         * The code for bypassing windows' session isolation was taken from the article:
         * Subverting Vista UAC in Both 32 and 64 bit Architectures By: Pero Matić
         * www.codeproject.com/Articles/35773/Subverting-Vista-UAC-in-Both-32-and-64-bit-Archite
         */
        public static int GetActiveSessionID()
        {
            return (int)WTSGetActiveConsoleSessionId();
        }

        /// <summary>
        /// Starts the given application in the current active session
        /// </summary>
        /// <param name="applicationPath">The path to the application</param>
        /// <param name="procInfo">process information</param>
        /// <returns></returns>
        public static bool StartProcessInActiveSession(string applicationPath, out ProcessManager.PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new ProcessManager.PROCESS_INFORMATION();

            // obtain the currently active session id; every logged on user in the system has a unique session id
            var dwSessionId = WTSGetActiveConsoleSessionId();

            // obtain the process id of the winlogon process that is running within the currently active session
            var processes = Process.GetProcessesByName("winlogon");
            foreach (var p in processes)
                if ((uint) p.SessionId == dwSessionId)
                    winlogonPid = (uint) p.Id;

            // obtain a handle to the winlogon process
            hProcess = ProcessManager.OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!ProcessManager.OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }
            var sa = new ProcessManager.SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);
            if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa,
                (int) SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int) TOKEN_TYPE.TokenPrimary,
                ref hUserTokenDup))
            {
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                return false;
            }
            var si = new ProcessManager.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop =  @"Winsta0\Winlogon";
            si.lpDesktop =  @"Winsta0\Default";
            var dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE ;
            var result = CreateProcessAsUser(hUserTokenDup, // client's access token
                null, // file to execute
                applicationPath, // command line
                ref sa, // pointer to process SECURITY_ATTRIBUTES
                ref sa, // pointer to thread SECURITY_ATTRIBUTES
                false, // handles are not inheritable
                dwCreationFlags, // creation flags
                IntPtr.Zero, // pointer to new environment block 
                null, // name of current directory 
                ref si, // pointer to STARTUPINFO structure
                out procInfo // receives information about new process
            );

            // invalidate the handles
            CloseHandle(hProcess);
            CloseHandle(hPToken);
            CloseHandle(hUserTokenDup);

            return result; // return the result
        }
        /// <summary>
        /// Starts the given application in the given session
        /// </summary>
        /// <param name="applicationPath">The path to the application</param>
        /// <param name="session">Windows Session ID</param>
        /// <param name="procInfo">process information</param>
        /// <returns></returns>
        public static bool StartProcessInSession(string applicationPath, int session, out ProcessManager.PROCESS_INFORMATION procInfo)
        {
            uint winlogonPid = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            procInfo = new ProcessManager.PROCESS_INFORMATION();

            // obtain the currently active session id; every logged on user in the system has a unique session id

            // obtain the process id of the winlogon process that is running within the currently active session
            var processes = Process.GetProcessesByName("winlogon");
            foreach (var p in processes)
                if ((uint) p.SessionId == session)
                    winlogonPid = (uint) p.Id;

            // obtain a handle to the winlogon process
            hProcess = ProcessManager.OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            // obtain a handle to the access token of the winlogon process
            if (!ProcessManager.OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }

            var sa = new ProcessManager.SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            // copy the access token of the winlogon process; the newly created token will be a primary token
            if (!DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa,
                (int) SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int) TOKEN_TYPE.TokenPrimary,
                ref hUserTokenDup))
            {
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                return false;
            }

            var si = new ProcessManager.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop =
                @"winsta0\default"; 
            var dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

            // create a new process in the current user's logon session
            var result = CreateProcessAsUser(hUserTokenDup, // client's access token
                null, // file to execute
                applicationPath, // command line
                ref sa, // pointer to process SECURITY_ATTRIBUTES
                ref sa, // pointer to thread SECURITY_ATTRIBUTES
                false, // handles are not inheritable
                dwCreationFlags, // creation flags
                IntPtr.Zero, // pointer to new environment block 
                null, // name of current directory 
                ref si, // pointer to STARTUPINFO structure
                out procInfo // receives information about new process
            );

            // invalidate the handles
            CloseHandle(hProcess);
            CloseHandle(hPToken);
            CloseHandle(hUserTokenDup);

            return result; // return the result
        }



        #region Enumerations

        private enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        private enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3
        }

        #endregion

        #region Constants

        public const int TOKEN_DUPLICATE = 0x0002;
        public const int MAXIMUM_ALLOWED = 0x2000000;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int CREATE_NO_WINDOW = 0x08000000;
        public const int IDLE_PRIORITY_CLASS = 0x40;
        public const int NORMAL_PRIORITY_CLASS = 0x20;
        public const int HIGH_PRIORITY_CLASS = 0x80;
        public const int REALTIME_PRIORITY_CLASS = 0x100;

        #endregion

        #region Win32 API Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine,
            ref ProcessManager.SECURITY_ATTRIBUTES lpProcessAttributes,
            ref ProcessManager.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            string lpCurrentDirectory, ref ProcessManager.STARTUPINFO lpStartupInfo, out ProcessManager.PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        private static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public static extern bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref ProcessManager.SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);



        #endregion
    }
}