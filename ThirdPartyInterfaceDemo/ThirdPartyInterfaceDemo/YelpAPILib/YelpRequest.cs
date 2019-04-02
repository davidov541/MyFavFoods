using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
//using System.Net;
using AjaxLibrary;

namespace YelpAPI
{
    internal class YelpRequest<TResult>
        where TResult : class, new()
    {
        //private static readonly HttpUtility HttpUtility = new HttpUtility();

        private Dictionary<String, String> m_Parameters = new Dictionary<String, String>();

        private String m_CompleteUri;
        private String m_RequestUri;
        private HttpWebRequest m_HttpRequest;
        public JsonObjectSerializer<TResult> m_Serializer = new JsonObjectSerializer<TResult>();

        public HttpWebRequest getRequest()
        {
            return m_HttpRequest;
        }

        public String getRequestUri()
        {
            return m_CompleteUri;
        }

        public YelpRequest(String requestUri)
        {
            this.m_RequestUri = requestUri;
        }

        public Dictionary<String, String> Parameters
        {
            get { return this.m_Parameters; }
        }

        public void BuildURIString()
        {
            StringBuilder uriBuilder = new StringBuilder();

            uriBuilder.Append(this.m_RequestUri);

            uriBuilder.Append("?ywsid=");
            uriBuilder.Append(YelpServices.APIKey);

            foreach (KeyValuePair<String, String> param in this.m_Parameters)
            {
                if (!String.IsNullOrEmpty(param.Value))
                {
                    uriBuilder.Append('&');
                    uriBuilder.Append(param.Key);
                    uriBuilder.Append('=');
                    uriBuilder.Append(HttpUtility.UrlEncode(param.Value));
                }
            }

            m_CompleteUri = uriBuilder.ToString();
        }

        public void CreateRequest()
        {
            if (this.m_HttpRequest != null)
            {
                throw new InvalidOperationException("YelpRequest instance can only be used for a single request.");
            }

            BuildURIString();

            this.m_HttpRequest = (HttpWebRequest)HttpWebRequest.Create(m_CompleteUri);
        }

        public IAsyncResult NewExecuteRequest(AsyncCallback callback, YelpInterface yi)
        {
            this.CreateRequest();

            Monitor.Enter(YelpServices.GlobalLock);

            try
            {
                TimeSpan timeSinceLastRequest = DateTime.Now.Subtract(YelpServices.LastRequest);

                if (timeSinceLastRequest.TotalSeconds < 2.0)
                {
                    Thread.Sleep(2000 - (Int32)timeSinceLastRequest.TotalMilliseconds);
                }

                IAsyncResult result = (IAsyncResult)this.m_HttpRequest.BeginGetResponse(callback, yi);

                return result;

            }
            finally
            {
                YelpServices.LastRequest = DateTime.Now;

                Monitor.Exit(YelpServices.GlobalLock);
            }
        }
    }
}
