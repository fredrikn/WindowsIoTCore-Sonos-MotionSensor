using System;
using System.Threading.Tasks;
using System.Xml;
using Windows.Devices.AllJoyn;

namespace SonosMotionDetector.Sonos
{
    public class SonosDevice
    {
        public string FriendlyName { get; set; }


        public string RoomName { get; set; }


        public string IpAddress { get; set; }


        private bool IsPlaying { get; set; }


        public int ZoneType { get; set; }


        public async Task PlayAsync()
        {
            if (IsPlaying)
                return;

            await SonosClient.PlayAsync(IpAddress);

            IsPlaying = true;
        }



        public async Task PlayRadioAsync(string currentTrackUri)
        {
            await SonosClient.SetAvTransportUriAsync(IpAddress, currentTrackUri, "");
        }


        public async Task<XmlNode> GetQueueAsync(int startIndex = 0, int requestCount = 500)
        {
            return await SonosClient.GetQueueAsync(IpAddress, startIndex, requestCount);
        }


        public async Task AddUriToQueueAsync(string trackUri, string trackMetadata = "")
        {
            await SonosClient.AddUriToQueueAsync(IpAddress, trackUri, trackMetadata);
        }


        public async Task ForwardToTimeAsync(string time)
        {
            await SonosClient.TrackSeekAsync(IpAddress, time);
        }


        public async Task ClearQueueAsync()
        {
            await SonosClient.ClearQueueAsync(IpAddress);
        }


        public async Task SetVolumeAsync(int volume)
        {
            if (volume < 0 || volume > 100)
                throw new IndexOutOfRangeException("The volume must be between 0 or 100.");

            await SonosClient.SetVolumeAsync(IpAddress, volume);
        }


        public async Task PauseAsync()
        {
            if (!IsPlaying)
                return;

            await SonosClient.PauseAsync(IpAddress);

            IsPlaying = false;
        }


        public async Task<int> GetVolumeAsync()
        {
            return await SonosClient.GetVolumeAsync(IpAddress);
        }


        public async Task<bool> IsPlayingAsync()
        {
            var response = await SonosClient.GetTransportInfoAsync(IpAddress);

            IsPlaying = response["CurrentTransportState"].InnerText == "PLAYING";

            return IsPlaying;
        }


        public async Task<XmlNode> GetPlayingInfoAsync()
        {
            return await SonosClient.GetPositionInfoAsync(IpAddress);
        }
    }
}