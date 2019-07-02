using System;

namespace 一丙英文背起來
{
    namespace GoogleTranslateToken
    {

        [Serializable]
        public class TokenKeyParseException : Exception
        {
            public TokenKeyParseException() { }
            public TokenKeyParseException(string message) : base("Time Token Key 解析失敗" + Environment.NewLine + message) { }
        }
    }
}