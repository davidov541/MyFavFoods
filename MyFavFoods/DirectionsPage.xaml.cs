using System;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Shell;

namespace MyFavFoods.Pages
{
    public partial class DirectionsPage : PhoneApplicationPage
    {
        MapLayer myRouteLayer;
        // Constructor
        public DirectionsPage()
        {
            //Start: MainPage.Lat, MainPage.Long
            //End: CurrRestaurant.Lat, CurrRestaurant.Long
            InitializeComponent();
            InitializeAppBar();
            CalculateRoute(RestaurantPage.CurrRestaurant.Lat, RestaurantPage.CurrRestaurant.Long);
        }
        #region Initialization
        private void InitializeAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;
            ApplicationBar.IsMenuEnabled = false;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Images/appbar.basecircle.rest.png", UriKind.Relative));
            button1.Text = "me";
            button1.Click += new EventHandler(Button_Me);

            ApplicationBar.Buttons.Add(button1);

        }

        #endregion

        #region EventHandlers
        public void Button_Me(object sender, EventArgs e)
        {
            Location l = new Location();
            l.Latitude = MainPage.Lat;
            l.Longitude = MainPage.Long;
            myMap.SetView(l, 14);
        }
        #endregion

        #region Application Logic

        // This method makes the initial CalculateRoute asynchronous request using the results of the Geocode Service.
        private void CalculateRoute(Double lattitude, Double longitude)
        {
            // Create the service variable and set the callback method using the CalculateRouteCompleted property.
            RouteService.RouteServiceClient routeService = new RouteService.RouteServiceClient("BasicHttpBinding_IRouteService");
            routeService.CalculateRouteCompleted += new EventHandler<RouteService.CalculateRouteCompletedEventArgs>(routeService_CalculateRouteCompleted);

            // Set the token.
            RouteService.RouteRequest routeRequest = new RouteService.RouteRequest();
            routeRequest.Credentials = new RouteService.Credentials();
            routeRequest.Credentials.ApplicationId = ((ApplicationIdCredentialsProvider)myMap.CredentialsProvider).ApplicationId;

            // Return the route points so the route can be drawn.
            routeRequest.Options = new RouteService.RouteOptions();
            routeRequest.Options.RoutePathType = RouteService.RoutePathType.Points;

            // Set the waypoints of the route to be calculated using the Geocode Service results stored in the geocodeResults variable.
            routeRequest.Waypoints = new System.Collections.Generic.List<RouteService.Waypoint>();

            // Add start and end location
            //Start: MainPage.Lat, MainPage.Long
            //End: RestaurantPage.CurrRestaurant.Lat, RestaurantPage.CurrRestaurant
            routeRequest.Waypoints.Add(GeoLocationToWaypoint(MainPage.Lat, MainPage.Long, "my location"));
            routeRequest.Waypoints.Add(GeoLocationToWaypoint(RestaurantPage.CurrRestaurant.Lat, RestaurantPage.CurrRestaurant.Long, RestaurantPage.CurrRestaurant.RestaurantName));

            // Make the CalculateRoute asnychronous request.
            routeService.CalculateRouteAsync(routeRequest);
        }

        private RouteService.Waypoint GeoLocationToWaypoint(Double latitude, Double longitude, String displayName)
        {
            RouteService.Waypoint waypoint = new RouteService.Waypoint();
            waypoint.Description = displayName;
            waypoint.Location = new RouteService.Location();
            waypoint.Location.Latitude = latitude;
            waypoint.Location.Longitude = longitude;
            return waypoint;
        }

        // This is the callback method for the CalculateRoute request.
        private void routeService_CalculateRouteCompleted(object sender, RouteService.CalculateRouteCompletedEventArgs e)
        {

            // If the route calculate was a success and contains a route, then draw the route on the map.
            if ((e.Result.ResponseSummary.StatusCode == RouteService.ResponseStatusCode.Success) & (e.Result.Result.Legs.Count != 0))
            {
                // Set properties of the route line you want to draw.
                Color routeColor = Colors.Blue;
                SolidColorBrush routeBrush = new SolidColorBrush(routeColor);
                MapPolyline routeLine = new MapPolyline();
                routeLine.Locations = new LocationCollection();
                routeLine.Stroke = routeBrush;
                routeLine.Opacity = 0.65;
                routeLine.StrokeThickness = 5.0;

                // Retrieve the route points that define the shape of the route.
                foreach (RouteService.Location p in e.Result.Result.RoutePath.Points)
                {
                    GeoCoordinate loc = new GeoCoordinate(p.Latitude, p.Longitude);
                    routeLine.Locations.Add(loc);
                }

                // Add a map layer in which to draw the route.
                myRouteLayer = new MapLayer();
                myMap.Children.Add(myRouteLayer);

                // Add the route line to the new layer.
                myRouteLayer.Children.Add(routeLine);

                // Figure the rectangle which encompasses the route. This is used later to set the map view.
                Location p0 = routeLine.Locations[0];

                // For the beginning and end geocode result draw a dot on the map.
                DrawEndPoints(routeLine.Locations);

                // Set the map view using the rectangle which bounds the rendered route.
                myMap.SetView(p0, 17);
            }
        }

        private void DrawEndPoints(LocationCollection locationCollection)
        {
            foreach (Location location in locationCollection)
            {
                Ellipse point = new Ellipse();
                point.Width = 20;
                point.Height = 20;
                point.Fill = new SolidColorBrush(Colors.Red);
                point.Opacity = 0.65;

                myRouteLayer.AddChild(point, location);
            }
        }

        #endregion
    }
}
