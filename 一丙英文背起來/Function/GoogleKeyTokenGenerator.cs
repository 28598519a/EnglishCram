using System.Collections.Generic;

namespace 一丙英文背起來
{
    namespace GoogleTranslateToken
    {
        /// <summary>
        /// 產生Google翻譯Token
        /// </summary>
        public class GoogleKeyTokenGenerator
        {
            /// <summary>
            /// Google翻譯Token
            /// </summary>
            protected struct TimeTokenKey
            {
                /// <summary>
                /// ttk前半的值
                /// </summary>
                public long Time { get; }

                /// <summary>
                /// ttk後半的值
                /// </summary>
                public long Value { get; }

                /// <param name="time">ttk前半的值</param>
                /// <param name="value">ttk後半的值</param>
                public TimeTokenKey(long time, long value)
                {
                    Time = time;
                    Value = value;
                }
            }

            protected TimeTokenKey CurrentKey;
            /// <summary>
            /// 初始化一個新的實例 <see cref="GoogleKeyTokenGenerator"/> 類
            /// 產生tkk初始值
            /// 備註 : 經測試，如果Google沒有修改驗證方式的話，tkk值可以不變
            /// </summary>
            public GoogleKeyTokenGenerator()
            {
                CurrentKey = new TimeTokenKey(433161, 440706042);
            }

            /// <summary>
            /// 由輸入文字產生Google翻譯Token
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @2
            /// 函數名: fo
            /// 備註: 合併為完整的fo
            /// </summary>
            /// <param name="source">輸入文字</param>
            /// <returns>Token值</returns>
            public string Generate(string source)
            {
                long time = DecryptFo(source);
                return time.ToString() + '.' + (time ^ CurrentKey.Time);
            }

            /// <summary>
            /// 用於計算token的前半部分
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @2
            /// 函數名: fo
            /// 備註: fo後半段
            /// </summary>
            /// <param name="source">輸入文字</param>
            /// <returns>token前半段</returns>
            private long DecryptFo(string source)
            {
                List<long> code = new List<long>();

                // for (var e = [], f = 0, g = 0; g < a.length; g++) {
                for (int g = 0; g < source.Length; g++)
                {
                    // var h = a.charCodeAt(g);
                    int l = source[g];
                    // 128 > h ? e[f++] = h
                    if (l < 128)
                    {
                        code.Add(l);
                    }
                    // : (2048 > h ? e[f++] = h >> 6 | 192
                    else
                    {
                        if (l < 2048)
                        {
                            code.Add(l >> 6 | 192);
                        }
                        // : (55296 == (h & 64512) && g + 1 < a.length && 56320 == (a.charCodeAt(g + 1) & 64512)
                        else
                        {
                            if (55296 == (l & 64512) && g + 1 < source.Length && 56320 == (source[g + 1] & 64512))
                            {
                                // ? (h = 65536 + ((h & 1023) << 10) + (a.charCodeAt(++g) & 1023), e[f++] = h >> 18 | 240, e[f++] = h >> 12 & 63 | 128)
                                l = 65536 + ((l & 1023) << 10) + (source[++g] & 1023);
                                code.Add(l >> 18 | 240);
                                code.Add(l >> 12 & 63 | 128);
                            }
                            // : e[f++] = h >> 12 | 224,
                            else
                            {
                                code.Add(l >> 12 | 224);
                            }
                            // e[f++] = h >> 6 & 63 | 128),
                            code.Add(l >> 6 & 63 | 128);
                        }
                        // e[f++] = h & 63 | 128)
                        code.Add(l & 63 | 128);
                    }
                }

                // a = b;
                long time = CurrentKey.Time;

                // for (f = 0; f < e.length; f++) a += e[f], a = co(a, "+-a^+6");
                foreach (long i in code)
                {
                    time += i;
                    Co(ref time, "+-a^+6");
                }

                // a = co(a, "+-3^+b+-f");
                Co(ref time, "+-3^+b+-f");

                // a ^= Number(d[1]) || 0;
                time ^= CurrentKey.Value;

                // 0 > a && (a = (a & 2147483647) + 2147483648);
                if (time < 0)
                {
                    time = (time & int.MaxValue) + 2147483648;
                }

                // a %= 1E6;
                time %= (long)1E6;

                return time;
            }

            /// <summary>
            /// 用於計算token的前半部分
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @3
            /// 函數名: co
            /// </summary>
            /// <param name="a">輸入文字計算到一半的東西</param>
            /// <param name="b">用完就可丟的參數</param>
            private static void Co(ref long a, string b)
            {
                // ref傳址，讓a的變動直接返回 (偷懶，改了一下不用return)
                for (int c = 0; c < b.Length - 2; c += 3)
                {
                    long d = b[c + 2];

                    d = 'a' <= d ? d - 87 : (long)char.GetNumericValue((char)d);
                    d = '+' == b[c + 1] ? (long)((ulong)a >> (int)d) : a << (int)d;
                    a = '+' == b[c] ? a + d & 4294967295 : a ^ d;
                }
            }
        }
    }
}