using System;
using AjaxLibrary;

namespace YelpAPI.Objects
{
    [JsonSerializable]
    public class Neighborhood
    {
        [JsonProperty("name")]
        public String Name
        {
            get;
            set;
        }

        [JsonProperty("url", SerializeAs = JavascriptType.String)]
        public String NeighborhoodUrl
        {
            get;
            set;
        }


    }
}
