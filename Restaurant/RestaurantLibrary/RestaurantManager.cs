using System;
using Android.Content;
using System.Collections.Generic;
using Parse;
using System.Threading.Tasks;
using Android.Util;
using Android.Net;

namespace RestaurantLibrary
{
	public class RestaurantManager
	{
		RestaurantDataSource _restaurantDataSource;
		FoodItemDataSource _foodItemDataSource;
		List<Restaurant> _restaurants;
		ParseHelper _parseHelper;
		Context _context;
		NetworkInfo netInfo;

		public RestaurantManager (Context ctx)
		{
			_restaurantDataSource = RestaurantDataSource.Instance;
			_foodItemDataSource = FoodItemDataSource.Instance;
			_parseHelper = new ParseHelper (_context);
			_restaurants = new List<Restaurant> ();
			_context = ctx;
		}

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public Restaurant getRestaurant(int position)
		{
			_restaurantDataSource.MoveTo (position);
			return _restaurantDataSource.Current;
		}

		public Restaurant getRestaurantById(int restaurantId)
		{
			return _restaurantDataSource.getRestaurantById (restaurantId);
		}

		public Restaurant getCurrentRestaurant()
		{
			return _restaurantDataSource.Current;
		}

		public int getRestaurantsCount()
		{
			return _restaurantDataSource.Count;
		}

		public async Task<int> getAllRestaurants()
		{
			if (isOnline ()) 
			{
				_restaurants = new List<Restaurant> ();
				IEnumerable<ParseObject> objs = await _parseHelper.getAllRestaurants ();
				foreach (ParseObject obj in objs) {
					Restaurant rest = new Restaurant ();
					rest.Id = obj.Get<int> ("Id");
					rest.Name = obj.Get<String> ("Name");
					rest.Location = obj.Get<String> ("Location");
					rest.Landmark = obj.Get<String> ("Landmark");
					rest.Address = obj.Get<String> ("Address");
					rest.FoodType = obj.Get<String> ("FoodType");
					rest.FoodItemTypeAvailable = obj.Get<String> ("FoodItemTypeAvailable");
					rest.isAC = obj.Get<bool> ("isAC");
					rest.Phone = obj.Get<int> ("Phone");
					rest.TimeServed = obj.Get<String> ("TimeServed");
					_restaurants.Add (rest);
				}
				_restaurantDataSource.SetRestaurants (_restaurants);
				return _restaurants.Count;
			}
			else 
			{
				return -10;
			}
		}
	}
}

