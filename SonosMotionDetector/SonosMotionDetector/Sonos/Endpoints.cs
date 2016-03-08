namespace SonosMotionDetector.Sonos
{
    public static class Endpoints
    {

        public static class Control
        {
            public static string RenderingControl => "/MediaRenderer/RenderingControl/Control";

            public static string AvTransport => "/MediaRenderer/AVTransport/Control";
        }


        public static class Event
        {
            public static string ZoneGroupTopology => "/ZoneGroupTopology/Event";

            public static string SystemProperties => "/SystemProperties/Event";

            public static string MusicServices => "/MusicServices/Event";

            public static string AlarmClock => "/AlarmClock/Event";
        }


        public static class MediaRenderer
        {
            public static string RenderingControl => "/MediaRenderer/RenderingControl/Event";
        }


        public static class MediaServer
        {
            public static string ContentDirectory => "/MediaServer/ContentDirectory/Event";
        }
    }
}