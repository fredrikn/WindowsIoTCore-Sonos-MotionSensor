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
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "Play",
                             SoapActionVariables.Play);
        }


        public static async Task PauseAsync(string ipAddress)
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "Pause",
                             SoapActionVariables.Pause);
        }


        public static async Task<int> GetVolumeAsync(string ipAddress)
        {
            var response = await SendAction(
                                            ipAddress,
                                            Endpoints.Control.RenderingControl,
                                            ServiceType.RenderingControl,
                                            "GetVolume",
                                            SoapActionVariables.GetVolume);

            return int.Parse(response["CurrentVolume"].InnerText);
        }


        public static async Task SetVolumeAsync(string ipAddress, int volume)
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.RenderingControl,
                             ServiceType.RenderingControl,
                             "SetVolume",
                             SoapActionVariables.SetVolume(volume));
        }


        public static async Task<XmlNode> GetPositionInfoAsync(string ipAddress)
        {
            return await SendAction(
                                    ipAddress,
                                    Endpoints.Control.AvTransport,
                                    ServiceType.AVTransport,
                                    "GetPositionInfo",
                                    SoapActionVariables.GetPositionInfo);
        }


        public static async Task<XmlNode> GetTransportInfoAsync(string ipAddress)
        {
            return await SendAction(
                                    ipAddress,
                                    Endpoints.Control.AvTransport,
                                    ServiceType.AVTransport,
                                    "GetTransportInfo",
                                    SoapActionVariables.GetTransportInfo);
        }

        public static async Task SetAvTransportUriAsync(
                                                        string ipAddress,
                                                        string trackUri,
                                                        string trackMetaData)
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "SetAVTransportURI",
                             SoapActionVariables.SetAVTransportURI(trackUri, trackMetaData));
        }


        public static async Task<XmlNode> GetQueueAsync(
                                                string ipAddress,
                                                int startIndex = 0,
                                                int count = 500)
        {
            var result = await SendAction(
                                    ipAddress,
                                    Endpoints.Control.ContentDirectory,
                                    ServiceType.ContentDirectory,
                                    "Browse",
                                    SoapActionVariables.Browse("Q:0", startIndex, count));

            var doc = new XmlDocument();
            doc.LoadXml(result["Result"].InnerText);

            return doc.FirstChild;
        }


        public static async Task AddUriToQueueAsync(
                                               string ipAddress,
                                               string trackUri,
                                               string trackMetadata = "")
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "AddURIToQueue",
                             SoapActionVariables.AddUriToQueue(trackUri, trackMetadata, 0, 1));
        }

        public static async Task ClearQueueAsync(string ipAddress)
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "RemoveAllTracksFromQueue",
                             SoapActionVariables.RemoveAllTracksFromQueue);
        }


        public static async Task TrackSeekAsync(string ipAddress, string forwardTotime)
        {
            await SendAction(
                             ipAddress,
                             Endpoints.Control.AvTransport,
                             ServiceType.AVTransport,
                             "Seek",
                             SoapActionVariables.Seek("REL_TIME", forwardTotime));
        }


        public static async Task<XmlNode> SendAction(
                                                     string ipAddress,
                                                     string path,
                                                     string service,
                                                     string action,
                                                     string variables)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("USER-AGENT", "Linux UPnP/1.0 Sonos/28.1-86200 (WDCR:Microsoft Windows NT 6.2.9200.0)");

            var request = CreateHttpRequestMessage(
                                                   ipAddress,
                                                   path,
                                                   service,
                                                   action,
                                                   variables);

            var response = await httpClient.SendAsync(request);

            var responseXml = new XmlDocument();
            responseXml.Load(await response.Content.ReadAsStreamAsync());

            var elements = responseXml.GetElementsByTagName($"{action}Response", $"urn:schemas-upnp-org:service:{service}:1");

            if (elements != null && elements.Count > 0)
                return elements[0];
                
            return null;
        }


        private static HttpRequestMessage CreateHttpRequestMessage(
                                                                   string ipAddress,
                                                                   string path,
                                                                   string service,
                                                                   string action,
                                                                   string variables)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, MakeUri(ipAddress, path));

            request.Headers.Add("SOAPACTION", $"\"urn:schemas-upnp-org:service:{service}:1#{action}\"");

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            string xml = CreateUpnpSoapEnvelope(service, action, variables);
            request.Content = new StringContent(xml, Encoding.UTF8, "text/xml");

            return request;
        }


        private static string CreateUpnpSoapEnvelope(
                                                     string service,
                                                     string action,
                                                     string variables)
        {
            return "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                 "  <s:Body>" +
                 $"      <u:{action} xmlns:u=\"urn:schemas-upnp-org:service:{service}:1\">" +
                 $"          {variables}" +
                 $"      </u:{action}>" +
                 "  </s:Body>" +
                 "</s:Envelope>";
        }


        private static Uri MakeUri(string ipAddress, string relativeUrl)
        {
            return new Uri($"http://{ipAddress}:1400{relativeUrl}");
        }
    }
}
