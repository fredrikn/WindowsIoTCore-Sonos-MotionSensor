using System.Threading.Tasks;

namespace SonosMotionDetector.Sonos
{
    public class SonosDevice
    {
        public string FriendlyName { get; set; }


        public string RoomName { get; set; }


        public string IpAddress { get; set; }


        public bool IsPlaying { get; set; }


        public int ZoneType { get; set; }


        public async Task PlayAsync()
        {
            if (IsPlaying)
                return;

            await SonosClient.SendAction(
                                        IpAddress,
                                        Endpoints.Control.AvTransport,
                                        "urn:schemas-upnp-org:service:AVTransport:1#Play",
                                        SoapActions.Play);

            IsPlaying = true;
        }


        public async Task PauseAsync()
        {
            if (!IsPlaying)
                return;

            await SonosClient.SendAction(
                                         IpAddress,
                                         Endpoints.Control.AvTransport,
                                         "urn:schemas-upnp-org:service:AVTransport:1#Pause",
                                         SoapActions.Pause);

            IsPlaying = false;
        }
    }
}