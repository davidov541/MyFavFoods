using System;
using MyFavFoods.AjaxLibrary;

namespace MyFavFoods.YelpAPI.Objects
{
    [JsonSerializable]
    public class Review
    {
        [JsonProperty("id")]
        public String Id
        {
            get;
            set;
        }

        [JsonProperty("rating")]
        public Int32 Rating
        {
            get;
            set;
        }

        [JsonProperty("rating_img_url", SerializeAs = JavascriptType.String)]
        public string RatingImageUrl
        {
            get;
            set;
        }

        [JsonProperty("rating_img_url_small", SerializeAs = JavascriptType.String)]
        public string RatingImageUrlSmall
        {
            get;
            set;
        }

        [JsonProperty("text_excerpt")]
        public String Excerpt
        {
            get;
            set;
        }

        [JsonProperty("url", SerializeAs = JavascriptType.String)]
        public string Url
        {
            get;
            set;
        }

        [JsonProperty("mobile_uri", SerializeAs = JavascriptType.String)]
        public string MobileUrl
        {
            get;
            set;
        }

        [JsonProperty("user_name")]
        public String UserName
        {
            get;
            set;
        }

        [JsonProperty("user_photo_url", SerializeAs = JavascriptType.String)]
        public string UserPhotoUrl
        {
            get;
            set;
        }

        [JsonProperty("user_photo_url_small", SerializeAs = JavascriptType.String)]
        public string UserPhotoUrlSmall
        {
            get;
            set;
        }

        [JsonProperty("user_url", SerializeAs = JavascriptType.String)]
        public string UserUrl
        {
            get;
            set;
        }

        [JsonProperty("date")]
        public DateTime Date
        {
            get;
            set;
        }
    }
}
