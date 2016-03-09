namespace SonosMotionDetector.Sonos
{
    class SoapActionVariables
    {
        public static string Play => "<InstanceID>0</InstanceID><Speed>1</Speed>";

        public static string Pause => "<InstanceID>0</InstanceID>";

        public static string SetVolume(int volume) => $"<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{volume}</DesiredVolume>";

        public static string GetVolume => "<InstanceID>0</InstanceID><Channel>Master</Channel>";
    }
}
