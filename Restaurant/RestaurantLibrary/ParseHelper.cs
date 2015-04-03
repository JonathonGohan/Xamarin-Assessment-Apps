using System;
using Parse;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;

namespace RestaurantLibrary
{
	public class ParseHelper
	{
		UserDetailsDataSource datasource;
		Context _context;
		NetworkInfo netInfo;

		public ParseHelper (Context ctx)
		{
			ParseClient.Initialize ("Xv27RuT9DpP5EjXt6WlnEctgzqha4sfjogidGRb8", "lOtaHrEVhQ8ZiG7CRDbowDGULWSU0VKuBwHG7ljr");
			datasource = UserDetailsDataSource.Instance;
			_context = ctx;
		}

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public Task<IEnumerable<ParseObject>> getAllRestaurants()
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("Restaurant");
			return query.FindAsync();
		}

		public Task<IEnumerable<ParseObject>> getAlFoodItemType()
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("FoodItemType");
			return query.FindAsync();
		}

		public Task<IEnumerable<ParseObject>> getAllRestaurantFoodItemsByFoodTypeId(int RestaurantId , int[] typeIds)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("FoodItem").WhereEqualTo ("RestaurantId", RestaurantId).WhereContainedIn ("FoodItemTypeId", typeIds);
			return query.FindAsync();
		}
		public Task<IEnumerable<ParseObject>> getAllRestaurantFoodItemsByFoodItemId(int RestaurantId , int[] itemIds)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("FoodItem").WhereEqualTo ("RestaurantId", RestaurantId).WhereContainedIn ("FoodItemId", itemIds);
			return query.FindAsync();
		}

		public Task<ParseObject> chkExistingUser(String emailid)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("User").WhereEqualTo ("EmailId", emailid);
			return query.FirstAsync ();
		}

		public Task<ParseObject> getUserDetails(String emailid, String password)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("User").WhereEqualTo ("EmailId", emailid).WhereEqualTo ("Password", password);
			return query.FirstAsync();
		}

		public void addNewUser(UserDetails user)
		{
			ParseObject obj = new ParseObject ("User");
			obj ["Username"] = user.Username;
			obj ["Password"] = user.Password;
			obj ["Phone"] = user.Phone;
			obj ["EmailId"] = user.EmailId;
			obj.SaveAsync ();
		}

		public Task<ParseObject> getOrderDetails(int OrderId)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("OrderDetails").WhereEqualTo ("Id", OrderId);
			return query.FirstAsync ();
		}

		public Task<IEnumerable<ParseObject>> getAllBookings(String EmailId)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("BookingDetails").WhereEqualTo ("EmailId", EmailId);
			return query.FindAsync();
		}

		public Task<int> getOrderCount()
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("OrderDetails");
			return query.CountAsync ();
		}

		public Task<int> getBookingCount()
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery ("BookingDetails");
			return query.CountAsync ();
		}

		public void saveOrderDetails(OrderDetails details)
		{
			ParseObject obj = new ParseObject ("OrderDetails");
			obj ["Id"] = details.Id;
			obj ["FoodItems"] = details.strFoodItems;
			obj ["Cost"] = "" + details.Cost;
			obj ["RestaurantId"] = details.RestaurantId;
			obj ["EmailId"] = details.EmailId;
			obj.SaveAsync ();
		}

		public void saveBookingDetails(BookingDetails details)
		{
			ParseObject obj = new ParseObject ("BookingDetails");
			obj ["Id"] = details.Id;
			obj ["EmailId"] = details.EmailId;
			obj ["RestaurantId"] = details.RestaurantId;
			obj ["Persons"] = details.persons;
			obj ["BookedDate"] = details.bookedDate;
			obj ["BookedTime"] = details.bookedTime;
			obj ["isAC"] = details.isAC;
			obj ["OrderId"] = details.OrderId;
			obj.SaveAsync ();
		}
	}
}

