using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MouseKeyPlayback
{
    /// <summary>
    /// A low level system wide keyboard hook which can be used to listen to all mouse events across the application.
    /// </summary>
    public class KeyboardHook : BaseHook
    {
        public delegate bool OnKeyboardEventHandler(uint key, KeyState keyState);
        public event OnKeyboardEventHandler OnKeyboardEvent;

        public KeyboardHook() : base(HookType.KeyboardHook)
        {

        }

        #region Structures
        public struct KeyboardHookStruct
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }
        #endregion

        protected override IntPtr OnHookCall(int nCode, IntPtr wParam, IntPtr lParam)
        { 
            KeyboardHookStruct keyboardStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            var returnCall = OnKeyboardHookCall(nCode, wParam, keyboardStruct);
            if ((int)returnCall != 0) return returnCall;

            return base.OnHookCall(nCode, wParam, lParam);
        }

        /// <summary>
        ///  Called when a keyboard hook call had occured, use this instead of OnHookCall to read the keyboard structure.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="keyboardStruct"></param>
        /// <returns></returns>
        protected virtual IntPtr OnKeyboardHookCall(int nCode, IntPtr wParam, KeyboardHookStruct keyboardStruct)
        {
          
            // If events are being listened, call to the associated keyboard down or up and check if we should consume the event or not
            if (OnKeyboardEvent != null)
            {
                var consumeEvent = false;

                if ((int)wParam == LibConstants.WM_KEYDOWN || (int)wParam == LibConstants.WM_SYSKEYDOWN)
                    consumeEvent = OnKeyboardEvent(keyboardStruct.vkCode, KeyState.Keydown);
                else if ((int)wParam == LibConstants.WM_KEYUP || (int)wParam == LibConstants.WM_SYSKEYUP)
                    consumeEvent = OnKeyboardEvent(keyboardStruct.vkCode, KeyState.Keyup);

                if (consumeEvent) return (IntPtr)LibConstants.CONSUME_INPUT;
            }

            return IntPtr.Zero;
        }
    }
}
