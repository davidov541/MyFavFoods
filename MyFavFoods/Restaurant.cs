using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyFavFoods.YelpAPI;
using MyFavFoods.YelpAPI.Objects;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Xml.Serialization;

namespace MyFavFoods
{
    [DataContract(Name = "Restaurant")]
    public class Restaurant
    {
	  [DataMember(Name = "Name")]
	  public String _Restaurantname;

	  [DataMember(Name = "Distance")]
	  public String _distance;

	  [DataMember(Name = "Image")]
	  public String _imgSrc;

	  [DataMember(Name = "Category")]
	  public String _category;

	  [DataMember(Name = "Notes")]
	  public String _notes;

	  [DataMember(Name = "Phone")]
	  public String _phone;

	  [DataMember(Name = "IsFav")]
	  public Boolean _isFav;

	  [DataMember(Name = "Lat")]
	  public Double _lat;

	  [DataMember(Name = "Long")]
	  public Double _long;

	  public Restaurant(String name, String distance, List<Category> categories, String phone, String address, String city, String state, String zip, List<Review> reviews, Double latitude, Double longitude)
	  {
		this.RestaurantName = name;
		this.ImgSrc = "Images/Bratt.jpg";
		this.Distance = distance;
		this.Categories = categories;
		this.IsFav = false;
		this.Notes = String.Empty;
		this.Phone = phone;
		this.Address = address;
		this.City = city;
		this.State = state;
		this.Zip = zip;
		this.Reviews = reviews;
		this.Lat = latitude;
		this.Long = longitude;
	  }

	  public String RestaurantName
	  {
		get;
		set;
	  }

	  public String Address
	  {
		get;
		set;
	  }

	  public String City
	  {
		get;
		set;
	  }

	  public String State
	  {
		get;
		set;
	  }

	  public String Zip
	  {
		get;
		set;
	  }

	  public List<Review> Reviews
	  {
		get;
		set;
	  }

	  public String ImgSrc
	  {
		get;
		set;
	  }

	  public String Distance
	  {
		get;
		set;
	  }

	  public List<Category> Categories
	  {
		get;
		set;
	  }

	  public String Notes
	  {
		get
		{
		    return this._notes;
		}
		set
		{
		    this._notes = value;
		}
	  }

	  public Boolean IsFav
	  {
		get;
		set;
	  }

	  public String Phone
	  {
		get;
		set;
	  }

	  public Double Lat
	  {
		get;
		set;
	  }

	  public Double Long
	  {
		get;
		set;
	  }

      public override bool Equals(object obj)
      {
          return this.Lat.Equals(((Restaurant)obj).Lat) && this.Long.Equals(((Restaurant)obj).Long);
      }
    }
}
