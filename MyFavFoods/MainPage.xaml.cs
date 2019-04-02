using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using MyFavFoods.Pages;
using MyFavFoods.YelpAPI;
using MyFavFoods.YelpAPI.Objects;
using System.IO;
using System.IO.IsolatedStorage;

namespace MyFavFoods
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static GeoCoordinateWatcher _watcher;
        private static List<Restaurant> _restaurantitems = new List<Restaurant>();
        private static List<Restaurant> _allItems = new List<Restaurant>();
        private static Uri _backuri;
        //private PivotItem _catPI;
        private static DataBase _db;
        private YelpInterface _yelp;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //if (DB == null)
            //{
            DB = new DataBase();
            //}
            if (_watcher == null)
            {
                InitLoc();
            }

            if (AllItems == null)
            {
                AllItems = DB.CurrRest_Query();
            }
            else if (AllItems.Count == 0)
            {
                GetRestaurantList();
            }
            else
            {
                AllItems.Sort(CompareRestaurants);
                _restaurantitems = AllItems;
                ShowAllTab();
                ShowFavTab();
                ShowCatTab();
            }
        }

        public static List<Restaurant> AllItems
        {
            get
            {
                return _allItems;
            }
            set
            {
                _allItems = value;
            }
        }

        public static List<Restaurant> RestauarantItems
        {
            get
            {
                return _restaurantitems;
            }
            set
            {
                _restaurantitems = value;
            }
        }

        public static DataBase DB
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        public static Double Lat
        {
            get
            {
                return _watcher.Position.Location.Latitude;
            }
        }

        public static Double Long
        {
            get
            {
                return _watcher.Position.Location.Longitude;
            }
        }

        public static GeoCoordinate Location
        {
            get
            {
                return _watcher.Position.Location;
            }
        }


        private void Callback(IAsyncResult asynchronousResult)
        {
            YelpInterface yelpInterface = (YelpInterface)asynchronousResult.AsyncState;
            // parse the results and get a result object
            SearchResult results = yelpInterface.getResults(asynchronousResult);

            if (results.Businesses.Count > 0)
            {
                foreach (Business business in results.Businesses)
                {
                    Restaurant currRest = new Restaurant(business.Name, business.Distance.ToString().Substring(0, 3), business.Categories, business.PhoneNumber, business.Address1, business.City, business.State, business.ZipCode, business.Reviews, (Double)business.Latitude, (Double)business.Longitude);
                    currRest.ImgSrc = business.PhotoUrl;
                    //retrieve a restaurant if already in the database
                    currRest = DB.DataBase_Query(currRest);
                    AllItems.Add(currRest);
                }
                AllItems.Sort(CompareRestaurants);
                //save currect restaurant list to the isolated storage
                DB.CurrRest_Sync(AllItems);
                _restaurantitems = AllItems;
                this.Dispatcher.BeginInvoke(() =>
                      {
                          ShowAllTab();
                          ShowFavTab();
                          ShowCatTab();
                      }
                 );
            }
        }

        private void ShowAllTab()
        {
            ShowRestaurantList(_restaurantitems, this.grdAll);
        }

        private void ShowFavTab()
        {
            ShowRestaurantList(_restaurantitems.Where<Restaurant>(FilterFavs), this.grdFav);
        }

        private static bool FilterFavs(Restaurant Restaurant)
        {
            return Restaurant.IsFav;
        }

        private void ShowCatTab()
        {
            List<String> categories = new List<string>();
            _restaurantitems = DB.CurrRest_Query();
            this.lstCategories.SelectionChanged += CategorySelected;
            foreach (Restaurant RestaurantItem in _restaurantitems)
            {
                foreach (Category category in RestaurantItem.Categories)
                {
                    if (!categories.Contains(category.Name))
                    {
                        ListBoxItem lbi = new ListBoxItem();
                        TextBlock tb = new TextBlock();
                        tb.Text = category.Name;
                        tb.Style = (Style)this.Resources["PhoneTextExtraLargeStyle"];
                        lbi.Content = tb;
                        categories.Add(category.Name);
                        this.lstCategories.Items.Add(lbi);
                    }
                }
            }
        }

        private void ShowRestaurantList(IEnumerable<Restaurant> Restaurants, Grid grd)
        {
            grd.Children.Clear();
            int i = 0;
            foreach (Restaurant RestaurantItem in Restaurants)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(80.0d, GridUnitType.Pixel);
                grd.RowDefinitions.Add(rd);

                Image imgPic = new Image();

                IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
                if (myStore.FileExists(RestaurantItem.Phone + ".jpg"))
                {
                    IsolatedStorageFileStream strmSettings = null;
                    strmSettings = new IsolatedStorageFileStream(RestaurantItem.ImgSrc, System.IO.FileMode.Open, myStore);

                    WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(strmSettings);

                    // Add WriteableBitmap object to image control.
                    imgPic.Source = myBitmap;

                    strmSettings.Close();
                }
                else
                {
                    imgPic.Source = new BitmapImage(new Uri(RestaurantItem.ImgSrc, UriKind.RelativeOrAbsolute));
                }
                imgPic.Stretch = Stretch.UniformToFill;
                imgPic.Margin = new Thickness(0.0d, 5.0d, 0.0d, 5.0d);
                imgPic.ManipulationCompleted += RestaurantSelected;
                Grid.SetRow(imgPic, i);
                Grid.SetColumn(imgPic, 0);
                grd.Children.Add(imgPic);

                TextBlock txtName = new TextBlock();
                txtName.Text = RestaurantItem.RestaurantName;
                txtName.FontSize = 40.0d;
                txtName.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtName.ManipulationCompleted += RestaurantSelected;
                Grid.SetColumn(txtName, 1);
                Grid.SetRow(txtName, i);
                grd.Children.Add(txtName);

                TextBlock txtDistance = new TextBlock();
                txtDistance.Text = String.Format("{0} mi", RestaurantItem.Distance);
                txtDistance.FontSize = 40.0d;
                txtDistance.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                txtDistance.ManipulationCompleted += RestaurantSelected;
                Grid.SetColumn(txtDistance, 2);
                Grid.SetRow(txtDistance, i);
                grd.Children.Add(txtDistance);
                i++;
            }
        }

        private void GetRestaurantList()
        {
            //YelpServices.APIKey = "8jpbVBaYsRXWCiTBgTIFig";
            YelpServices.APIKey = "6kkUDie7EZeQw7BEzYepwg";
            this._yelp = new YelpInterface(YelpInterface.RequestType.BUSINESS);

            // set the search terms
            this._yelp.AddParam("term", "food");
            this._yelp.AddParam("lat", Lat.ToString());
            this._yelp.AddParam("long", Long.ToString());
            this._yelp.AddParam("radius", "10");
            this._yelp.AddParam("limit", "6");

            this._yelp.GetRequestString();

            // execute the request
            IAsyncResult asyncResult = this._yelp.ExecuteRequest(new AsyncCallback(Callback));
        }

        private void RestaurantSelected(object sender, ManipulationCompletedEventArgs e)
        {
            if (e.TotalManipulation.Translation.Equals(new Point(0.0d, 0.0d)))
            {
                _backuri = this.NavigationService.CurrentSource;
                List<Restaurant> viewedList = new List<Restaurant>();
                if (this.MainPivot.SelectedIndex == 0)
                {
                    viewedList = _restaurantitems;
                }
                else if (this.MainPivot.SelectedIndex == 1)
                {
                    viewedList = _restaurantitems.Where<Restaurant>(FilterFavs).ToList<Restaurant>();
                }
                DB.Selected_Sync(viewedList.ElementAt<Restaurant>(Grid.GetRow((FrameworkElement)sender)));
                RestaurantPage.CurrRestaurant = viewedList.ElementAt<Restaurant>(Grid.GetRow((FrameworkElement)sender));
                //AllItems.Remove(RestaurantPage.CurrRestaurant);
                //RestauarantItems.Remove(RestaurantPage.CurrRestaurant);
                this.NavigationService.Navigate(new Uri("/RestaurantPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }

        private void CategorySelected(object sender, SelectionChangedEventArgs e)
        {

            int flag = 0;
            Category selectedCat = null;
            foreach (Restaurant rest in _allItems)
            {
                if (flag == 1)
                    break;
                foreach (Category category in rest.Categories)
                {
                    if (category.Name == ((TextBlock)((ListBoxItem)this.lstCategories.SelectedItem).Content).Text)
                    {
                        flag = 1;
                        selectedCat = category;
                        break;
                    }

                }
            }
            //String selectedCat = ((TextBlock)((ListBoxItem)this.lstCategories.SelectedItem).Content).Text;
            if (flag == 1)
            {
		        _restaurantitems = _allItems.Where<Restaurant>(r => r.Categories.Contains(selectedCat)).ToList<Restaurant>();
                this.MainPivot.SelectedIndex = 0;
                ShowAllTab();
                ShowFavTab();
                this.ApplicationTitle.Text = "MYFAVFOODS - " + selectedCat.Name;
            }
            else
            {
		        _restaurantitems = AllItems;
                this.MainPivot.SelectedIndex = 0;
                ShowAllTab();
                ShowFavTab();
                this.ApplicationTitle.Text = "MYFAVFOODS - " + "All";
            }
            /*else if (!this._RestaurantItems.Equals(_allItems))
            {
                this._RestaurantItems = _allItems;
                this.MainPivot.SelectedIndex = 0;
                ShowAllTab();
                ShowFavTab();
                this.ApplicationTitle.Text = "MYFAVFOODS - " + "All";
            }*/
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private static int CompareRestaurants(Restaurant a, Restaurant b)
        {
            float distancea = float.Parse(a.Distance);
            float distanceb = float.Parse(b.Distance);
            return distancea.CompareTo(distanceb);
        }

        #region LocationServices

        private void InitLoc()
        {
            _watcher = new GeoCoordinateWatcher();
            _watcher.MovementThreshold = 20;
            _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            _watcher.TryStart(true, TimeSpan.FromMilliseconds(60000));
        }

        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyStatusChanged(e));
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e));
        }

        // Call the custom method from the StatusChanged event handler.
        void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // The Location Service is disabled or unsupported.
                    // Check to see if the user has disabled the Location Service.
                    if (_watcher.Permission == GeoPositionPermission.Denied)
                    {
                        // The user has disabled the Location Service on their device.
                        //txtStart.Text = "you have disabled the Location Service on your device.";
                        //txtEnd.Text = "you have disabled the Location Service on your device.";
                    }
                    else
                    {
                        //txtStart.Text = "location is not functioning on this device";
                        //txtEnd.Text = "location is not functioning on this device";
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    // The Location Service is initializing.
                    // Disable the Start Location button.
                    break;

                case GeoPositionStatus.NoData:
                    // The Location Service is working, but it cannot get location data.
                    // Alert the user and enable the Stop Location button.
                    //txtStart.Text = "Location data is not available.";
                    break;

                case GeoPositionStatus.Ready:
                    // The Location Service is working and is receiving location data.
                    // Show the current position and enable the Stop Location button.
                    break;
            }
        }

        void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //txtStart.Text = e.Position.Location.Latitude.ToString("0.000");
            //txtEnd.Text = e.Position.Location.Longitude.ToString("0.000");
        }
        #endregion
    }
}
