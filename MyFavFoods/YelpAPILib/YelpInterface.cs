using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyFavFoods.AjaxLibrary;
using MyFavFoods.YelpAPI.Objects;
using System.IO;

namespace MyFavFoods.YelpAPI
{
    public class YelpInterface
    {
        private RequestType requestType;
        private YelpRequest<SearchResult> yelpRequest;
        public enum RequestType{
            BUSINESS,
            PHONE
        }

        public YelpInterface(RequestType requestType)
        {
            this.requestType = requestType;
            switch (this.requestType)
            {
                case RequestType.BUSINESS:
                    yelpRequest = new YelpRequest<SearchResult>("http://api.yelp.com/business_review_search");
                    break;
                case RequestType.PHONE:
                    yelpRequest = new YelpRequest<SearchResult>("http://api.yelp.com/phone_search");
                    break;
            }
        }

        public void AddParam(String type, String param)
        {
            this.yelpRequest.Parameters.Add(type, param);
        }

        public String GetRequestString()
        {
            this.yelpRequest.BuildURIString();
            return this.yelpRequest.getRequestUri();
        }

        private SearchResult ProcessResponse(HttpWebResponse response)
        {
            if (((Int32)response.StatusCode / 100) == 2)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return yelpRequest.m_Serializer.DeSerialize(reader);
                }
            }
            else
            {
                throw new Exception("Unexpected Response");
            }
        }

        internal IAsyncResult ExecuteRequest(AsyncCallback asyncCallback)
        {
            return this.yelpRequest.NewExecuteRequest(asyncCallback, this);
        }

        internal SearchResult getResults(IAsyncResult asynchronousResult)
        {
            HttpWebResponse response = (HttpWebResponse)this.yelpRequest.getRequest().EndGetResponse(asynchronousResult);
            return ProcessResponse(response);
        }
    }
}
