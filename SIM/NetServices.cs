﻿using System.IO;
using System.Net;

namespace SIM
{
    public static class NetServices
    {
        public static string Platform;
        public static string REQUEST_GET(string URL, string cookies)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.ServicePoint.Expect100Continue = false;
                request.UserAgent = "DeadByDaylight/++UE4+Release-4.25.1-CL-0 Android/7.1.1";
                request.Headers.Add("Cookie", cookies);
                request.Headers.Add("x-kraken-client-version", "39.40.1");
                request.Headers.Add("x-kraken-client-provider", "gas3");
                request.Headers.Add("x-kraken-client-platform", "android");
                request.Headers.Add("x-kraken-client-os", "7.1.1");
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch { return "ERROR"; }
        }
        public static string REQUEST_GET_HEADER(string URL, string cookies)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.ServicePoint.Expect100Continue = false;
                request.UserAgent = "DeadByDaylight/++UE4+Release-4.25.1-CL-0 Android/7.1.1";
                request.Headers.Add("Cookie", cookies);
                request.Headers.Add("x-kraken-client-version", "39.40.1");
                request.Headers.Add("x-kraken-client-provider", "gas3");
                request.Headers.Add("x-kraken-client-platform", "android");
                request.Headers.Add("x-kraken-client-os", "7.1.1");
                request.ContentType = "application/json";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    WebHeaderCollection responseHeaders = response.Headers;
                    return responseHeaders["Kraken-State-Version"];
                }
            }
            catch { return "ERROR"; }
        }
        public static string REQUEST_POST(string URL, string cookies, string content, string contents)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.ServicePoint.Expect100Continue = false;
                request.UserAgent = "DeadByDaylight/++UE4+Release-4.25.1-CL-0 Android/7.1.1";
                request.Headers.Add("Cookie", cookies);
                request.Headers.Add("x-kraken-client-version", "39.40.1");
                request.Headers.Add("x-kraken-client-provider", "gas3");
                request.Headers.Add("x-kraken-client-platform", "android");
                request.Headers.Add("x-kraken-client-os", "7.1.1");
                request.ContentType = "application/octet-stream";
                request.Method = "POST";

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] requestAsByteArray = System.Text.Encoding.UTF8.GetBytes(content);
                    requestStream.Write(requestAsByteArray, 0, requestAsByteArray.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch { return "ERROR"; }
        }
    }
}
