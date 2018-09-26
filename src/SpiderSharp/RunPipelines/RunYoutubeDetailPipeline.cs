using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.ClosedCaptions;
using YoutubeExplode.Models.MediaStreams;

namespace SpiderSharp
{
    public partial class SpiderContext
    {
        public void RunYoutubeDetailPipeline(string urlField, string detailField, string videoField, string streamField, string captionField)
        {
            var it = this.Data;
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
        }

        public void RunYoutubeDetailPipeline(string urlField, string detailField)
        {
            this.RunYoutubeDetailPipeline(urlField, detailField, "video", "stream", "caption");
        }

        public void RunYoutubeDetailPipeline(string urlField)
        {
            this.RunYoutubeDetailPipeline(urlField, urlField + "-detail");
        }
    }
}
