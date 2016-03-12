namespace SonosMotionDetector.Sonos
{
    public static class Endpoints
    {

        public static class Control
        {
            public static string RenderingControl => "/MediaRenderer/RenderingControl/Control";

            public static string AvTransport => "/MediaRenderer/AVTransport/Control";

            public static string ContentDirectory => "/MediaServer/ContentDirectory/Control";
        }
    }
}