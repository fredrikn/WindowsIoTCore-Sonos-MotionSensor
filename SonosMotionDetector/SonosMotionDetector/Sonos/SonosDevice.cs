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


        public async Task PlayAsync(bool fadeMusic = true)
        {
            if (IsPlaying)
                return;

            if (fadeMusic)
                await SetVolumeAsync(0);

            await SonosClient.PlayAsync(IpAddress);

            if (fadeMusic)
            {
                await GetVolumeAsync();

                //Volume is 0-100. Fade into 50. TODO: Make this configured
                for (int i = 0; i < 50; i++)
                {
                    await SetVolumeAsync(i);
                    await Task.Delay(100);
                }
            }

            IsPlaying = true;
        }


        public async Task SetVolumeAsync(int volume)
        {
            await SonosClient.SendAction(
                                        IpAddress,
                                        Endpoints.Control.RenderingControl,
                                        "RenderingControl",
                                        "SetVolume",
                                        SoapActionVariables.SetVolume(volume));
        }


        public async Task PauseAsync(bool fadeMusic = true)
        {
            if (!IsPlaying)
                return;

            if (fadeMusic)
            {
                //Volume is 0-100. Fade into 0.
                for (int i = await GetVolumeAsync(); i > 0; i--)
                {
                    await SetVolumeAsync(i);
                    await Task.Delay(100);
                }
            }

            await SonosClient.SendAction(
                                         IpAddress,
                                         Endpoints.Control.AvTransport,
                                         "AVTransport",
                                         "Pause",
                                         SoapActionVariables.Pause);

            IsPlaying = false;
        }


        public async Task<int> GetVolumeAsync()
        {
            var response = await SonosClient.SendAction(
                                                         IpAddress,
                                                         Endpoints.Control.RenderingControl,
                                                         "RenderingControl",
                                                         "GetVolume",
                                                         SoapActionVariables.GetVolume);

            return int.Parse(response["CurrentVolume"].InnerText);
        }
    }
}