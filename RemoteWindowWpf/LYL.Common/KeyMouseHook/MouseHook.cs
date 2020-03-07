using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MouseKeyPlayback
{
    /// <summary>
    /// A low level system wide mouse hook which can be used to listen to all mouse events across the application.
    /// </summary>
    public class MouseHook : BaseHook
    {
        public delegate bool OnMouseMoveEventHandler(int x, int y);

        /// <summary>
        /// Called everytime the mouse moved.
        /// </summary>
        public event OnMouseMoveEventHandler OnMouseMove;

        public delegate bool OnMouseEventHandler(int mouseEvent);

        /// <summary>
        /// When this event is being called, a mouseEvent will be delivered containing data about the mouse event occured.
        /// They key parameter will return one the MouseKeys enum (if exists), if not it will contain the same mouseEvent data.
        /// If the key exists in the MouseKeys enum, the key state will also contain the correct state of the key.
        /// </summary>
        public event OnMouseEventHandler OnMouseEvent;

        public delegate bool OnMouseWheelEventHandler(int wheelValue);

        /// <summary>
        /// When a mouse wheel event occurs this event will be called.
        /// You can detect if it's a normal mouse scroll using the MouseEventEvents enum.
        /// </summary>
        public event OnMouseWheelEventHandler OnMouseWheelEvent;

        public MouseHook() : base(HookType.MouseHook)
        {

        }

        #region Constants
        public enum MouseKeys
        {
            Left = MouseEvents.LeftDown,
            Middle = MouseEvents.MiddleDown,
            Right = MouseEvents.RightDown
        }

		public enum MouseActions
		{
			Click,
			DoubleClick,
			Up,
			Down
		}

        public enum MouseEvents
        {
            LeftDown = 0x201,
            LeftUp = 0x202,
            LeftDoubleClick = 0x203,
            RightDown = 0x204,
            RightUp = 0x205,
            RightDoubleClick = 0x206,
            MiddleDown = 0x207,
            MiddleUp = 0x208,
            MiddleDoubleClick = 0x209,
            MouseScroll = 0x20a,
            ScrollUp = 7864320,
            ScrollDown = -7864320,
            MouseMove = -1
        }

        public enum MouseWheelEvents
        {
            ScrollUp = 7864320,
            ScrollDown = -7864320
        }
        #endregion
        #region Structures
        public struct NativePoint
        {
            public int x;
            public int y;
        }

        public struct MouseHookStruct
        {
            public NativePoint pt;
            public int mouseData;
            private int flags;
            private int time;
            private int dwExtraInfo;
        }
        #endregion


        protected override IntPtr OnHookCall(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseHookStruct mouseStruct = (MouseHookStruct) Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            IntPtr returnCall = OnMouseHookCall(nCode, wParam, mouseStruct);
            if ((int)returnCall != 0) return returnCall;

            return base.OnHookCall(nCode, wParam, lParam);
        }

        /// <summary>
        ///  Called when a mouse hook call had occured, use this instead of OnHookCall to read the mouse structure.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="mouseStruct"></param>
        /// <returns></returns>
        protected virtual IntPtr OnMouseHookCall(int nCode, IntPtr wParam, MouseHookStruct mouseStruct)
        {
            var consumeEvent = false;

            if ((nCode == LibConstants.HC_ACTION))
            {
                var wParamInt = (int)wParam;

                if (wParamInt == LibConstants.WM_MOUSEMOVE)
                {
                    // If the mouse is moving
                    if (OnMouseMove != null)
                        consumeEvent = OnMouseMove(mouseStruct.pt.x, mouseStruct.pt.y);

                }
                else if (wParamInt == LibConstants.WM_MOUSEWHEEL)
                {
                    //If the wheel moved
                    if (OnMouseWheelEvent != null)
                        consumeEvent = OnMouseWheelEvent(mouseStruct.mouseData);
                }
                else
                {
                    // If a mouse event occured
                    if (OnMouseEvent != null)
                        consumeEvent = OnMouseEvent(wParamInt);
                }
            }

            if (consumeEvent) return (IntPtr)LibConstants.CONSUME_INPUT;
            return IntPtr.Zero;
        }
    }
}
