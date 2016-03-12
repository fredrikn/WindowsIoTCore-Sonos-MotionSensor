using System;
using System.Threading.Tasks;
using System.Xml;

namespace SonosMotionDetector.Sonos
{
    public class SonosDevice
    {
        public string FriendlyName { get; set; }


        public string RoomName { get; set; }


        public string IpAddress { get; set; }


        private bool IsPlaying { get; set; }


        public int ZoneType { get; set; }


        public async Task PlayAsync(
                                    string trackUri = "",
                                    string trackMetadata = "",
                                    int setVolume = 0,
                                    bool fadeMusic = true)
        {
            if (IsPlaying)
                return;

            if (fadeMusic)
                await SetVolumeAsync(0);

            if (!string.IsNullOrWhiteSpace(trackUri))
                await SonosClient.SetAVTransportURIAsync(IpAddress, trackUri, trackMetadata);

            await SonosClient.PlayAsync(IpAddress);

            if (fadeMusic)
                await FadeVolumeUpAsync(setVolume == 0 ? await GetVolumeAsync() : setVolume);

            IsPlaying = true;
        }


        public async Task SetVolumeAsync(int volume)
        {
            if (volume < 0 || volume > 100)
                throw new IndexOutOfRangeException("The volume must be between 0 or 100.");

            await SonosClient.SetVolumeAsync(IpAddress, volume);
        }


        public async Task PauseAsync(bool fadeMusic = true)
        {
            if (!IsPlaying)
                return;

            if (fadeMusic)
                await FadeVolumeDownAsync();

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


        private async Task FadeVolumeUpAsync(int volume)
        {
            for (var i = 0; i < volume; i++)
            {
                await SetVolumeAsync(i);
                await Task.Delay(100);
            }
        }


        private async Task FadeVolumeDownAsync()
        {
            for (var i = await GetVolumeAsync(); i > 0; i--)
            {
                await SetVolumeAsync(i);
                await Task.Delay(100);
            }
        }
    }
}