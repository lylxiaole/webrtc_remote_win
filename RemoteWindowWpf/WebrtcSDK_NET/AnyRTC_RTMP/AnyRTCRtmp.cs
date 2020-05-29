using Srs_Librtmp;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebrtcSDK_NET.AnyRTC_RTMP
{
    public class AnyRTCRtmp:IDisposable
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rtmpServerUr"></param>
        /// <param name="whwd">>win 图片控件句柄</param>
        public void StartPushRtmp(string rtmpServerUr, IntPtr whwd)
        { 
            AnyRTCRtmp_Exp.StartPushRtmp(rtmpServerUr,whwd);
        }

        public void Dispose()
        {
            AnyRTCRtmp_Exp.Dispose();
        }

    }
}
