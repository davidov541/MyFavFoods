using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using MyFavFoods.YelpAPI;
using MyFavFoods.YelpAPI.Objects;

namespace MyFavFoods
{
    public class DataBase
    {
        public DataBase()
        {
        }

        /// <summary>
        /// decide if a restaurant is in the fav list
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Boolean IsInFav(string phone)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (!myStore.FileExists("FavFoods.xml"))
            {
                return false;
            }
            else
            {
                IsolatedStorageFileStream strmSettings = null;
                XDocument xmlDoc = null;
                strmSettings = new IsolatedStorageFileStream("FavFoods.xml", System.IO.FileMode.Open, myStore);
                xmlDoc = XDocument.Load(strmSettings);

                var query = from p in xmlDoc.Element("FavFoods").Elements("Restaurant")
                            select p;
                foreach (var record in query)
                {
                    if (record.Element("Phone").Value == phone && record.Element("IsFav").Value == "Y")
                    {
                        if (strmSettings != null)
                            strmSettings.Close();
                        return true;
                    }
                }
                if (strmSettings != null)
                    strmSettings.Close();
                return false;
            }
        }

        /// <summary>
        /// get the selected restaurant from isolated storage
        /// </summary>
        /// <returns></returns>
        public Restaurant Selected_Query()
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (!myStore.FileExists("Selected.xml"))
                return null;
            
            IsolatedStorageFileStream strmSettings = null;
            XDocument xmlDoc = null;

            strmSettings = new IsolatedStorageFileStream("Selected.xml", System.IO.FileMode.Open, myStore);
            xmlDoc = XDocument.Load(strmSettings);
            
            var categories = from p in xmlDoc.Element("FavFoods").Element("Categories").Elements("Category")
                             select p;
            List<Category>record_categories = new List<Category>();
            foreach (string category in categories)
            {   
                Category temp_cat = new Category ();
                temp_cat.Name = category;
                record_categories.Add(temp_cat);
            }

            var reviews = from p in xmlDoc.Element("FavFoods").Element("Reviews").Elements("Review")
                          select p;
            List<Review>record_reviews = new List<Review>();
            foreach(var review in reviews)
            {
                Review temp_rev = new Review();
                temp_rev.UserName = review.Element("UserName").Value;
                temp_rev.Excerpt = review.Element("Excerpt").Value;
                temp_rev.RatingImageUrl = review.Element("Image").Value;
                record_reviews.Add(temp_rev);
            }

            var record = xmlDoc.Element("FavFoods");

            Restaurant selected = new Restaurant(record.Element("RestaurantName").Value,
                                                 record.Element("Distance").Value,
                                                 record_categories,
                                                 record.Element("Phone").Value,
                                                 record.Element("Address").Value,
                                                 record.Element("City").Value,
                                                 record.Element("State").Value,
                                                 record.Element("Zip").Value,
                                                 record_reviews,
								 Double.Parse(record.Element("Lat").Value),
								 Double.Parse(record.Element("Long").Value));
            selected.IsFav = (record.Element("IsFav").Value == "Y" ? true : false);
            selected.Notes = record.Element("Notes").Value;
            selected.ImgSrc = record.Element("Image").Value;

