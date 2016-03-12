using System;

namespace SonosMotionDetector.Sonos
{
    class SoapActionVariables
    {
        public static string Play => "<InstanceID>0</InstanceID><Speed>1</Speed>";

        public static string Pause => "<InstanceID>0</InstanceID>";

        public static string SetVolume(int volume) => $"<InstanceID>0</InstanceID><Channel>Master</Channel><DesiredVolume>{volume}</DesiredVolume>";

        public static string GetVolume => "<InstanceID>0</InstanceID><Channel>Master</Channel>";

        public static string GetPositionInfo => "<InstanceID>0</InstanceID>";

        public static string GetTransportInfo => "<InstanceID>0</InstanceID>";
        public static string RemoveAllTracksFromQueue => "<InstanceID>0</InstanceID>";

        public static string Seek(string unit, string trackIndex) => $"<InstanceID>0</InstanceID><Unit>{unit}</Unit><Target>{trackIndex}</Target>";

        public static string SetAVTransportURI(string trackUri, string trackMetaData)
            => $"<InstanceID>0</InstanceID><CurrentURI>{trackUri}</CurrentURI><CurrentURIMetaData>{trackMetaData}</CurrentURIMetaData>";

        public static string Browse(string objectId, int startIndex = 0, int requestedCount = 500) 
            => $"<ObjectID>{objectId}</ObjectID><BrowseFlag>BrowseDirectChildren</BrowseFlag><Filter /><StartingIndex>{startIndex}</StartingIndex><RequestedCount>{requestedCount}</RequestedCount><SortCriteria />";

        internal static string AddUriToQueue(string trackUri, string trackMetadata, int desiredFirstTrackNumberEnqueued, int enqueueAsNext = 1)
            => $"<InstanceID>0</InstanceID><EnqueuedURI>{trackUri}</EnqueuedURI><EnqueuedURIMetaData>{trackMetadata}</EnqueuedURIMetaData><DesiredFirstTrackNumberEnqueued>{desiredFirstTrackNumberEnqueued}</DesiredFirstTrackNumberEnqueued><EnqueueAsNext>{enqueueAsNext}</EnqueueAsNext>";
    }
}
