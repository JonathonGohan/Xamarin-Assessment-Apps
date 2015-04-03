using System;
using Android.Widget;
using RestaurantLibrary;
using Android.Views;
using Android.Content;
using Android.App;
using Android.Util;

namespace RestaurantActivity
{
	public class RestaurantNamesListAdapter : BaseAdapter<Restaurant>
	{
		RestaurantManager _restaurantManager;
		Context _context;
		Context _activity;
		TextView _restaurantNameTextView;
		ProgressDialog _progress;

		public override Restaurant this[int index] 
		{
			get 
			{
				return _restaurantManager.getRestaurant (index);
			}
		}

		public override int Count 
		{
			get 
			{
				return _restaurantManager.getRestaurantsCount ();
			}
		}

		public RestaurantNamesListAdapter (Context ctx)
		{
			_restaurantManager = new RestaurantManager (ctx);
			_context = ctx;
			_activity = (Activity)ctx;
			_progress = new ProgressDialog (ctx);
			_progress.SetProgressStyle (ProgressDialogStyle.Spinner);
			populateRestaurants ();
		}

		async void populateRestaurants()
		{
			_progress.SetMessage ("Retreiving Restaurant Details....Please wait !!");
			_progress.Show();
			try
			{
				int restCount = await _restaurantManager.getAllRestaurants();
				if( restCount > 0 ) 
					NotifyDataSetChanged();
				else if( restCount == -10 )
					Toast.MakeText(_context, "No Internet connection", ToastLength.Short).Show();

			}
			catch(Exception e)
			{
				Log.WriteLine (LogPriority.Info, "Error from populate restaurants", e.Message + " : " + e.StackTrace + " : " + e.Source);
				Toast.MakeText (_context, e.Message, ToastLength.Short).Show ();
			}
			finally
			{
				_progress.Dismiss();
			}
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public Restaurant getCurrentRestDetails(int position)
		{
			return _restaurantManager.getRestaurant (position);
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View row = convertView;
			if (row == null) 
			{
				LayoutInflater inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				row = inflater.Inflate (Resource.Layout.RestaurantNameListViewItem, null);
			}

			_restaurantNameTextView = row.FindViewById<TextView> (Resource.Id.ListItemRestaurantNameTextView);
			_restaurantNameTextView.Text = this [position].Name;

			return row;
		}
	}
}

