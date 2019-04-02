using System;
using AjaxLibrary;

namespace YelpAPI.Objects
{
    [JsonSerializable]
    public class Category
    {
        [JsonProperty("name")]
        public String Name
        {
            get;
            set;
        }

        [JsonProperty("category_filter")]
        public String Filter
        {
            get;
            set;
        }

        [JsonProperty("search_url", SerializeAs = JavascriptType.String)]
        public String Url
        {
            get;
            set;
        }
    }
}
