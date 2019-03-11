using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

using System;

using YoutubeExplode;

namespace SpiderSharp
{
    public abstract partial class SpiderEngine
    {
        [Obsolete("Use RunYoutubeDetailPipeline in SpiderContext")]
        public void AddYoutubeDetailPipeline(string urlField, string detailField, string videoField, string streamField, string captionField)
        {
            this.AddPipeline(it =>
            {
                JObject obj = JObject.FromObject(it);
                var urlVideo = obj[urlField].ToString();

                var client = new YoutubeClient();
                var id = YoutubeClient.ParseVideoId(urlVideo); // "bnsUkE8i0tU"

                obj[detailField] = new JObject();
                if (!string.IsNullOrEmpty(videoField))
                {
                    var video = client.GetVideoAsync(id).Result;
                    var json1 = JsonConvert.SerializeObject(video, new StringEnumConverter());
                    obj[detailField][videoField] = JObject.Parse(json1);
                }

                if (!string.IsNullOrEmpty(streamField))
                {
                    var streamInfoSet = client.GetVideoMediaStreamInfosAsync(id).Result;
                    var json2 = JsonConvert.SerializeObject(streamInfoSet, new StringEnumConverter());
                    obj[detailField][streamField] = JObject.Parse(json2);
                }

                if (!string.IsNullOrEmpty(captionField))
                {
                    var caption = client.GetVideoClosedCaptionTrackInfosAsync(id).Result;
                    var json3 = JsonConvert.SerializeObject(caption, new StringEnumConverter());
                    obj[detailField][captionField] = JArray.Parse(json3);
                }

                //var streamInfo = (YoutubeExplode.Models.MediaStreams.MuxedStreamInfo)streamInfoSet.Muxed.OrderByDescending(o => o.Size).FirstOrDefault();

                //var tmp = $"{Path.GetTempFileName()}.{streamInfo.Container.ToString().ToLower().Trim()}";
                //var tas = client.DownloadMediaStreamAsync(streamInfo, tmp);
                //tas.Wait();

                return obj;
            });
        }

        public void AddYoutubeDetailPipeline(string urlField, string detailField)
        {
            this.AddYoutubeDetailPipeline(urlField, detailField, "video", "stream", "caption");
        }

        public void AddYoutubeDetailPipeline(string urlField)
        {
            this.AddYoutubeDetailPipeline(urlField, urlField + "-detail");
        }
    }
}