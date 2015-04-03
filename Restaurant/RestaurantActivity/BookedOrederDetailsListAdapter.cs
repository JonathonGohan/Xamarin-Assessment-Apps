using System;
using Android.Widget;
using RestaurantLibrary;
using Android.Content;
using System.Collections.Generic;
using Android.Views;
using Android.Net;
using System.Threading.Tasks;
using Parse;

namespace RestaurantActivity
{
	public class BookedOrederDetailsListAdapter : BaseAdapter<FoodItem>
	{
		Context _context;
		List<FoodItem> _foodItems = new List<FoodItem>();
		NetworkInfo netInfo;
		OrderDetails _currentOrder;
		ParseHelper _parseHelper;

		public BookedOrederDetailsListAdapter (Context ctx)
		{
			_context = ctx;
			_parseHelper = new ParseHelper (ctx);
		}

		#region Overrided Methods

		public override int Count 
		{
			get 
			{
				return _foodItems.Count;
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override FoodItem this[int index] 
		{
			get 
			{
				return _foodItems[index];
			}
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View row = convertView;

			if (row == null) 
			{
				LayoutInflater infalter = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				row = infalter.Inflate (Resource.Layout.BookedOrderDetailsListItem, null);
			}

			row.FindViewById<TextView> (Resource.Id.BookedDetailsItemNameTextView).Text = this [position].Name;
			row.FindViewById<TextView> (Resource.Id.BookedDetailsItemCostTextView).Text = " Rs. " + this [position].Cost;
			row.FindViewById<TextView> (Resource.Id.BookedOrderDetailsItemCountTextView).Text = "" + this [position].OrderCount;

			return row;
		}

		#endregion

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public async Task<double> getAllFoodItemsByOrder(int orderId)
		{
			if (isOnline ()) 
			{
				ParseObject parseObject = await _parseHelper.getOrderDetails (orderId);

				String _temp = parseObject.Get<String> ("FoodItems");

				if (String.IsNullOrEmpty ( _temp ) == true) 
				{
					return Convert.ToDouble(parseObject.Get<String> ("Cost") );
				}
				else 
				{
					_currentOrder = new OrderDetails ();
					_currentOrder.RestaurantId = parseObject.Get<int> ("RestaurantId");
					_currentOrder.Id = parseObject.Get<int> ("Id");
					_currentOrder.Cost = Convert.ToDouble(parseObject.Get<String> ("Cost") );
					_currentOrder.EmailId = parseObject.Get<String> ("EmailId");
					_currentOrder.strFoodItems= parseObject.Get<String> ("FoodItems");
					_currentOrder.foodItems = new List<FoodItem> ();

					String[] _foodItemsCounts = _currentOrder.strFoodItems.Split (new String[]{ "," }, StringSplitOptions.RemoveEmptyEntries);
					int[] foodItemIds = new int[_foodItemsCounts.Length];
					int[] foodItemTypeCount =  new int[_foodItemsCounts.Length];
					for (int i = 0; i < _foodItemsCounts.Length; i++) 
					{
						foodItemIds [i] = Convert.ToInt32 (_foodItemsCounts [i].Split (new String[]{ ":" }, StringSplitOptions.RemoveEmptyEntries) [0].Trim ());
						foodItemTypeCount [i] = Convert.ToInt32 (_foodItemsCounts [i].Split (new String[]{ ":" }, StringSplitOptions.RemoveEmptyEntries) [1].Trim ());
					}

					IEnumerable<ParseObject> objs = await _parseHelper.getAllRestaurantFoodItemsByFoodItemId( _currentOrder.RestaurantId, foodItemIds );
					int count = 0;
					foreach (ParseObject obj in objs) 
					{
						FoodItem item = new FoodItem ();
						item.Id = obj.Get<int> ("FoodItemId");
						item.RestaurantId = obj.Get<int> ("RestaurantId");
						item.Name = obj.Get<String> ("Name");
						item.TypeId = obj.Get<int> ("FoodItemTypeId");
						item.isVeg = obj.Get<bool> ("isVeg");
						item.Cost = Convert.ToDouble (obj.Get<String> ("Cost"));
						item.OrderCount = foodItemTypeCount [count];
						_foodItems.Add (item);
						count++;
					}
					return _currentOrder.Cost;
				}
			}
			else 
			{
				return -10;
			}
		}
	}
}

