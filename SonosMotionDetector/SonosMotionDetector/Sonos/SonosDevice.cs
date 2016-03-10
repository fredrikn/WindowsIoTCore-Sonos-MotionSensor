using System;
using System.Threading.Tasks;

namespace SonosMotionDetector.Sonos
{
    public class SonosDevice
    {
        public string FriendlyName { get; set; }


        public string RoomName { get; set; }


        public string IpAddress { get; set; }


        private bool IsPlaying { get; set; }


        public int ZoneType { get; set; }


        public async Task PlayAsync(bool fadeMusic = true)
        {
            if (IsPlaying)
                return;

            if (fadeMusic)
                await SetVolumeAsync(0);

            await SonosClient.PlayAsync(IpAddress);

            if (fadeMusic)
                await FadeVolumeUpAsync();

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


        public async Task GetCurrentTrackAsync()
        {
            await SonosClient.GetPositionInfoAsync(IpAddress);
        }


        public async Task<bool> IsPlayingAsync()
        {
            var response = await SonosClient.GetTransportInfoAsync(IpAddress);
            return response["CurrentTransportState"].InnerText == "PLAYING";
        }


        private async Task FadeVolumeUpAsync()
        {
            for (int i = 0; i < 50; i++)
            {
                await SetVolumeAsync(i);
                await Task.Delay(100);
            }
        }


        private async Task FadeVolumeDownAsync()
        {
            for (int i = await GetVolumeAsync(); i > 0; i--)
            {
                await SetVolumeAsync(i);
                await Task.Delay(100);
            }
        }
    }
}