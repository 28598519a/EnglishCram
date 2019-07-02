using System;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace 一丙英文背起來
{
    public static class WebServices
    {
        public static bool CheckVersion(double ClientVersion)
        {
            // 網路連線檢測
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
                    finally
                    {
                        client.Dispose();
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
            }
            else
            {
                Control.ShowAutoClosingMessageBox(
                "              網路未連線" + Environment.NewLine + Environment.NewLine + "(此提示會於2秒後會自動關閉)",
                "更新檢測失敗", 2);
            }
            return false;
        }

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

        public static void DownLoadFile(string downPath, string saveFolder, bool overWrite)
        {
            string savePath = saveFolder + "\\" + GetDownFileName(downPath);

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

        private static IEnumerable<byte> GetDecodedBytes(string encData)
        {
            var encChars = encData.ToCharArray();
            for (int i = 0; i < encChars.Length; i++)
            {
                if (encChars[i] == '%')
                {
                    var hexString = new string(encChars, i + 1, 2);

                    i += 2;

                    int characterValue;
                    if (int.TryParse(hexString, NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture, out characterValue))
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

        private static string DecodeRF5987(string encStr)
        {
            Match m = Regex.Match(encStr, "^(?<e>.+)'(?<l>.*)'(?<d>[^;]+)$");
            if (m.Success)
            {
                //TODO: 此處未包含伺服器傳回資料有誤之容錯處理
                var enc = Encoding.GetEncoding(m.Groups["e"].Value);
                return enc.GetString(GetDecodedBytes(m.Groups["d"].Value).ToArray());
            }
            return encStr;
        }
    }
}