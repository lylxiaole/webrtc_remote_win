using LYL.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebrtcSDK_NET.WebRtc;

namespace UserMoudle.RemoteWindow.remoteControlLogic
{
    public class FileChannelMessageDeal
    {
        private static string localDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "cacheFile\\";
        private static Dictionary<DataChannel, List<FileFragHelper_Read>> caches = new Dictionary<DataChannel, List<FileFragHelper_Read>>();
        public static void RegisterFileChannel(DataChannel channel)
        {
            if (caches.ContainsKey(channel) == false)
            {
                caches.Add(channel, new List<FileFragHelper_Read>());
                channel.onChannelMessage += Channel_onChannelMessage;
                channel.onChannelStateChanged += Channel_onChannelStateChanged;
            }
        }

        private static void Channel_onChannelStateChanged(object sender, RTCDataChannelState e)
        {
            if (e == RTCDataChannelState.RTCDataChannelClosed)
            {
                caches_recive_files.Clear();
                removeFileChannel(sender as DataChannel);
            }
        }

        public static void removeFileChannel(DataChannel channel)
        {
            if (channel == null)
            {
                return;
            }
            if (caches.ContainsKey(channel) == true)
            {
                foreach (var reader in caches.FirstOrDefault(o => o.Key == channel).Value)
                {
                    reader.onReadFrag -= Read_onReadFrag;
                    reader.onEndReadFrag -= Read_onEndReadFrag;
                    reader.onReadFragError -= Read_onReadFragError;
                    reader.Dispose();
                }
                caches.Remove(channel);
            }
        }

        private static void Channel_onChannelMessage(object sender, ChannelMessage e)
        {
            var gettype = JsonConvert.DeserializeObject<SendFileInfo<object>>(e.buffer);
            if (gettype.SendFileType == SendFileType.StartSendFile)
            {
                var message = JsonConvert.DeserializeObject<SendFileInfo<FileFragInfo>>(e.buffer);
                CreateFile(message.Data, sender as DataChannel);
            }
            else if (gettype.SendFileType == SendFileType.EndSendFile)
            {
                var message = JsonConvert.DeserializeObject<SendFileInfo<FileFragInfo>>(e.buffer);
                EndWriteFile(message.Data, sender as DataChannel);
            }
            else if (gettype.SendFileType == SendFileType.SendFileFrag)
            {
                var message = JsonConvert.DeserializeObject<SendFileInfo<FileFrag>>(e.buffer);
                WriteBytesToFile(message.Data, sender as DataChannel);
            }
            else if (gettype.SendFileType == SendFileType.SendFileError)
            {
                var message = JsonConvert.DeserializeObject<SendFileInfo<FileFragInfo>>(e.buffer);
                ReciveFileError(message.Data, sender as DataChannel);
            }
            else if (gettype.SendFileType == SendFileType.ReciveFileFragSuccess)
            {
                var info = JsonConvert.DeserializeObject<SendFileInfo<string>>(e.buffer);
                var find = findFileReaderByGuid(info.Data);
                if (find != null)
                {
                    find.ContinueRead();
                }
            }
        }

        private static FileFragHelper_Read findFileReaderByGuid(string guid)
        {
            foreach (var keyvaluepar in caches)
            {
                var find = keyvaluepar.Value.FirstOrDefault(o => o.CurrentFileInfo?.fileGuid == guid);
                if (find != null)
                {
                    return find;
                }
            }
            return null;
        }
        private static DataChannel findDataChannelByGuid(string guid)
        {
            foreach (var keyvaluepar in caches)
            {
                var find = keyvaluepar.Value.FirstOrDefault(o => o.CurrentFileInfo?.fileGuid == guid);
                if (find != null)
                {
                    return keyvaluepar.Key;
                }
            }
            return null;
        }

