using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyFavFoods.YelpAPI.Objects;
using Microsoft.Phone;
using System.IO;
using System.IO.IsolatedStorage;
using MyFavFoods.ImageService;

namespace MyFavFoods.Pages
{
    public partial class RestaurantPage : PhoneApplicationPage
    {
        private static Restaurant _currRestaurant;
        private ApplicationBarIconButton btnFav;
        private CameraCaptureTask _cameracapture;
        private DataBase DB = new DataBase();

        public RestaurantPage()
        {
            InitializeComponent();
		if (_currRestaurant == null)
		{
		    _currRestaurant = DB.Selected_Query();
		}
            this.ApplicationTitle.Text = "MYFAVFOODS - " + _currRestaurant.RestaurantName;
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(_currRestaurant.Phone + ".jpg"))
            {
                IsolatedStorageFileStream strmSettings = null;
                strmSettings = new IsolatedStorageFileStream(_currRestaurant.ImgSrc, System.IO.FileMode.Open, myStore);

                WriteableBitmap myBitmap = PictureDecoder.DecodeJpeg(strmSettings);

                // Add WriteableBitmap object to image control.
                imgPic.Source = myBitmap;

                strmSettings.Close();
            }
            else
                this.imgPic.Source = new BitmapImage(new Uri(_currRestaurant.ImgSrc, UriKind.RelativeOrAbsolute));
                this.txtNotes.Text = _currRestaurant.Notes;
                this.btnFav = (ApplicationBarIconButton)this.ApplicationBar.Buttons[0];
		        this.txtCall.Text = CurrRestaurant.Phone;
            if (CurrRestaurant.IsFav)
            {
                Uri url = new Uri("/Images/appbar.favs.removefrom.rest.png", UriKind.RelativeOrAbsolute);
                btnFav.IconUri = url;
                btnFav.Text = "Remove Fav";
            }
            else
            {
                Uri url = new Uri("/Images/appbar.favs.addto.rest.png", UriKind.RelativeOrAbsolute);
                btnFav.IconUri = url;
                btnFav.Text = "Add Fav";
            }
            this.txtAddress.Text = CurrRestaurant.Address + "\n" + CurrRestaurant.City + " " + CurrRestaurant.State + ", " + CurrRestaurant.Zip;
            
            _cameracapture = new CameraCaptureTask();
            _cameracapture.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
            ShowReviews();

		    imgMap.Visibility = System.Windows.Visibility.Collapsed;

		    GetImage(CurrRestaurant.Lat, CurrRestaurant.Long);
            //mapLoc.Center = MainPage.Location;
        }

        public static Restaurant CurrRestaurant
        {
            get
            {
                return _currRestaurant;
            }
            set
            {
                _currRestaurant = value;
            }
        }

        public void ShowReviews()
        {
            foreach (Review review in CurrRestaurant.Reviews)
            {
                Grid grd = new Grid();
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(1, GridUnitType.Star);
                grd.ColumnDefinitions.Add(cd);
                RowDefinition rd1 = new RowDefinition();
                rd1.Height = new GridLength(2, GridUnitType.Star);
                grd.RowDefinitions.Add(rd1);
                RowDefinition rd2 = new RowDefinition();
                rd2.Height = new GridLength(1, GridUnitType.Star);
                grd.RowDefinitions.Add(rd2);
                RowDefinition rd3 = new RowDefinition();
                rd3.Height = new GridLength(25, GridUnitType.Pixel);
                grd.RowDefinitions.Add(rd3);

                TextBlock tb1 = new TextBlock();
                tb1.Style = (Style)this.Resources["PhoneTextTitle3Style"];
                tb1.Text = review.UserName;
                Grid.SetColumn(tb1, 0);
                Grid.SetRow(tb1, 0);
                grd.Children.Add(tb1);

                TextBlock tb2 = new TextBlock();
                tb2.Style = (Style)this.Resources["PhoneTextNormalStyle"];
                tb2.Text = review.Excerpt;
                tb2.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(tb2, 0);
                Grid.SetRow(tb2, 1);
                grd.Children.Add(tb2);

                Image img = new Image();
                img.Source = new BitmapImage(new Uri(review.RatingImageUrl));
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                img.Stretch = Stretch.Uniform;
                Grid.SetColumn(img, 0);
                Grid.SetRow(img, 2);
                grd.Children.Add(img);
                this.spReviews.Children.Add(grd);
            }
        }

        private void imgFav_ManipulationCompleted(object sender, EventArgs e)
        {
            CurrRestaurant.IsFav = !CurrRestaurant.IsFav;
            if (CurrRestaurant.IsFav)
            {
                btnFav.IconUri = new Uri("/Images/appbar.favs.removefrom.rest.png", UriKind.RelativeOrAbsolute);
                btnFav.Text = "Remove Fav";
            }
            else
            {
                btnFav.IconUri = new Uri("Images/appbar.favs.addto.rest.png", UriKind.RelativeOrAbsolute);
                btnFav.Text = "Add Fav";
            }
            DB.Selected_Sync(CurrRestaurant);
            DB.CurrRest_Sync(CurrRestaurant);
            DB.DataBase_Sync(CurrRestaurant);
        }

