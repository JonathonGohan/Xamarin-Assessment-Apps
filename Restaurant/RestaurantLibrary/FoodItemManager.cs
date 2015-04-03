using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using Android.Net;
using Android.Content;
using Android.Widget;
using Android.App;

namespace RestaurantLibrary
{
	public class FoodItemManager
	{
		ParseHelper _parseHelper;
		List<FoodItem> _foodItems;
		List<FoodItemType> _foodItemTypes;
		FoodItemDataSource _foodItemDataSource;
		int restaurantId = -1;
		NetworkInfo netInfo;
		Context _context;

		public FoodItemManager (int restId, Context ctx)
		{
			_context = ctx;
			_foodItemDataSource = FoodItemDataSource.Instance;
			_parseHelper = new ParseHelper (_context);
			_foodItems = new List<FoodItem> ();
			restaurantId = restId;
			_foodItemTypes = new List<FoodItemType>();
		}

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		#region load Data

		public async Task<int> getAllFoodItems(int[] typeIds)
		{
			if (isOnline ()) 
			{
				_foodItemDataSource.resetData ();
				_foodItems = new List<FoodItem> ();
				IEnumerable<ParseObject> objs = await _parseHelper.getAllRestaurantFoodItemsByFoodTypeId (restaurantId, typeIds);
				foreach (ParseObject obj in objs) 
				{
					FoodItem item = new FoodItem ();
					item.Id = obj.Get<int> ("FoodItemId");
					item.RestaurantId = obj.Get<int> ("RestaurantId");
					item.Name = obj.Get<String> ("Name");
					item.TypeId = obj.Get<int> ("FoodItemTypeId");
					item.isVeg = obj.Get<bool> ("isVeg");
					item.Cost = Convert.ToDouble (obj.Get<String> ("Cost"));
					_foodItems.Add (item);
				}
				_foodItemDataSource.SetFoodItems (_foodItems);
				return _foodItems.Count;
			}
			else 
			{
				return -10;
			}
		}

		public async Task<int> getAllFoodItemTypes()
		{
			if (isOnline ()) {
				_foodItemTypes = new List<FoodItemType> ();
				IEnumerable<ParseObject> objs = await _parseHelper.getAlFoodItemType ();
				foreach (ParseObject obj in objs) {
					FoodItemType type = new FoodItemType ();
					type.Id = obj.Get<int> ("Id");
					type.Type = obj.Get<String> ("FoodItemName");
					_foodItemTypes.Add (type);
				}
				_foodItemDataSource.SetFoodItemTypes (_foodItemTypes);
				return _foodItemTypes.Count;
			}
			else 
			{
				return -10;
			}
		}

		#endregion

		#region access Data

		public String getFoodItemTypeByTypeId(int foodTypeId)
		{
			return _foodItemDataSource.getFoodItemTypeByTypeId (foodTypeId);
		}

		public OrderDetails getCurrentOrderDetails()
		{
			return _foodItemDataSource.CurrentOrderDetails;
		}

		public async Task<int> getOrderCount()
		{
			if (isOnline())
				return await _parseHelper.getOrderCount ();
			else
				return -10;
		}

		public async Task<int> getBookingCount()
		{
			if (isOnline ())
				return await _parseHelper.getBookingCount ();
			else
				return -10;
		}

		public void saveOrderDetails(OrderDetails details)
		{
			_parseHelper.saveOrderDetails (details);
		}

		public void saveBookingDetails(BookingDetails details)
		{
			_parseHelper.saveBookingDetails (details);
		}

		#endregion
	}
}

