using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFavFoods.YelpAPI
{
    public static class YelpServices
    {
        private static String s_APIKey;
        private static DateTime s_LastRequest;
        private static Object s_GlobalLock = new Object();

        public static String APIKey
        {
            get { return YelpServices.s_APIKey; }
            set { YelpServices.s_APIKey = value; }
        }

        public static DateTime LastRequest
        {
            get { return YelpServices.s_LastRequest; }
            set { YelpServices.s_LastRequest = value; }
        }

        public static Object GlobalLock
        {
            get { return YelpServices.s_GlobalLock; }
            set { YelpServices.s_GlobalLock = value; }
        }
    }
}
