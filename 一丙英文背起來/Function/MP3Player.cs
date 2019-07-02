using System.Runtime.InteropServices;
using System.Security;

namespace 一丙英文背起來
{
    public class MP3Player
    {
        /// <summary>
        /// MP3檔案路徑
        /// </summary>
        public string FilePath;

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            MciSendString("close all", "", 0, 0);
            MciSendString("open " + FilePath + " alias media", "", 0, 0);
            MciSendString("play media", "", 0, 0);
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            MciSendString("pause media", "", 0, 0);
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            MciSendString("close media", "", 0, 0);
        }

        /// <summary>
        /// API函数
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Unicode)]
            internal static extern int MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        }

        private int MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback)
        {
            return SafeNativeMethods.MciSendString(lpstrCommand, lpstrReturnString, uReturnLength, hwndCallback);
        }
    }
}