        private static void removeFileReaderByGuid(DataChannel channel, string guid)
        {
            var find = findFileReaderByGuid(guid);
            if (find != null)
            {
                find.onReadFrag -= Read_onReadFrag;
                find.onEndReadFrag -= Read_onEndReadFrag;
                find.onReadFragError -= Read_onReadFragError;
                find.Dispose();
                caches.FirstOrDefault(o => o.Key == channel).Value.Remove(find);
            }
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
        }
        private static void addFileReader(DataChannel channel, FileFragHelper_Read reader)
        {
            var find = findFileReaderByGuid(reader.CurrentFileInfo.fileGuid);
            if (find == null)
            {
                caches.FirstOrDefault(o => o.Key == channel).Value.Add(reader);
            }
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
        }

        private static int cuteFileCount(DataChannel channel)
        {
            var sendingCount = 0;
            if (caches.ContainsKey(channel) == true)
            {
                sendingCount = caches.FirstOrDefault(o => o.Key == channel).Value.Count;
            }
            var recivingCount = caches_recive_files.Count;
            return sendingCount + recivingCount;
        }

        public static List<FileFragInfo> GetAllSendingFileInfo()
        {
            var fileinfos = new List<FileFragInfo>();
            foreach (var keypair in caches)
            {
                fileinfos.AddRange(keypair.Value.Select(o => o.CurrentFileInfo));
            }
            return fileinfos;
        }
        public static List<FileFragInfo> GetAllRecivingingFileInfo()
        {
            return caches_recive_files;
        }


        public static void OpenFileDiction()
        {
            if (Directory.Exists(localDirectory) == false)
            {
                Directory.CreateDirectory(localDirectory);
            }
            System.Diagnostics.Process.Start("explorer.exe", localDirectory);

        }

        #region 发送文件 
        public static void SelectFileToSend(DataChannel channel)
        {
            FileFragHelper_Read read = new FileFragHelper_Read();
            read.onReadFrag += Read_onReadFrag;
            read.onEndReadFrag += Read_onEndReadFrag;
            read.onReadFragError += Read_onReadFragError;
            var res = read.SelectFileToFragSend();
            if (res == false)
            {
                read.onReadFrag -= Read_onReadFrag;
                read.onEndReadFrag -= Read_onEndReadFrag;
                read.onReadFragError -= Read_onReadFragError;
                return;
            }
            else
            {
                addFileReader(channel, read);
                FileChannelMessageDeal.startSendFile(channel, read);
                read.StartRead();
            }
        }

