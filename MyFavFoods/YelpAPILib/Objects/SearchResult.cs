using System.Collections.Generic;
using MyFavFoods.AjaxLibrary;

namespace MyFavFoods.YelpAPI.Objects
{
    [JsonSerializable]
    public class SearchResult
    {
        [JsonProperty("businesses")]
        public List<Business> Businesses
        {
            get;
            set;
        }

        [JsonProperty("message")]
        public Message Message
        {
            get;
            set;
        }
    }
}