            if (strmSettings != null)
                strmSettings.Close();
            return selected;

        }

        /// <summary>
        /// save the selected restaurant into isolated storage
        /// </summary>
        /// <param name="_restaurant"></param>
        public void Selected_Sync(Restaurant _restaurant)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream strmSettings = null;
            XDocument xmlDoc = null;
            if (myStore.FileExists("Selected.xml"))
                myStore.DeleteFile("Selected.xml");

            strmSettings = new IsolatedStorageFileStream("Selected.xml", System.IO.FileMode.Create, myStore);
            xmlDoc = XDocument.Load("FavFoodsSetting.xml", LoadOptions.None);

            xmlDoc.Root.Add(
                        new XElement("Phone", _restaurant.Phone),
                        new XElement("Notes", _restaurant.Notes),
                        new XElement("Image", _restaurant.ImgSrc),
                        new XElement("IsFav", _restaurant.IsFav == true ? "Y" : "N"),
                        new XElement("RestaurantName", _restaurant.RestaurantName),
                        new XElement("Address",_restaurant.Address),
                        new XElement("City",_restaurant.City),
                        new XElement("State",_restaurant.State),
                        new XElement("Zip",_restaurant.Zip),
                        new XElement("Distance", _restaurant.Distance),
                        new XElement("Reviews"),
                        new XElement("Categories"),
				new XElement("Lat", _restaurant.Lat),
				new XElement("Long", _restaurant.Long)
                        );
           foreach (Category category in _restaurant.Categories)
           {
                xmlDoc.Root.Element("Categories").Add(new XElement("Category",category.Name));
           }
           foreach (Review review in _restaurant.Reviews)
           {
                xmlDoc.Root.Element("Reviews").Add(new XElement("Review", 
                        new XElement("UserName",review.UserName),
                        new XElement("Excerpt",review.Excerpt),
                        new XElement("Image",review.RatingImageUrl)));
           }
           strmSettings.Close();
           strmSettings = myStore.OpenFile("Selected.xml", FileMode.Truncate);
           xmlDoc.Save(strmSettings);

           if (strmSettings != null)
            strmSettings.Close();
           
        }

        /// <summary>
        /// update the database entries given a restaurant
        /// </summary>
        /// <param name="_restaurant"></param>
        public void DataBase_Sync(Restaurant _restaurant)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream strmSettings = null;
            XDocument xmlDoc = null;
            if (!myStore.FileExists("FavFoods.xml"))
            {

                strmSettings = new IsolatedStorageFileStream("FavFoods.xml", System.IO.FileMode.Create, myStore);
                xmlDoc = XDocument.Load("FavFoodsSetting.xml", LoadOptions.None);

                xmlDoc.Root.Add(new XElement("Restaurant",
                        new XElement("Phone", _restaurant.Phone),
                        new XElement("Notes", _restaurant.Notes),
                        new XElement("Image", _restaurant.ImgSrc),
                        new XElement("IsFav", _restaurant.IsFav == true ? "Y" : "N"))
                               );

                strmSettings.Close();
                strmSettings = myStore.OpenFile("FavFoods.xml", FileMode.Truncate);
                xmlDoc.Save(strmSettings);

                if (strmSettings != null)
                    strmSettings.Close();
            }
            else
            {
                strmSettings = new IsolatedStorageFileStream("FavFoods.xml", System.IO.FileMode.Open, myStore);
                xmlDoc = XDocument.Load(strmSettings);

                int rest_flag = 0;
                var query = from p in xmlDoc.Element("FavFoods").Elements("Restaurant")
                            select p;
                foreach (var record in query)
                {
                    if (record.Element("Phone").Value == _restaurant.Phone)
                    {
                        rest_flag = 1;
                        if (record.Element("Notes").Value != _restaurant.Notes)
                            record.Element("Notes").Value = _restaurant.Notes;
                        if (record.Element("Image").Value != _restaurant.ImgSrc)
                            record.Element("Image").Value = _restaurant.ImgSrc;
                        if (record.Element("IsFav").Value != (_restaurant.IsFav == true ? "Y" : "N"))
                            record.Element("IsFav").Value = (_restaurant.IsFav == true ? "Y" : "N");
                        strmSettings.Close();
                        strmSettings = myStore.OpenFile("FavFoods.xml", FileMode.Truncate);

                        xmlDoc.Save(strmSettings);
                        break;
                    }
                }

                if (rest_flag == 0)
                {
                    xmlDoc.Root.Add(new XElement("Restaurant",
                    new XElement("Phone", _restaurant.Phone),
                    new XElement("Notes", _restaurant.Notes),
                    new XElement("Image", _restaurant.ImgSrc),
                    new XElement("IsFav", _restaurant.IsFav ? "Y" : "N"))
                                );
                    strmSettings.Close();
                    strmSettings = myStore.OpenFile("FavFoods.xml", FileMode.Truncate);

                    xmlDoc.Save(strmSettings);
                }

                if (strmSettings != null)
                    strmSettings.Close();
            }

        }

        /// <summary>
        /// get an updated restaurant from local database
        /// </summary>
        /// <param name="_restaurant"></param>
        /// <returns></returns>
        public Restaurant DataBase_Query(Restaurant _restaurant)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (!myStore.FileExists("FavFoods.xml"))
            {
                return _restaurant;
            }
            else
            {
                IsolatedStorageFileStream strmSettings = null;
                XDocument xmlDoc = null;
                strmSettings = new IsolatedStorageFileStream("FavFoods.xml", System.IO.FileMode.Open, myStore);
                xmlDoc = XDocument.Load(strmSettings);

                var query = from p in xmlDoc.Element("FavFoods").Elements("Restaurant")
                            select p;
                foreach (var record in query)
                {
                    if (record.Element("Phone").Value == _restaurant.Phone)
                    {
                        _restaurant.Notes = record.Element("Notes").Value;
                        _restaurant.ImgSrc = record.Element("Image").Value;
                        _restaurant.IsFav = (record.Element("IsFav").Value == "Y");

                        if (strmSettings != null)
                        {
                            strmSettings.Close();
                        }

                        return _restaurant;
                    }
                }
                if (strmSettings != null)
                    strmSettings.Close();
                return _restaurant;
            }
        }

        /// <summary>
        /// Save the current restaurant list into isolated storage
        /// </summary>
        /// <param name="Restaurants"></param>
        public void CurrRest_Sync(List<Restaurant> Restaurants)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream strmSettings = null;
            XDocument xmlDoc = null;

            if (myStore.FileExists("CurrRest.xml"))
                myStore.DeleteFile("CurrRest.xml");
            

            strmSettings = new IsolatedStorageFileStream("CurrRest.xml", System.IO.FileMode.Create, myStore);
            xmlDoc = XDocument.Load("FavFoodsSetting.xml", LoadOptions.None);

            foreach (Restaurant record in Restaurants)
            {
                XElement restaurant = new XElement("Restaurant",
                    new XElement("Phone", record.Phone),
                    new XElement("Notes", record.Notes),
                    new XElement("Image", record.ImgSrc),
                    new XElement("IsFav", record.IsFav == true ? "Y" : "N"),
                    new XElement("RestaurantName", record.RestaurantName),
                    new XElement("Address", record.Address),
                    new XElement("City", record.City),
                    new XElement("State", record.State),
                    new XElement("Zip", record.Zip),
                    new XElement("Distance", record.Distance),
			  new XElement("Lat", record.Lat),
			  new XElement("Long", record.Long));
                    //new XElement("Reviews"),
                    //new XElement("Categories"));
                    
                XElement field_reviews = new XElement("Reviews");    
                foreach (Review review in record.Reviews)
                {
                    field_reviews.Add(new XElement("Review",
                        new XElement("UserName", review.UserName),
                        new XElement("Excerpt", review.Excerpt),
                        new XElement("Image", review.RatingImageUrl)));
                }
                restaurant.Add(field_reviews);

                XElement field_categories = new XElement("Categories");
                foreach (Category category in record.Categories)
                {
                    field_categories.Add(new XElement("Category", category.Name));
                }
                restaurant.Add(field_categories);
                xmlDoc.Root.Add(restaurant);
                  
             }
             strmSettings.Close();
             strmSettings = myStore.OpenFile("CurrRest.xml", FileMode.Truncate);
             xmlDoc.Save(strmSettings);

             if (strmSettings != null)
                strmSettings.Close();
            
        }
        /// <summary>
        /// sync the changed restaurant to the currect restaurant list
        /// </summary>
        /// <param name="Restaurants"></param>
        public void CurrRest_Sync(Restaurant rest)
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (myStore.FileExists("CurrRest.xml"))
            {
                IsolatedStorageFileStream strmSettings = null;
                XDocument xmlDoc = null;

                strmSettings = new IsolatedStorageFileStream("CurrRest.xml", System.IO.FileMode.Open, myStore);
                xmlDoc = XDocument.Load(strmSettings);


                var restaurants = from p in xmlDoc.Element("FavFoods").Elements("Restaurant")
                                 select p;

                foreach (var restaurant in restaurants)
                {
                    if (rest.Phone == restaurant.Element("Phone").Value)
                    {
                        restaurant.Element("Notes").Value = rest.Notes;
                        restaurant.Element("Image").Value = rest.ImgSrc;
                        restaurant.Element("IsFav").Value = (rest.IsFav == true ? "Y" : "N");
                        break;
                    }
                }

                strmSettings.Close();
                strmSettings = myStore.OpenFile("CurrRest.xml", FileMode.Truncate);
                xmlDoc.Save(strmSettings);

                if (strmSettings != null)
                    strmSettings.Close();
            }
        }

        /// <summary>
        /// retrieve the list of current restaurants
        /// </summary>
        /// <returns></returns>
        public List<Restaurant> CurrRest_Query()
        {
            IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication();
            

            if (!myStore.FileExists("CurrRest.xml"))
                return null;

            IsolatedStorageFileStream strmSettings = null;
            XDocument xmlDoc = null;

            List<Restaurant> Restaurants = new List<Restaurant>();
            strmSettings = new IsolatedStorageFileStream("CurrRest.xml", System.IO.FileMode.Open, myStore);
            xmlDoc = XDocument.Load(strmSettings);


            var restaurants = from p in xmlDoc.Element("FavFoods").Elements("Restaurant")
                             select p;
            foreach (var restaurant in restaurants)
            {
                List<Category> record_categories = new List<Category>();
                var categories = from p in restaurant.Element("Categories").Elements("Category")
                                 select p;
                foreach (string category in categories)
                {
                    Category temp_cat = new Category();
                    temp_cat.Name = category;
                    record_categories.Add(temp_cat);
                }

                var reviews = from p in restaurant.Element("Reviews").Elements("Review")
                              select p;
                List<Review> record_reviews = new List<Review>();
                foreach (var review in reviews)
                {
                    Review temp_rev = new Review();
                    temp_rev.UserName = review.Element("UserName").Value;
                    temp_rev.Excerpt = review.Element("Excerpt").Value;
                    temp_rev.RatingImageUrl = review.Element("Image").Value;
                    record_reviews.Add(temp_rev);
                }

                Restaurant returned_rest = new Restaurant(restaurant.Element("RestaurantName").Value, restaurant.Element("Distance").Value,
                                record_categories, restaurant.Element("Phone").Value, restaurant.Element("Address").Value,
                                restaurant.Element("City").Value, restaurant.Element("State").Value,
                                restaurant.Element("Zip").Value, record_reviews,
					  Double.Parse(restaurant.Element("Lat").Value),
					  Double.Parse(restaurant.Element("Long").Value));
                returned_rest.Notes = restaurant.Element("Notes").Value;
                returned_rest.IsFav = (restaurant.Element("IsFav").Value == "Y");
                returned_rest.ImgSrc = restaurant.Element("Image").Value;
                Restaurants.Add(returned_rest);
            }


            if (strmSettings != null)
                strmSettings.Close();
            return Restaurants;

        }
        
    }
    
}
