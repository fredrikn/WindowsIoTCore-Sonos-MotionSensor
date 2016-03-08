using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SonosMotionDetector.Sonos
{
    public static class SonosClient
    {
        public static async Task SendAction(
                                            string ipAddress,
                                            string path,
                                            string soapAction,
                                            string body)
        {
            string xml =
                 $"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>{body}</s:Body></s:Envelope>";

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("USER-AGENT", "Linux UPnP/1.0 Sonos/28.1-86200 (WDCR:Microsoft Windows NT 6.2.9200.0)");

            var request = new HttpRequestMessage(HttpMethod.Post, MakeUri(ipAddress, path))
            {
                Content = new StringContent(xml)
            };

            request.Headers.Add("SOAPACTION", $"\"{soapAction}\"");

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            await httpClient.SendAsync(request);
        }


        private static Uri MakeUri(string ipAddress, string relativeUrl)
        {
            return new Uri($"http://{ipAddress}:1400{relativeUrl}");
        }
    }
}
