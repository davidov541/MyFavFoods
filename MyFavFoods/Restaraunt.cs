using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFavFoods
{
    public class Restaraunt
    {
        private String _restarauntname, _distance, _imgSrc, _category, _notes;
        private Boolean _isFav;

        public Restaraunt(String name, String src, String distance, String category, String notes)
        {
            this.RestarauntName = name;
            this.ImgSrc = src;
            this.Distance = distance;
		    this.Category = category;
            this.Notes = notes;
            this.IsFav = false;
        }

        public String RestarauntName
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

	  public String Category
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
    }
}
