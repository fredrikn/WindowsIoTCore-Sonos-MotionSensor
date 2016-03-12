using SonosMotionDetector.Sonos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SonosMotionDetector
{
    using System.Threading.Tasks;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        GpioPin _pin11;

        readonly GpioController _gpio;

        IEnumerable<SonosDevice> _sonosDevices;

        SonosDevice _selectedSonosDevice = null;

        readonly DispatcherTimer _idleTimer = new DispatcherTimer();

        private object _lock = new object();

        public MainPage()
        {
            this.InitializeComponent();
            _gpio = GpioController.GetDefault();

            //_selectedSonosDevice = new SonosDevice() { IpAddress = "192.168.1.159" };
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _pin11 = _gpio.OpenPin(27);
            _pin11.SetDriveMode(GpioPinDriveMode.Input);
            _pin11.ValueChanged += Pin_ValueChanged;

            _idleTimer.Interval = new TimeSpan(0, 0, int.Parse(IdleTimeSettings.Text));
            _idleTimer.Tick += _idleTimer_Tick;

            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            DevicesList.IsEnabled = false;
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pin11 != null)
            {
                _pin11.ValueChanged -= Pin_ValueChanged;
                _pin11.Dispose();
            }

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            DevicesList.IsEnabled = true;
        }


        private void DevicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedSonosDevice = _sonosDevices.FirstOrDefault(device => device.RoomName == DevicesList.SelectedItem as string);

            if (_selectedSonosDevice != null)
                StartButton.IsEnabled = true;
        }


        private async void ListPlayers_Click(object sender, RoutedEventArgs e)
        {
            var sonos = new SonosDiscovery();

            _sonosDevices = await sonos.GetZonePlayers();

            foreach (var d in _sonosDevices)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => DevicesList.Items.Add(d.RoomName));
            }
        }


        private async void Pin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    _pin11.ValueChanged -= Pin_ValueChanged;

                    IndicationStatus.Text = args.Edge == GpioPinEdge.RisingEdge ? "DETECTION" : "NOTHING THERE";

                    if (args.Edge == GpioPinEdge.RisingEdge)
                        await PlayDevice();

                    _pin11.ValueChanged += Pin_ValueChanged;

                });
        }


        private async Task PlayDevice()
        {
            _idleTimer.Stop();

            if (await _selectedSonosDevice.IsPlayingAsync())
                return;

            var currentPlayingDevice = await GetFirstPlayingDeviceAsync();

            //TODO: Make sure if more than one player is playing and the music is different, don't play anything.

            if (currentPlayingDevice != null)
            {
                var currentlyPlayingResponse = await currentPlayingDevice.GetPlayingInfoAsync();

                var currentTrackUri = currentlyPlayingResponse["TrackURI"].InnerText;
                var currentTrackMedatada = ""; //currentlyPlayingResponse["TrackMetaData"].InnerText;

                await _selectedSonosDevice.PlayAsync(
                                                    trackUri: currentTrackUri,
                                                    trackMetadata: currentTrackMedatada,
                                                    setVolume: await currentPlayingDevice.GetVolumeAsync());
            }

            _idleTimer.Start();
        }


        private async Task<SonosDevice> GetFirstPlayingDeviceAsync()
        {
            foreach (var device in _sonosDevices)
            {
                if (await device.IsPlayingAsync())
                    return device;
            }

            return null;
        }


        private async void _idleTimer_Tick(object sender, object e)
        {
            _idleTimer.Stop();
            await _selectedSonosDevice.PauseAsync();
        }


        private void IdleTimeSettings_LostFocus(object sender, RoutedEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            IdleTimeSettings.Text = !regex.IsMatch(IdleTimeSettings.Text) ? IdleTimeSettings.Text : "30";
        }
    }
}