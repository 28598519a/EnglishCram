using System;

namespace 一丙英文背起來
{
    namespace GoogleTranslateToken
    {
        /// <summary>
        /// 例外: 被Google封鎖IP
        /// </summary>
        [Serializable]
        public class GoogleTranslateIPBannedException : Exception
        {
            public GoogleTranslateIPBannedException() { }
            public GoogleTranslateIPBannedException(string message) : base("Google封鎖了這個IP (約幾個小時)" + Environment.NewLine + message) { }
        }
    }
}