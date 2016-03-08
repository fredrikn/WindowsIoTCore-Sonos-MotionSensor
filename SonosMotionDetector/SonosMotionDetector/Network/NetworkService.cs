using System.Collections.Generic;

namespace SonosMotionDetector.Network
{
    public class NetworkService
    {
        public NetworkService(
                              string rawData,
                              string ipAddress,
                              Dictionary<string, string> headers)
        {
            Data = rawData;
            IpAddress = ipAddress;
            Headers = headers;
        }

        public string Data { get; }

        public string IpAddress { get; }

        public Dictionary<string, string> Headers { get; }
    }
}
