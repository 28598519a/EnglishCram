using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            protected struct TokenKey
            {
                /// <summary>
                /// ttk前半的值(總小時)
                /// </summary>
                public long Time { get; }

                /// <summary>
                /// ttk後半的值
                /// </summary>
                public long Value { get; }

                /// <param name="time">ttk前半的值(Utc小時)</param>
                /// <param name="value">ttk後半的值</param>
                public TokenKey(long time, long value)
                {
                    Time = time;
                    Value = value;
                }
            }

            protected TokenKey CurrentKey;
            /// <summary>
            /// 初始化一個新的實例 <see cref="GoogleKeyTokenGenerator"/> 類
            /// 產生tkk初始值
            /// 備註 : 經測試，如果Google沒有修改驗證方式的話，tkk值可以不變
            /// </summary>
            public GoogleKeyTokenGenerator()
            {
                CurrentKey = new TokenKey(433167, 3105097842);
            }

            /// <summary>
            /// Google Translate地址
            /// </summary>
            protected readonly Uri Address = new Uri("https://translate.google.com");

            /// <summary>
            /// 自世界標準時間1970年1月1日00:00:00開始，經過的小時數
            /// </summary>
            protected int UtcTotalHours
            {
                get
                {
                    DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return (int)DateTime.UtcNow.Subtract(unixTime).TotalHours;
                }
            }
            /// <summary>
            /// token後半段是否該換了 (True 不能)
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @2
            /// 函數名: fo
            /// 備註: if (null !== eo) var b = eo;
            /// </summary>
            public bool IsExternalKeyObsolete => CurrentKey.Time != UtcTotalHours;

            /// <summary>
            /// 最長等待時間
            /// </summary>
            public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

            /// <summary>
            /// 由輸入文字產生Google翻譯Token
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @2
            /// 函數名: fo
            /// 備註: 合併為完整的fo
            /// </summary>
            /// <param name="source">輸入文字</param>
            /// <returns>Token值</returns>
            public virtual async Task<string> GenerateAsync(string source)
            {
                if (IsExternalKeyObsolete)
                    try
                    {
                        // 得使用 ConfigureAwait(false) 不然線程會鎖死
                        CurrentKey = await GetNewExternalKeyAsync().ConfigureAwait(false);
                    }
                    catch (TokenKeyParseException)
                    {
                        throw new NotSupportedException("這個方法已經失效 or 有什麼其他東西出錯");
                    }

                long time = DecryptFirstFo(source);

                return time.ToString() + '.' + (time ^ CurrentKey.Time);
            }

            /// <summary>
            /// 用於計算token的前半部分
            /// 本函數請參閱translate_m_zh-TW.js
            /// 標示點: // @2
            /// 函數名: fo
            /// 備註: fo前半段 ;tkk由Google後端直接產生
            /// </summary>
            /// <param name="source">輸入文字</param>
            /// <returns>token後半段</returns>
            protected virtual async Task<TokenKey> GetNewExternalKeyAsync()
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = TimeOut;

                string result;

                using (httpClient)
                {
                    // 得使用 ConfigureAwait(false) 不然線程會鎖死
                    result = await httpClient.GetStringAsync(Address).ConfigureAwait(false);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Address);

                    HttpResponseMessage response;

                    try
                    {
                        response = await httpClient.SendAsync(request);
                    }
                    catch (HttpRequestException ex) when (ex.Message.Contains("503"))
                    {
                        throw new GoogleTranslateIPBannedException();
                    }

                    result = await response.Content.ReadAsStringAsync();
                }

                long tkk;

                try
                {
                    // [^']+ 匹配任何除了 ' 以外的字元
                    Match mt = Regex.Match(result, "tkk:'[^']+'");
                    string tkkText = mt.Value.Replace("tkk:'", string.Empty).Trim('\'');

                    if (tkkText.Equals(null))
                    {
                        throw new TokenKeyParseException("找不到 tkk 位置");
                    }
                    else if (tkkText.Length > 100)
                    {
                        throw new TokenKeyParseException($"tkk 長度異常{Environment.NewLine}tkk: {tkkText}");
                    }

                    string[] splitted = tkkText.Split('.');
                    if (splitted.Length != 2 || !long.TryParse(splitted[1], out tkk))
                    {
                        throw new TokenKeyParseException($"tkk 格式異常{Environment.NewLine}tkk: {tkkText}");
                    }

                }
                catch (ArgumentException)
                {
                    throw new TokenKeyParseException();
                }

                // tkk[0]經測試為Number(new Date().getTime()/3600000)；tkk[1]不確定，先直接請求html
                TokenKey newExternalKey = new TokenKey(UtcTotalHours, tkk);
                return newExternalKey;
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
            private long DecryptFirstFo(string source)
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