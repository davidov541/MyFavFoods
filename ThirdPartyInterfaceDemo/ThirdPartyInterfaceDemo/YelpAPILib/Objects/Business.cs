using System;
using System.Collections.Generic;
using AjaxLibrary;

namespace YelpAPI.Objects
{
    [JsonSerializable]
    public class Business
    {
        [JsonProperty("id")]
        public String Id
        {
            get;
            set;
        }

        [JsonProperty("name")]
        public String Name
        {
            get;
            set;
        }

        [JsonProperty("phone")]
        public String PhoneNumber
        {
            get;
            set;
        }

        [JsonProperty("url")]
        public String Url
        {
            get;
            set;
        }

        [JsonProperty("uri")]
        public String Uri
        {
            get;
            set;
        }

        #region Location

        [JsonProperty("address1")]
        public String Address1
        {
            get;
            set;
        }

        [JsonProperty("address2")]
        public String Address2
        {
            get;
            set;
        }

        [JsonProperty("address3")]
        public String Address3
        {
            get;
            set;
        }

        [JsonProperty("city")]
        public String City
        {
            get;
            set;
        }

        [JsonProperty("state")]
        public String State
        {
            get;
            set;
        }

        [JsonProperty("state_code")]
        public String StateCode
        {
            get;
            set;
        }

        [JsonProperty("zip")]
        public String ZipCode
        {
            get;
            set;
        }

        [JsonProperty("country")]
        public String Country
        {
            get;
            set;
        }

        [JsonProperty("country_code")]
        public String CountryCode
        {
            get;
            set;
        }

        [JsonProperty("longitude")]
        public Double? Longitude
        {
            get;
            set;
        }

        [JsonProperty("latitude")]
        public Double? Latitude
        {
            get;
            set;
        }

        [JsonProperty("distance")]
        public Double? Distance
        {
            get;
            set;
        }

        #endregion

        #region Misc Urls

        [JsonProperty("mobile_url", SerializeAs = JavascriptType.String)]
        public String MobileUrl
        {
            get;
            set;
        }

        [JsonProperty("nearby_url", SerializeAs = JavascriptType.String)]
        public String NearbyUrl
        {
            get;
            set;
        }

        [JsonProperty("photo_url", SerializeAs = JavascriptType.String)]
        public String PhotoUrl
        {
            get;
            set;
        }

        [JsonProperty("photo_url_small", SerializeAs = JavascriptType.String)]
        public String PhotoUrlSmall
        {
            get;
            set;
        }

        #endregion

        #region Misc

        [JsonProperty("is_closed")]
        public Boolean IsClosed
        {
            get;
            set;
        }

        [JsonProperty("neighborhoods")]
        public List<Neighborhood> Neighborhoods
        {
            get;
            set;
        }

        [JsonProperty("categories")]
        public List<Category> Categories
        {
            get;
            set;
        }

        #endregion

        #region Reviews and Ratings

        [JsonProperty("avg_rating")]
        public Single AverageRating
        {
            get;
            set;
        }

        [JsonProperty("rating_img_url", SerializeAs = JavascriptType.String)]
        public String RatingImageUrl
        {
            get;
            set;
        }

        [JsonProperty("rating_img_url_small", SerializeAs = JavascriptType.String)]
        public String RatingImageUrlSmall
        {
            get;
            set;
        }

        [JsonProperty("review_count")]
        public Int32 ReviewCount
        {
            get;
            set;
        }

        [JsonProperty("reviews")]
        public List<Review> Reviews
        {
            get;
            set;
        }

        #endregion
    }
}
