using System;
using System.Net;
using System.Text;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 一丙英文背起來
{
    public static class WebServices {
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
                            Control.MainVisibility(false);
                            UpdateWindow UWindow = new UpdateWindow();
                            UWindow.SetUpdateLabel(Rtn_EngCram["ServerVersion"].ToString(), Rtn_EngCram["ReleaseUrl"].ToString());
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
    }
}