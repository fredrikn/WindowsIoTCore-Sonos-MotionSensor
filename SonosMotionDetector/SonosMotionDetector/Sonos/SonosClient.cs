using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SonosMotionDetector.Sonos
{
    public static class SonosClient
    {
        public static async Task PlayAsync(string ipAddress)
        {
            await SonosClient.SendAction(
                            ipAddress,
                            Endpoints.Control.AvTransport,
                            "AVTransport",
                            "Play",
                            SoapActionVariables.Play);
        }


        public static async Task<XmlNode> SendAction(
                                                        string ipAddress,
                                                        string path,
                                                        string service,
                                                        string action,
                                                        string variables)
        {
            string xml =
                 $"<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body><u:{action} xmlns:u =\"urn:schemas-upnp-org:service:{service}:1\">{variables}</u:{action}>/s:Body></s:Envelope>";

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("USER-AGENT", "Linux UPnP/1.0 Sonos/28.1-86200 (WDCR:Microsoft Windows NT 6.2.9200.0)");

            var request = new HttpRequestMessage(HttpMethod.Post, MakeUri(ipAddress, path))
            {
                Content = new StringContent(xml, Encoding.UTF8, "text/xml")
            };

            request.Headers.Add("SOAPACTION", $"\"urn:schemas-upnp-org:service:{service}:1#{action}\"");

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            var response = await httpClient.SendAsync(request);

            if (response.Content != null)
            {
                var responseXml = new XmlDocument();

                responseXml.Load(await response?.Content.ReadAsStreamAsync());

                var elements = responseXml.GetElementsByTagName($"{action}Response", $"urn:schemas-upnp-org:service:{service}:1");

                if (elements != null && elements.Count > 0)
                    return elements[0];
            }

            return null;
        }


        private static Uri MakeUri(string ipAddress, string relativeUrl)
        {
            return new Uri($"http://{ipAddress}:1400{relativeUrl}");
        }
    }
}
