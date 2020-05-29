using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Srs_Librtmp
{ 
    public class AnyRTCRtmp_Exp
    { 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rtmpServerUrl"></param>
        /// <param name="whwd">win 图片控件句柄</param>
        [DllImport("AnyCore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void StartPushRtmp([MarshalAs(UnmanagedType.LPStr)]string rtmpServerUrl,IntPtr whwd);

        [DllImport("AnyCore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Dispose(); 
    }
}