        private void imgEdit_ManipulationCompleted(object sender, EventArgs e)
        {
            //NotesPage.CurrRestaurant = CurrRestaurant;
            this.NavigationService.Navigate(new Uri("/NotesPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void imgTakePic_ManipulationCompleted(object sender, EventArgs e)
        {
            _cameracapture.Show();
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                

                IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
		    if (!myStore.FileExists(CurrRestaurant.Phone + ".jpg"))
		    {
			  IsolatedStorageFileStream strmSettings = null;
			  strmSettings = new IsolatedStorageFileStream(CurrRestaurant.Phone + ".jpg", System.IO.FileMode.Create, myStore);
			  WriteableBitmap wb = new WriteableBitmap(bmp);
			  Extensions.SaveJpeg(wb, strmSettings, wb.PixelWidth, wb.PixelHeight, 0, 85);
			  CurrRestaurant.ImgSrc = CurrRestaurant.Phone + ".jpg";
			  imgPic.Source = bmp;
			  if (strmSettings != null)
				strmSettings.Close();
		    }
		    else
		    {
			  IsolatedStorageFileStream strmSettings = null;
			  strmSettings = new IsolatedStorageFileStream(CurrRestaurant.Phone + ".jpg", System.IO.FileMode.Open, myStore);
			  WriteableBitmap wb = new WriteableBitmap(bmp);
			  Extensions.SaveJpeg(wb, strmSettings, wb.PixelWidth, wb.PixelHeight, 0, 85);
			  imgPic.Source = bmp;
			  if (strmSettings != null)
			  {
				strmSettings.Close();
			  }
		    }

                //DB.Selected_Sync(CurrRestaurant);
                DB.CurrRest_Sync(CurrRestaurant);
                DB.DataBase_Sync(CurrRestaurant);
                //this.btnFav.IconUri = new Uri(e.OriginalFileName);
                bmp = null;
            }
        }


        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MainPage.AllItems.Add(CurrRestaurant);
            foreach (Restaurant rest in MainPage.AllItems)
            {
                if (CurrRestaurant.Phone == rest.Phone)
                {
                    rest.Notes = CurrRestaurant.Notes;
                    rest.ImgSrc = CurrRestaurant.ImgSrc;
                    rest.IsFav = CurrRestaurant.IsFav;
                    break;
                }
            }
            e.Cancel = true;
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

	  private void send_SMS(String message)
	  {
		Microsoft.Phone.Tasks.SmsComposeTask sms = new Microsoft.Phone.Tasks.SmsComposeTask();
		sms.Body = message;
		sms.Show();
	  }

	  private void txtText_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
	  {
		if (e.TotalManipulation.Translation.Equals(new Point(0.0, 0.0)))
		{
		    String message = "Meet me here:\n";
		    if (message.Length > 40)
		    {
			  message += CurrRestaurant.RestaurantName.Substring(0, 40) + "\n";
		    }
		    else
		    {
			  message += CurrRestaurant.RestaurantName + "\n";
		    }
		    message += CurrRestaurant.Address + "\n";
		    message += CurrRestaurant.City + ", " + CurrRestaurant.State + " " + CurrRestaurant.Zip + "\n";
		    send_SMS(message);
		}
	  }

	  private void txtCall_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
	  {
		Microsoft.Phone.Tasks.PhoneCallTask call = new PhoneCallTask();
		call.PhoneNumber = CurrRestaurant.Phone;
		call.DisplayName = CurrRestaurant.RestaurantName;
		call.Show();
	  }

	  #region Imagery Service
	  private void GetImage(double latitude, double longitude)
	  {
		int zoomLevel = 17;
		ImageService.GetMapUriRequest mapUriRequest = new ImageService.GetMapUriRequest();

		mapUriRequest.request = new MapUriRequest();

		// Set credentials using a valid Bing Maps key
		mapUriRequest.request.Credentials = new Credentials();
		mapUriRequest.request.Credentials.ApplicationId =
		    "AsLD0lBY5cHNweJ7Qeeo2T5cNiC-HiZugg8Cq8RXwsLFEjv8XX6ftN9AV3-jNOsj";

		// Set the location of the requested image
		mapUriRequest.request.Center = new ImageService.UserLocation();
		mapUriRequest.request.Center.Latitude = latitude;
		mapUriRequest.request.Center.Longitude = longitude;

		// Set the map style and zoom level
		MapUriOptions mapUriOptions = new MapUriOptions();
		mapUriOptions.Style = MapStyle.Road;
		mapUriOptions.ZoomLevel = zoomLevel;

		// Set the size of the requested image to match the size of the image control
		mapUriOptions.ImageSize = new ImageService.SizeOfint();
		mapUriOptions.ImageSize.Width = 200;
		mapUriOptions.ImageSize.Height = 200;

		mapUriRequest.request.Options = mapUriOptions;

		ImageryServiceClient imageSerivce = new ImageryServiceClient("BasicHttpBinding_IImageryService");
		imageSerivce.GetMapUriCompleted += new EventHandler<GetMapUriCompletedEventArgs>(ImageService_GetMapUriCompleted);
		imageSerivce.GetMapUriAsync(mapUriRequest);

	  }

	  void ImageService_GetMapUriCompleted(object sender, GetMapUriCompletedEventArgs e)
	  {
		// The result is an MapUriResponse Object
		GetMapUriResponse mapUriResponse = e.Result;
		BitmapImage bmpImg = new BitmapImage(new Uri(mapUriResponse.GetMapUriResult.Uri));

		imgMap.Source = bmpImg;
		imgMap.Visibility = System.Windows.Visibility.Visible;
	  }

	  #endregion

	  private void txtDirections_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
	  {
		this.NavigationService.Navigate(new Uri("/DirectionsPage.xaml", UriKind.RelativeOrAbsolute));
	  }
    }
}