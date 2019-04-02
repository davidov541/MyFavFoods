using System;
using Microsoft.Phone.Controls;

namespace MyFavFoods.Pages
{
    public partial class NotesPage : PhoneApplicationPage
    {
	  private static Restaurant _currrestaurant;
      private DataBase DB = new DataBase();

	  public NotesPage()
	  {
		InitializeComponent();
        _currrestaurant = DB.Selected_Query();
		this.txtNewNotes.Text = _currrestaurant.Notes;
	  }

	  private void imgSubmit_ManipulationCompleted(object sender, EventArgs e)
	  {
		CurrRestaurant.Notes = txtNewNotes.Text;
		RestaurantPage.CurrRestaurant = CurrRestaurant;
		this.NavigationService.Navigate(new Uri("/RestaurantPage.xaml", UriKind.RelativeOrAbsolute));
		DB.DataBase_Sync(CurrRestaurant);
        DB.CurrRest_Sync(CurrRestaurant);
        DB.Selected_Sync(CurrRestaurant);
	  }

	  private void imgCancel_ManipulationCompleted(object sender, EventArgs e)
	  {
          this.NavigationService.Navigate(new Uri("/RestaurantPage.xaml", UriKind.RelativeOrAbsolute));
	  }

	  public static Restaurant CurrRestaurant
	  {
		get
		{
		    return _currrestaurant;
		}
		set
		{
		    _currrestaurant = value;
		}
	  }
    }
}