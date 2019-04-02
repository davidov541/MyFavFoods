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
using System.Windows.Media.Imaging;

namespace MyFavFoods.Controls
{
    public partial class MainControl : UserControl
    {
        private List<Restaraunt> _restarauntItems = new List<Restaraunt>();

        // Constructor
        public MainControl()
        {
            InitializeComponent();
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
            this._restarauntItems.Add(new Restaraunt("Hofbrauhaus", "Images/Bratt.jpg", "1 mi"));
        }

        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MainPivot.SelectedIndex)
            {
                case 0:
                    ShowAllTab();
                    break;
                case 1:
                    ShowFavTab();
                    break;
                case 2:
                    break;
            }
        }

        private void ShowAllTab()
        {
            ShowRestarauntList(this._restarauntItems, this.grdAll);
        }

        private void ShowFavTab()
        {
            ShowRestarauntList(this._restarauntItems, this.grdFav);
        }

        private void ShowRestarauntList(IEnumerable<Restaraunt> restaraunts, Grid grd)
        {
            grd.RowDefinitions.Clear();
            int i = 0;
            foreach (Restaraunt restarauntItem in restaraunts)
            {
                RowDefinition rd = new RowDefinition();
                rd.Height = new GridLength(80.0d, GridUnitType.Pixel);
                grd.RowDefinitions.Add(rd);

                Image imgPic = new Image();
                imgPic.Source = new BitmapImage(new Uri(restarauntItem.ImgSrc, UriKind.Relative));
                imgPic.Stretch = Stretch.Fill;
                imgPic.Margin = new Thickness(0.0d, 5.0d, 0.0d, 5.0d);
                Grid.SetRow(imgPic, i);
                Grid.SetColumn(imgPic, 0);
                grd.Children.Add(imgPic);

                TextBlock txtName = new TextBlock();
                txtName.Text = restarauntItem.RestarauntName;
                txtName.FontSize = 40.0d;
                txtName.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetColumn(txtName, 1);
                Grid.SetRow(txtName, i);
                grd.Children.Add(txtName);

                TextBlock txtDistance = new TextBlock();
                txtDistance.Text = restarauntItem.Distance;
                txtDistance.FontSize = 40.0d;
                txtDistance.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetColumn(txtDistance, 2);
                Grid.SetRow(txtDistance, i);
                grd.Children.Add(txtDistance);

                i++;
            }
        }

        private void grdAll_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RestarauntPage rp = new RestarauntPage();
            
        }
    }
}
