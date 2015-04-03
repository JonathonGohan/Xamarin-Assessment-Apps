using System;
using System.Collections.Generic;

namespace RestaurantLibrary
{
	public class OrderDetails
	{
		public int Id;
		public int RestaurantId;
		public String EmailId;
		public List<FoodItem> foodItems = new List<FoodItem> ();
		public String strFoodItems;
		public double Cost;
	}

	public class BookingDetails
	{
		public int Id;
		public int RestaurantId;
		public int persons;
		public bool isAC;
		//public bool isFoodSelected;
		public String bookedDate;
		public String bookedTime;
		public int OrderId;
		public String EmailId;
	}

	public class FoodItem
	{
		public int Id;
		public int RestaurantId;
		public String Name;	
		public int TypeId;
		public double Cost;
		public bool isVeg;
		public String TimeServed;
		public int OrderCount;
	}

	public class FoodItemType
	{
		public int Id;
		public String Type;
	}

	public class UserDetails
	{
		public String Username;
		public String EmailId;
		public int Phone;
		public String Password;
		public List<BookingDetails> bookingDetails = new List<BookingDetails>();
	}

	public class Restaurant
	{
		public int Id;
		public String Name;
		public String Address;
		public String Location;
		public String Landmark;
		public int Phone;
		public bool isAC;
		public String FoodType;
		public String FoodItemTypeAvailable;
		public String TimeServed;
	}
}

