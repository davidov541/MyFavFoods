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
using System.Windows.Media.Imaging;

namespace MyFavFoods.Pages
{
    public partial class RestarauntPage : PhoneApplicationPage
    {
        private static Restaraunt _currRestaraunt;

        public RestarauntPage()
        {
            InitializeComponent();
            this.ApplicationTitle.Text = "MYFAVFOODS - " + _currRestaraunt.RestarauntName;
            this.imgPic.Source = new BitmapImage(new Uri(_currRestaraunt.ImgSrc, UriKind.Relative));
            this.txtNotes.Text = _currRestaraunt.Notes;
        }

      private void Image_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
      {
          if (e.TotalManipulation.Translation.Equals(new Point(0.0d, 0.0d)) && !String.IsNullOrEmpty(this.txtNewNote.Text))
          {
              _currRestaraunt.Notes += "\n" + this.txtNewNote.Text;
              this.txtNewNote.Text = "";
              this.txtNotes.Text = _currRestaraunt.Notes;
          }
      }

      public static Restaraunt CurrRestaraunt
      {
          get
          {
              return _currRestaraunt;
          }
          set
          {
              _currRestaraunt = value;
          }
      }
    }
}