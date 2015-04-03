using System;
using Android.Widget;
using RestaurantLibrary;
using Android.Content;
using Android.Views;

namespace RestaurantActivity
{
	public class CartListAdapter : BaseAdapter<FoodItem>
	{
		Context _context;
		FoodItemDataSource _foodItemDataSource;

		public CartListAdapter (Context ctx)
		{
			_context = ctx;
			_foodItemDataSource = FoodItemDataSource.Instance;
		}

		#region Overrided Methods

		public override int Count 
		{
			get 
			{
				return _foodItemDataSource.CurrentOrderDetails.foodItems.Count;
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
				return _foodItemDataSource.CurrentOrderDetails.foodItems [index];
			}
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View row = convertView;

			if (row == null) 
			{
				LayoutInflater infalter = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				row = infalter.Inflate (Resource.Layout.CartListItem, null);
			}

			row.FindViewById<TextView> (Resource.Id.CartFoodItemTextView).Text = this [position].Name;
			row.FindViewById<TextView> (Resource.Id.CartFoodTypeTextView).Text = _foodItemDataSource.getFoodItemTypeByTypeId (this [position].TypeId);
			row.FindViewById<TextView> (Resource.Id.CartFoodCostTextView).Text = " Rs. " + this [position].Cost;
			row.FindViewById<TextView> (Resource.Id.CartFoodCountTextView).Text = "" + this [position].OrderCount;

			return row;
		}

		#endregion
	}
}

