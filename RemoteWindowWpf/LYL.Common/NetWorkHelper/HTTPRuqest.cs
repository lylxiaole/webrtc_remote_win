using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LYL.Common
{
    public class HTTPRuqest
    {

        private static string Http(string url, string method = "GET", string contenttype = "application/json;charset=utf-8", Dictionary<string, string> header = null, string data = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = string.IsNullOrEmpty(method) ? "GET" : method;
            request.ContentType = string.IsNullOrEmpty(contenttype) ? "application/json;charset=utf-8" : contenttype;
            request.Timeout = 20000;
            if (header != null)
            {
                foreach (var i in header.Keys)
                {
                    request.Headers.Add(i.ToString(), header[i].ToString());
                }
            }
            if (!string.IsNullOrEmpty(data))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                request.ContentLength = bytes.Length;
                Stream RequestStream = request.GetRequestStream(); 
                RequestStream.Write(bytes, 0, bytes.Length); 
                RequestStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream ResponseStream = response.GetResponseStream();
            StreamReader StreamReader = new StreamReader(ResponseStream, Encoding.GetEncoding("utf-8"));
            string re = StreamReader.ReadToEnd();
            StreamReader.Close();
            ResponseStream.Close(); 
            return re;
        }


        private static string httpGetParams(Dictionary<string, string> urlparams = null)
        {
            string url = "";
            if (urlparams != null && urlparams.Keys.Count > 0)
            {
                url += "?";
                foreach (var para in urlparams.Keys)
                {
                    url += para + "=" + urlparams[para] + "&";
                }
                url = url.Substring(0, url.Length - 1);
            }
            return url;
        }


        public static T HttpGetMethod<T>(string url, Dictionary<string, string> urlparams = null, Dictionary<string, string> headers = null)
        {
            url += httpGetParams(urlparams);
            var res = Http(url,header: headers);
            return JsonConvert.DeserializeObject<T>(res);
        }


        public static T HttpPostMethod<T>(string url, object obj, Dictionary<string, string> headers = null)
        {
            var res = Http(url, method: "POST", data: JsonConvert.SerializeObject(obj),header: headers);//得到请求结果
            return JsonConvert.DeserializeObject<T>(res);
        }


        public static T LYLGet<T>(string url, Dictionary<string, string> urlparams = null, Dictionary<string, string> headers = null)
        {
            url += httpGetParams(urlparams);
            var res = Http(url,header: headers);
            var resdata = JsonConvert.DeserializeObject<APIResult<T>>(res);
            if (resdata.success == false)
            {
                throw new Exception(resdata.errMsg);
            }
            else
            {
                return resdata.data;
            }
        }


        public static T LYLPost<T>(string url, object obj, Dictionary<string, string> headers = null)
        {
            var res = Http(url, method: "POST", data: JsonConvert.SerializeObject(obj),header: headers);//得到请求结果
            var resdata = JsonConvert.DeserializeObject<APIResult<T>>(res);
            if (resdata.success == false)
            {
                throw new Exception(resdata.errMsg);
            }
            else
            {
                return resdata.data;
            }
        }
    }
}
