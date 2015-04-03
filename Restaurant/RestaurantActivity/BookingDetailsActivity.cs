
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
	[Activity (Label = "Booking Details")]			
	public class BookingDetailsActivity : Activity
	{

		RestaurantManager _restaurantManager;
		UserDetailsManager _userDetailsManager;
		Button Proceed;
		TextView _hotelNameTextView;
		TextView _bookedByTextView;
		ImageView _dateImageView;
		ImageView _timeImageView;
		TextView _dateTextView;
		TextView _timeTextView;
		Spinner _personSpinner;
		Spinner _acSpinner;
		String restaurantId = "";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.BookingDetails);
			restaurantId = Intent.GetStringExtra ("SelectedRestaurantId");
			_restaurantManager = new RestaurantManager (this);
			_userDetailsManager = new UserDetailsManager (this);

			_hotelNameTextView = FindViewById<TextView> (Resource.Id.BookHoteNameTextView);
			_bookedByTextView = FindViewById<TextView> (Resource.Id.BookedByTextView);
			_dateImageView = FindViewById<ImageView> ( Resource.Id.DateImageView);
			_timeImageView = FindViewById<ImageView>(Resource.Id.TimeImageView);
			_dateTextView = FindViewById<TextView> (Resource.Id.DateTextView);
			_timeTextView = FindViewById<TextView> (Resource.Id.TimeTextView);
			_personSpinner = FindViewById<Spinner> (Resource.Id.PersonNoSpinner);
			_acSpinner = FindViewById<Spinner> (Resource.Id.ACSpinner);

			Restaurant currentRestaurant = _restaurantManager.getCurrentRestaurant ();

			if ( currentRestaurant.isAC == false) 
			{
				_acSpinner.SetSelection (1);
				_acSpinner.Enabled = false;
			}
			else 
			{
				_acSpinner.SetSelection (0);
				_acSpinner.Enabled = true;
			}

			_dateImageView.Click += (object sender, EventArgs e) => { ShowDialog(1111);	};
			_timeImageView.Click += (object sender, EventArgs e) => { ShowDialog(0); };

			UserDetails currentUser = _userDetailsManager.getCurrentUser ();

			Proceed = FindViewById<Button> (Resource.Id.BookingProceedButton);
			Proceed.Click += (object sender, EventArgs e) => 
			{
				String _bookedDate = FindViewById<TextView>(Resource.Id.DateTextView).Text;
				String _bookedTime =FindViewById<TextView>(Resource.Id.TimeTextView).Text;
				if( String.IsNullOrEmpty(_bookedDate ) == false && String.IsNullOrEmpty(_bookedTime) == false )
				{
					BookingDetails _bookingDetails = new BookingDetails();
					_bookingDetails.EmailId = currentUser.EmailId;
					_bookingDetails.bookedDate = _bookedDate;
					_bookingDetails.bookedTime = _bookedTime;
					_bookingDetails.isAC = FindViewById<Spinner>(Resource.Id.ACSpinner).SelectedItemPosition == 0 ? true : false;
					_bookingDetails.persons =  ( FindViewById<Spinner>(Resource.Id.PersonNoSpinner).SelectedItemPosition ) + 1;
					_bookingDetails.RestaurantId = currentRestaurant.Id;
					_userDetailsManager.setBookingDetails(_bookingDetails);
					Intent foodItem = new Intent(this , typeof(FoodItemsActivity));
					foodItem.PutExtra("RestaurantId", restaurantId);
					StartActivity(foodItem);
					OverridePendingTransition (Resource.Animation.leftin, Resource.Animation.leftout);
				}
				else
				{
					Toast.MakeText(this, "Select Date and Time", ToastLength.Short).Show();
				}
			};

			_hotelNameTextView.Text = currentRestaurant.Name;
			_bookedByTextView.Text = currentUser.Username;
		}

		protected override Dialog OnCreateDialog (int id)
		{
			if (id == 1111) 
			{
				DatePickerDialog datePicker = new DatePickerDialog (this, HandleDateSet, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
				datePicker.DatePicker.MinDate = new Java.Util.Date ().Time - 1000;
				return datePicker;
			}
			else if( id == 0 )
			{
				TimePickerDialog timePicker = new TimePickerDialog (this, TimePickerCallback, DateTime.Now.TimeOfDay.Hours, DateTime.Now.TimeOfDay.Minutes, false);
				return timePicker;
			}
			return null;
		}

		void HandleDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			_dateTextView.Text = e.Date.ToString ("dd-MMM-yyyy");
		}

		private void TimePickerCallback (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			int hour = e.HourOfDay;
			String time = (hour > 12 ? hour - 12 : hour) + " : " + e.Minute + (hour >= 12 ? "  PM" : "  AM");
			_timeTextView.Text = time;
		}

		public override void OnBackPressed ()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.rightin, Resource.Animation.rightout);
		}
	}
}

