using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SonosMotionDetector.Network;
using System.Net.Http;
using System.Xml;

namespace SonosMotionDetector.Sonos
{
    public class SonosDiscovery
    {
        SsdpListener _ssdpListener = new SsdpListener();


        public async Task<IEnumerable<SonosDevice>> GetZonePlayers()
        {
            var discoveredSonosDevices = await _ssdpListener.DiscoverAsync(
                                                                            new TimeSpan(0, 0, 0, 0, 2000),
                                                                            "urn:schemas-upnp-org:device:ZonePlayer:1");

            return await GetSonosDevicesAsync(discoveredSonosDevices);
        }


        private static async Task<List<SonosDevice>> GetSonosDevicesAsync(IEnumerable<NetworkService> discoveredSonosDevices)
        {
            var sonosDevices = new List<SonosDevice>();

            foreach (var discoveredSonosDevice in discoveredSonosDevices)
            {
                var httpClient = new HttpClient();

                var doc = new XmlDocument();
                doc.Load(await httpClient.GetStreamAsync(discoveredSonosDevice.Headers["LOCATION"]));

                sonosDevices.Add(CreateSonosDevice(discoveredSonosDevice, doc));
            }

            return sonosDevices;
        }


        private static SonosDevice CreateSonosDevice(NetworkService discoveredSonosDevice, XmlDocument doc)
        {
            var sonosDevice = new SonosDevice();

            sonosDevice.IpAddress = discoveredSonosDevice.IpAddress;
            sonosDevice.RoomName = GetValueFromElement(doc, "roomName");
            sonosDevice.FriendlyName = GetValueFromElement(doc, "friendlyName");
            sonosDevice.ZoneType = int.Parse(GetValueFromElement(doc, "zoneType"));

            return sonosDevice;
        }


        private static string GetValueFromElement(XmlDocument doc, string elementName)
        {
            var element = doc.GetElementsByTagName(elementName);

            if (element.Count > 0)
                return element.Item(0).InnerText;

            return null;
        }
    }
}