        private static void Read_onReadFragError(object sender, FileFragHelper_Read e)
        {
            var channel = findDataChannelByGuid(e.CurrentFileInfo.fileGuid);
            if (channel == null)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                FileChannelMessageDeal.readFragError(channel, e);
            }));
        }

        private static void Read_onEndReadFrag(object sender, FileFragHelper_Read e)
        {
            var channel = findDataChannelByGuid(e.CurrentFileInfo.fileGuid);
            if (channel == null)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                FileChannelMessageDeal.endSendFile(channel, e);
            }));
        }

        private static void Read_onReadFrag(object sender, FileFragHelper_Read e)
        {
            var channel = findDataChannelByGuid(e.CurrentFileInfo.fileGuid);
            if (channel == null)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                FileChannelMessageDeal.sendFileFrag(channel, e);
            }));
        }

        public static void startSendFile(DataChannel channel, FileFragHelper_Read filereader)
        {
            var find = findFileReaderByGuid(filereader.CurrentFileInfo.fileGuid);
            if (find == null)
            {
                caches.FirstOrDefault(o => o.Key == channel).Value.Add(filereader);
            }
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
            SendFileInfo<FileFragInfo> data = new SendFileInfo<FileFragInfo>
            {
                Data = filereader.CurrentFileInfo,
                SendFileType = SendFileType.StartSendFile
            };
            channel.SendChannelData(JsonConvert.SerializeObject(data));

        }
        public static void sendFileFrag(DataChannel channel, FileFragHelper_Read filereader)
        {
            var find = findFileReaderByGuid(filereader.CurrentFileInfo.fileGuid);
            if (find != null)
            {
                SendFileInfo<FileFrag> data = new SendFileInfo<FileFrag>
                {
                    Data = filereader.CurrentFrag,
                    SendFileType = SendFileType.SendFileFrag
                };
                find.CurrentFileInfo.currentFragIndex = filereader.CurrentFrag.currentFragIndex;
                onFragChanged?.Invoke(find.CurrentFileInfo, find.CurrentFileInfo);
                channel.SendChannelData(JsonConvert.SerializeObject(data));
            }
        }
        public static void endSendFile(DataChannel channel, FileFragHelper_Read filereader)
        {
            removeFileReaderByGuid(channel, filereader.CurrentFileInfo.fileGuid);
            SendFileInfo<FileFragInfo> data = new SendFileInfo<FileFragInfo>
            {
                Data = filereader.CurrentFileInfo,
                SendFileType = SendFileType.EndSendFile
            };
            channel.SendChannelData(JsonConvert.SerializeObject(data));
        }
        public static void readFragError(DataChannel channel, FileFragHelper_Read filereader)
        {
            removeFileReaderByGuid(channel, filereader.CurrentFileInfo.fileGuid);
            SendFileInfo<FileFragInfo> data = new SendFileInfo<FileFragInfo>
            {
                Data = filereader.CurrentFileInfo,
                SendFileType = SendFileType.SendFileError
            };
            channel.SendChannelData(JsonConvert.SerializeObject(data));
        }
        #endregion

        #region 接收文件  
        private static List<FileFragInfo> caches_recive_files = new List<FileFragInfo>();
        private static void addRecieFileCache(FileFragInfo info)
        {
            if (caches_recive_files.Any(o => o.fileGuid == info.fileGuid) == false)
            {
                caches_recive_files.Add(info);
            }
        }
        private static void removeRecieFileCache(FileFragInfo info)
        {
            var find = caches_recive_files.FirstOrDefault(o => o.fileGuid == info.fileGuid);
            if (find != null)
            {
                caches_recive_files.Remove(find);
            }
        }

        private static void CreateFile(FileFragInfo info, DataChannel channel)
        {
            if (Directory.Exists(localDirectory) == false)
            {
                Directory.CreateDirectory(localDirectory);
            }
            var filepath = localDirectory + info.fileGuid;
            var stream = File.Create(filepath);
            stream.Close();
            stream.Dispose();
            //
            FileInfo localfile = new FileInfo(filepath);
            localfile.Attributes = FileAttributes.Hidden;
            //
            addRecieFileCache(info);
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
        }
        private static void WriteBytesToFile(FileFrag frag, DataChannel channel)
        {
            //
            SendFileInfo<string> data = new SendFileInfo<string>
            {
                Data = frag.fileGuid,
                SendFileType = SendFileType.ReciveFileFragSuccess
            };
            channel.SendChannelData(JsonConvert.SerializeObject(data));

            var fileInfo = caches_recive_files.FirstOrDefault(o => o.fileGuid == frag.fileGuid);
            fileInfo.currentFragIndex = frag.currentFragIndex;
            onFragChanged?.Invoke(fileInfo, fileInfo);
            //
            var filepath = localDirectory + frag.fileGuid;
            try
            {
                using (var filestream = File.OpenWrite(filepath))
                {
                    filestream.Seek(frag.currentFragIndex * Params.flagSize, SeekOrigin.Begin);
                    filestream.Write(frag.bytes, 0, frag.bytesLength);
                    filestream.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (Exception)
            {
            }
        }
        private static void EndWriteFile(FileFragInfo info, DataChannel channel)
        {
            var oldfilepath = localDirectory + info.fileGuid;
            var newfilepath = localDirectory + info.fileGuid + "__" + info.fileName;
            File.Move(oldfilepath, newfilepath);
            //
            FileInfo localfile = new FileInfo(newfilepath);
            localfile.Attributes = FileAttributes.Normal; 
            //
            removeRecieFileCache(info);
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
        }
        private static void ReciveFileError(FileFragInfo info, DataChannel channel)
        {
            var oldfilepath = localDirectory + info.fileGuid;
            File.Delete(oldfilepath);
            removeRecieFileCache(info);
            onFileCountChanged?.Invoke(channel, cuteFileCount(channel));
        }
        #endregion 
        public static event EventHandler<int> onFileCountChanged;
        public static event EventHandler<FileFragInfo> onFragChanged;
    }

}
