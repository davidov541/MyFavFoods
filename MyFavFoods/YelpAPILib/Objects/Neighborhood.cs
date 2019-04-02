using System;
using MyFavFoods.AjaxLibrary;

namespace MyFavFoods.YelpAPI.Objects
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
