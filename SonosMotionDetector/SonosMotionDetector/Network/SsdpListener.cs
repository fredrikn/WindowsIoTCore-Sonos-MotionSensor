using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SonosMotionDetector.Network
{
    public class SsdpListener
    {
        public async Task<IEnumerable<NetworkService>> DiscoverAsync(TimeSpan timeout, string stFilter = "ssdp:all")
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentException("Timeout value must be greater than zero.", nameof(timeout));

            var discoveredBridges = new List<NetworkService>();
            var multicastIP = new HostName("239.255.255.250");
            var bridgeWasFound = false;

            using (var socket = new DatagramSocket())
            {
                socket.MessageReceived += (sender, e) =>
                                          {
                                              var reader = e.GetDataReader();
                                              var receivedString = reader.ReadString(reader.UnconsumedBufferLength);

                                              if (string.IsNullOrWhiteSpace(receivedString))
                                                  return;

                                              if (!discoveredBridges.Exists((device) => device.Data.Equals(receivedString)))
                                              {
                                                  discoveredBridges.Add(CreateNetworkService(receivedString, e.RemoteAddress.RawName));
                                                  bridgeWasFound = true;
                                              }
                                          };

                await socket.BindEndpointAsync(null, string.Empty);
                socket.JoinMulticastGroup(multicastIP);

                while (true)
                {
                    bridgeWasFound = false;

                    using (var stream = await socket.GetOutputStreamAsync(multicastIP, "1900"))
                    {
                        using (var writer = new DataWriter(stream))
                        {
                            var request = new StringBuilder();
                            request.AppendLine("M-SEARCH * HTTP/1.1");
                            request.AppendLine("HOST: 239.255.255.250:1900");
                            request.AppendLine("MAN: ssdp:discover");
                            request.AppendLine("MX: 3");
                            request.AppendLine($"ST: {stFilter}");

                            writer.WriteString(request.ToString());
                            await writer.StoreAsync();

                            if (timeout > TimeSpan.Zero)
                                await Task.Delay(timeout);

                            if (!bridgeWasFound)
                                break;
                        }
                    }

                }
            }

            return discoveredBridges;
        }


        private NetworkService CreateNetworkService(string data, string ipAddress)
        {
            var headerCollection = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var headers = new Dictionary<string, string>();

            foreach (var header in headerCollection)
            {
                var matches = Regex.Matches(header, "^([^:]+): (.+)", RegexOptions.IgnoreCase);

                if (matches.Count > 0 && matches[0].Groups != null && matches[0].Groups.Count > 2)
                    headers[matches[0].Groups[1].Value] = matches[0].Groups[2].Value;
            }

            return new NetworkService(data, ipAddress, headers);
        }
    }
}
