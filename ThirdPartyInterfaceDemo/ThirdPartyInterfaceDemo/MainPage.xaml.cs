using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using YelpAPI;
using YelpAPI.Objects;


namespace ThirdPartyInterfaceDemo
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            YelpServices.APIKey = "8jpbVBaYsRXWCiTBgTIFig";
        }

        private void btn_radius_Click(object sender, RoutedEventArgs e)
        {
            // get a new interface to yelp for a business querry
            YelpInterface yelpInterface = new YelpInterface(YelpInterface.RequestType.BUSINESS);

            // set the search terms
            String[] tokens = this.txt_querry.Text.Split();
            //yelpInterface.AddParam("term", tokens[0].Trim());
            yelpInterface.AddParam("lat", tokens[0].Trim());
            yelpInterface.AddParam("long", tokens[1].Trim());
            yelpInterface.AddParam("radius", tokens[2].Trim());
            yelpInterface.AddParam("limit", tokens[3].Trim());

            // display the search term
            this.txt_request.Text = yelpInterface.GetRequestString();

            // execute the request
            IAsyncResult asyncResult = yelpInterface.ExecuteRequest(new AsyncCallback(Callback));
        }

        private void btn_business_Click(object sender, RoutedEventArgs e)
        {
            // get a new interface to yelp for a business querry
            YelpInterface yelpInterface = new YelpInterface(YelpInterface.RequestType.PHONE);

            // set the search terms
            String[] tokens = this.txt_business.Text.Split();
            yelpInterface.AddParam("phone", tokens[0].Trim());

            // display the search term
            this.txt_request.Text = yelpInterface.GetRequestString();

            // execute the request
            IAsyncResult asyncResult = yelpInterface.ExecuteRequest(new AsyncCallback(Callback));
        }

        private void Callback(IAsyncResult asynchronousResult)
        {
            // get the yelp interface which initiated this callback
            YelpInterface yelpInterface = (YelpInterface)asynchronousResult.AsyncState;

            // parse the results and get a result object
            SearchResult results = yelpInterface.getResults(asynchronousResult); 

            // do something with the results inside a dispatcher lambda
            txt_result.Dispatcher.BeginInvoke(() => 
                {
                    for (int i = 0; i < results.Businesses.Count(); i++)
                    {
                        txt_result.Text +=
                               results.Businesses[i].Name + ", " +
                               results.Businesses[i].PhoneNumber + ", " +
                               results.Businesses[i].City + '\n';
                    }
                });

        }

        
    }
}
