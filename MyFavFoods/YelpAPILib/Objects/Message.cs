using System;
using MyFavFoods.AjaxLibrary;

namespace MyFavFoods.YelpAPI.Objects
{
    [JsonSerializable]
    public class Message
    {
        [JsonProperty("code", SerializeAs = JavascriptType.Number)]
        public ResponseCodes Code
        {
            get;
            set;
        }

        [JsonProperty("text")]
        public String Text
        {
            get;
            set;
        }

        [JsonProperty("version")]
        public String Version
        {
            get;
            set;
        }
    }
}
