using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserMoudle.RemoteWindow.remoteControlLogic
{
    public enum SendFileType
    {
        StartSendFile = 1,
        SendFileFrag = 2,
        EndSendFile = 3,
        SendFileError = 4,
        ReciveFileFragSuccess = 5
    }
    public class SendFileInfo<T>
    {
        public SendFileType SendFileType { get; set; }
        public T Data { get; set; }
    }
}
