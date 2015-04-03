using System;
using Android.Widget;
using RestaurantLibrary;
using Android.Content;
using Android.Views;
using System.Collections.Generic;

namespace RestaurantActivity
{
	public class FoodItemsListAdapter : BaseAdapter<FoodItem>
	{
		Context _context;
		FoodItemDataSource _foodItemDataSource;
		List<FoodItem> _foodItems;

		public FoodItemsListAdapter (Context ctx, FoodItemManager manager)
		{
			_context = ctx;
			_foodItemDataSource = FoodItemDataSource.Instance;
			_foodItems = new List<FoodItem> ();
			_foodItemDataSource.resetData ();
		}

		#region overrideed Methods

		public override int Count 
		{
			get 
			{
				return _foodItems.Count;
			}
		}

		public override FoodItem this[int index] 
		{
			get 
			{
				return _foodItems[index];
			}
		}

		public override long GetItemId (int position)
		{
			return _foodItems [position].Id;
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View row = convertView;

			if (row == null) 
			{
				LayoutInflater inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				row = inflater.Inflate (Resource.Layout.FoodItemsListViewItem, null);
			}

			FoodItem item = _foodItemDataSource.CurrentOrderDetails.foodItems.Find (e => e.Id == this [position].Id);
			if (item == null)
				(row.FindViewById<TextView> (Resource.Id.FoodItemCountTextView)).Text = "";
			else
				(row.FindViewById<TextView> (Resource.Id.FoodItemCountTextView)).Text = "" + item.OrderCount;

			(row.FindViewById<TextView> (Resource.Id.FoodItemNameTextView)).Text = this [position].Name;
			(row.FindViewById<TextView> (Resource.Id.FoodItemCostTextView)).Text = "Rs. " + this [position].Cost;

			return row;
		}

		#endregion

		public void populateFoodItemsByTypeId(int foodTypeId)
		{
			_foodItems = _foodItemDataSource.getFoodItems (foodTypeId);
			NotifyDataSetChanged ();
		}

		public void updateBookingDetails( int itemId, int count)
		{
			_foodItemDataSource.updateBookingDetails (itemId, count);
		}

		public String getTotalCost()
		{
			return "" + _foodItemDataSource.CurrentOrderDetails.Cost;
		}
	}
}

