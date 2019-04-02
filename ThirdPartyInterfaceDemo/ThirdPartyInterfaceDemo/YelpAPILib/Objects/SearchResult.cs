using System.Collections.Generic;
using AjaxLibrary;

namespace YelpAPI.Objects
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
