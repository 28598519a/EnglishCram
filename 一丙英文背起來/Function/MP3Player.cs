using System;
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
            MciSendString("close all", "", 0, new IntPtr());
            MciSendString("open " + FilePath + " alias media", "", 0, new IntPtr());
            MciSendString("play media", "", 0, new IntPtr());
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            MciSendString("pause media", "", 0, new IntPtr());
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            MciSendString("close media", "", 0, new IntPtr());
        }

        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Unicode)]
            internal static extern bool MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, IntPtr hwndCallback);
        }

        /// <summary>
        /// API函数
        /// </summary>
        private void MciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, IntPtr hwndCallback)
        {
            SafeNativeMethods.MciSendString(lpstrCommand, lpstrReturnString, uReturnLength, hwndCallback);
        }
    }
}