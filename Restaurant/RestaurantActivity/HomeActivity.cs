
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestaurantLibrary;

namespace RestaurantActivity
{
	[Activity (Label = "Restaurants")]			
	public class HomeActivity : Activity
	{
		ListView restaurantListView;
		AlertDialog.Builder builder;

		TextView _restNameTextView;
		TextView _addrNameTextView;
		TextView _locNameTextView;
		TextView _phoneTextView;
		TextView _landTextView;
		TextView _foodtypeTextView;
		TextView _acTextView;
		TextView _timeTextView;

		static int _selectedRestaurant=-1;

		RestaurantNamesListAdapter adapter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Home);
			restaurantListView = FindViewById<ListView> (Resource.Id.RestaurantNameListView);
			adapter = new RestaurantNamesListAdapter (this);
			restaurantListView.Adapter = adapter;
			restaurantListView.ItemClick += RestaurantNameClicked;
			builder = new AlertDialog.Builder (this);
			builder.SetCancelable (false);
			builder.SetTitle ("Restaurant Details");
			builder.SetPositiveButton("Next", (object sender2, DialogClickEventArgs e2) => 
			{
					Intent bookingIntent = new Intent(this, typeof(BookingDetailsActivity));
					bookingIntent.PutExtra("SelectedRestaurantId", ""  + _selectedRestaurant );
					StartActivity(bookingIntent);
					OverridePendingTransition (Resource.Animation.leftin, Resource.Animation.leftout);
			});
			builder.SetNegativeButton("Cancel", (object sender2, DialogClickEventArgs e2) => 
			{
			});
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.userDetailsMenu, menu);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			int id = item.ItemId;
			switch (id) 
			{
				case Resource.Id.contextmenu_myProfile:
					Intent userItent = new Intent (this, typeof(UserDetailsActivity));
					StartActivity (userItent);
					OverridePendingTransition (Resource.Animation.pushdownin, Resource.Animation.pushdownout);
					break;
			}
			return true;
		}

		public void RestaurantNameClicked(object sender, AdapterView.ItemClickEventArgs e)
		{
			View alertView = LayoutInflater.Inflate (Resource.Layout.RestaurantDetails, null);
			builder.SetView (alertView);
			_restNameTextView = alertView.FindViewById<TextView> (Resource.Id.RestaurantNameTextView);
			_addrNameTextView = alertView.FindViewById<TextView> (Resource.Id.AddressTextView);
			_locNameTextView = alertView.FindViewById<TextView> (Resource.Id.LocationTextView);
			_phoneTextView = alertView.FindViewById<TextView> (Resource.Id.PhoneTextView);
			_landTextView = alertView.FindViewById<TextView> (Resource.Id.LandmarkTextView);
			_foodtypeTextView = alertView.FindViewById<TextView> (Resource.Id.FoodTypeTextView);
			_acTextView = alertView.FindViewById<TextView> (Resource.Id.ACTextView);
			_timeTextView = alertView.FindViewById<TextView> (Resource.Id.TimeServedTextView);

			Restaurant restdet = adapter.getCurrentRestDetails (e.Position);
			_selectedRestaurant = restdet.Id;
			_restNameTextView.Text =  restdet.Name; 
			_addrNameTextView.Text = restdet.Address;
			_phoneTextView.Text = "" + restdet.Phone;
			_locNameTextView.Text = restdet.Location;
			_landTextView.Text = restdet.Landmark;
			_foodtypeTextView.Text = restdet.FoodType;
			_acTextView.Text = restdet.isAC ? "Yes" : "No";
			_timeTextView.Text = restdet.TimeServed;

			AlertDialog alertDialog  = builder.Create();
			alertDialog.Show ();
		}

		public override void OnBackPressed ()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.rightin, Resource.Animation.rightout);
		}
	}
}

