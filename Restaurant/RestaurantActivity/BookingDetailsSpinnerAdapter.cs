using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using RestaurantLibrary;

namespace RestaurantActivity
{
	public class BookingDetailsSpinnerAdapter : ArrayAdapter
	{
		Context _context;
		UserBookingDetailsManager _userBookingDetailsManager;
		String emailId="";
		UserDetailsActivity _activity;

		public BookingDetailsSpinnerAdapter (Context ctx, int layoutId , String email) : base(ctx, layoutId)
		{
			_context = ctx;
			_userBookingDetailsManager = new UserBookingDetailsManager (ctx);
			emailId = email;
			_activity = (UserDetailsActivity)ctx;
			loadAllBookings ();
		}

		async void loadAllBookings()
		{
			int result = await _userBookingDetailsManager.getAllBookings (emailId);
			if (result > 0) 
			{
				NotifyDataSetChanged ();
			}
			_activity.closeProgressBar ();
		}

		public override int Count 
		{
			get 
			{
				return _userBookingDetailsManager.getBookingsCount ();
			}
		}

		public override long GetItemId (int position)
		{
			return _userBookingDetailsManager.getBookingByPosition (position).Id;
		}

		public override View GetDropDownView (int position, View convertView, ViewGroup parent)
		{
			return getCustomView (position, convertView);
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			return getCustomView (position, convertView);
		}

		View getCustomView(int position ,View convertView)
		{
			View row = convertView;

			if (row == null) 
			{
				LayoutInflater inflater = _context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater;
				row = inflater.Inflate (Resource.Layout.BookingDetailsSpinnerItem, null);
			}

			row.FindViewById<TextView> (Resource.Id.BookSpinnerItemRestaurantNameTextView).Text = _userBookingDetailsManager.getRestaurantNameById (_userBookingDetailsManager.getBookingByPosition (position).RestaurantId);
			row.FindViewById<TextView> (Resource.Id.BookSpinnerItemBookingIdTextView).Text = "" + _userBookingDetailsManager.getBookingByPosition (position).Id;
			row.FindViewById<TextView> (Resource.Id.BookSpinnerItemTotalDateTextView).Text = _userBookingDetailsManager.getBookingByPosition (position).bookedDate;

			return row;
		}

		public int displaySelectedBookingDetails(View view , int bookingId)
		{
			BookingDetails details = _userBookingDetailsManager.getBookingDetailsById (bookingId);
			view.FindViewById<TextView> (Resource.Id.BookDetailsRestaurantTextView).Text = _userBookingDetailsManager.getRestaurantNameById (details.RestaurantId);
			view.FindViewById<TextView> (Resource.Id.BookDetailsDateTextView).Text = details.bookedDate;
			view.FindViewById<TextView> (Resource.Id.BookDetailsTimeTextView).Text = details.bookedTime;
			view.FindViewById<TextView> (Resource.Id.BookDetailsPersonTextView).Text = "" + details.persons;
			view.FindViewById<TextView> (Resource.Id.BookDetailsACTextView).Text = details.isAC ? "Yes" : "No";
			return details.OrderId;
		}
	}
}

