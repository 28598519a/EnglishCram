using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 一丙英文背起來
{
    public class WebServices
    {
        /// <summary>
        /// 版本檢查
        /// </summary>
        /// <param name="ClientVersion">版本號</param>
        /// <returns>有新版本</returns>
        public static bool CheckVersion(double ClientVersion)
        {
            if (WebRequestTest(App.Host))
            {
                string url = App.Host + "/macros/s/AKfycbwErFMRlji8MTJzRLOhkSCxoctYFebWyk0wWh7CLfXDVXM8Bnc/exec";
                JObject Webresponse = null;

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        /*
                         * 指定 WebClient 編碼
                         * 指定 WebClient 的 Content-Type header
                         */
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

                        // 設定資料 -> 送出資料 -> JSON反序列化
                        NameValueCollection Postdata = new NameValueCollection();
                        Postdata.Add("ClientVersion", ClientVersion.ToString());
                        Webresponse = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(client.UploadValues(url, "POST", Postdata)));
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message.ToString());
                    }
                }

                try
                {
                    // 解析Server返回的JSON
                    JObject Rtn_EngCram = JObject.Parse(Webresponse["Rtn_EngCram"].ToString());
                    if (Convert.ToDouble(Rtn_EngCram["ServerVersion"].ToString()) > ClientVersion)
                    {
                        UpdateWindow UWindow = new UpdateWindow();
                        UWindow.SetUpdateLabel(Rtn_EngCram["ServerVersion"].ToString(), Rtn_EngCram["ReleaseUrl"].ToString(), Rtn_EngCram["ReleaseDate"].ToString());
                        Control.MainVisibility(false);
                        UWindow.Show();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                Control.ShowAutoClosingMessageBox(
                "              網路未連線" + Environment.NewLine + Environment.NewLine + "(此提示會於2秒後會自動關閉)",
                "更新檢測失敗", 2);
            }
            return false;
        }

        /// <summary>
        /// 網路連線檢測
        /// </summary>
        /// <param name="url">網址</param>
        /// <returns>可連線</returns>
        public static bool WebRequestTest(string url)
        {
            try
            {
                WebRequest myRequest = WebRequest.Create(url);
                WebResponse myResponse = myRequest.GetResponse();
            }
            catch (WebException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="downPath">下載網址</param>
        /// <param name="saveFolder">保存的目錄</param>
        /// <param name="overWrite">覆寫</param>
        public static void DownLoadFile(string downPath, string saveFolder, bool overWrite = false)
        {
            DownLoadFile(downPath, saveFolder, null, overWrite);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="downPath">下載網址</param>
        /// <param name="saveFolder">保存的目錄</param>
        /// <param name="saveName">自訂檔名</param>
        /// <param name="overWrite">覆寫</param>
        public static void DownLoadFile(string downPath, string saveFolder, string saveName, bool overWrite = false)
        {
            string savePath;
            if (saveName.Equals(null))
            {
                savePath = Path.Combine(saveFolder, GetDownFileName(downPath));
            }
            else
            {
                savePath = Path.Combine(saveFolder, saveName);
            }

            if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            if (File.Exists(savePath) && overWrite == false)
                return;

            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile(downPath, savePath);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        /// <summary>
        /// 取得下載檔案的名稱
        /// </summary>
        /// <param name="downPath">下載網址</param>
        /// <returns>下載檔案的名稱</returns>
        private static string GetDownFileName(string downPath)
        {
            using (WebClient wc = new WebClient())
            {
                using (var stream = wc.OpenRead(downPath))
                {
                    //若伺服器未提供檔名，預設以下載時間產生檔名
                    string fn = DateTime.Now.ToString("yyyyMMddHHmmss") + ".data";
                    string cd = wc.ResponseHeaders["content-disposition"];
                    if (!string.IsNullOrEmpty(cd))
                    {
                        Match m = Regex.Match(cd, "filename[*]=(?<es>[^;]+)");
                        if (m.Success)
                        {
                            fn = DecodeRF5987(m.Groups["es"].Value);
                        }
                        else
                        {
                            m = Regex.Match(cd, "filename=[\"]*(?<f>[^\";]+)[\"]*");
                            if (m.Success)
                            {
                                fn = m.Groups["f"].Value;
                                //如伺服器會傳回UrlEncode()格式檔名，視需要加入
                                if (fn.Contains("%")) fn = Uri.UnescapeDataString(fn);
                            }
                        }
                    }
                    return fn;
                }
            }
        }

        /// <summary>
        /// RFC5987解碼器
        /// 參考: https://github.com/grumpydev/RFC5987-Decoder
        /// </summary>
        /// <param name="encData">加密資料</param>
        /// <returns>解密資料</returns>
        private static IEnumerable<byte> GetDecodedBytes(string encData)
        {
            var encChars = encData.ToCharArray();
            for (int i = 0; i < encChars.Length; i++)
            {
                if (encChars[i] == '%')
                {
                    var hexString = new string(encChars, i + 1, 2);

                    i += 2;

                    if (int.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int characterValue))
                    {
                        yield return (byte)characterValue;
                    }
                }
                else
                {
                    yield return (byte)encChars[i];
                }
            }
        }

        /// <summary>
        /// 解密RFC5987標準定義的header加密
        /// </summary>
        /// <param name="encStr">加密內容</param>
        /// <returns>解密內容</returns>
        private static string DecodeRF5987(string encStr)
        {
            Match m = Regex.Match(encStr, "^(?<e>.+)'(?<l>.*)'(?<d>[^;]+)$");
            if (m.Success)
            {
                //注意 : 此處未包含伺服器傳回資料有誤之容錯處理
                var enc = Encoding.GetEncoding(m.Groups["e"].Value);
                return enc.GetString(GetDecodedBytes(m.Groups["d"].Value).ToArray());
            }
            return encStr;
        }

        /// <summary>
        /// 下載Google語音
        /// </summary>
        /// <param name="Text">輸入文字</param>
        /// <param name="languageCode">語音的語言</param>
        /// <returns>語音的保存位置</returns>
        public static string GoogleTransVoice(string Text, LanguageCode.CommonCode languageCode = LanguageCode.CommonCode.en)
        {
            string voice = @"cache\" + Text + "_tts.mp3";
            if (!File.Exists(voice))
            {
                string url = GoogleTransVoiceUrl(Text, languageCode);
                DownLoadFile(url, @"cache\", Text + "_tts.mp3");
            }
            return voice;
        }

        /// <summary>
        /// 組合出Google語音請求API
        /// </summary>
        /// <param name="Text">輸入文字</param>
        /// <param name="languageCode">語音的語言</param>
        /// <returns>語音URL</returns>
        public static string GoogleTransVoiceUrl(string Text, LanguageCode.CommonCode languageCode = LanguageCode.CommonCode.en)
        {
            // 計算Google語音的tk驗證參數
            GoogleTranslateToken.GoogleKeyTokenGenerator generator = new GoogleTranslateToken.GoogleKeyTokenGenerator();
            string token = generator.Generate(Text);
             
            string TTSVoice = "https://translate.google.com/translate_tts?";
            TTSVoice += "&ie=UTF-8";               // 輸入編碼
            TTSVoice += "&q=" + Text;              // 輸入文字
            TTSVoice += "&tl=" + languageCode;     // 輸出語言
            TTSVoice += "&total=1";
            TTSVoice += "&idx=0";
            TTSVoice += "&textlen=" + Text.Length; // 輸入文字長度
            TTSVoice += "&tk=" + token;           // 安全性驗證值
            TTSVoice += "&client=webapp";
            TTSVoice += "&prev=input";
            //TTSVoice += "&ttsspeed = 0.24";       // 語音速度
            return TTSVoice;
        }
    }
}