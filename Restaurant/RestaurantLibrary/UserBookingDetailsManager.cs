using System;
using Android.Content;
using System.Threading.Tasks;
using Android.Net;
using Parse;
using System.Collections.Generic;

namespace RestaurantLibrary
{
	public class UserBookingDetailsManager
	{
		Context _context;
		UserBookingDetailsDataSource _userBookingsDataSource;
		NetworkInfo netInfo;
		ParseHelper _parseHelper;
		List<BookingDetails> _bookingDetails;
		RestaurantDataSource _restaurantDataSource;

		public UserBookingDetailsManager (Context ctx)
		{
			_context = ctx;
			_userBookingsDataSource = UserBookingDetailsDataSource.Instance;
			_parseHelper = new ParseHelper (ctx);
			_bookingDetails = new List<BookingDetails> ();
			_restaurantDataSource = RestaurantDataSource.Instance;
		}

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public async Task<int> getAllBookings(String emailId)
		{
			if (isOnline ()) 
			{
				IEnumerable<ParseObject> parseObjects = await _parseHelper.getAllBookings (emailId);
				foreach (ParseObject obj in parseObjects) 
				{
					BookingDetails dets = new BookingDetails ();
					dets.bookedDate = obj.Get<String> ("BookedDate");
					dets.bookedTime = obj.Get<String> ("BookedTime");
					dets.EmailId = obj.Get<String> ("EmailId");
					dets.Id = obj.Get<int> ("Id");
					dets.isAC = obj.Get<bool> ("isAC");
					dets.OrderId = obj.Get<int> ("OrderId");
					dets.persons = obj.Get<int> ("Persons");
					dets.RestaurantId = obj.Get<int> ("RestaurantId");
					_bookingDetails.Add (dets);
				}
				_userBookingsDataSource.setBookingDetails (_bookingDetails);
				return _bookingDetails.Count;
			}
			else 
			{
				return -10;
			}
		}

		public BookingDetails getBookingByPosition(int position)
		{
			return _userBookingsDataSource.getBookingDetailsByPosition (position);
		}

		public int getBookingsCount()
		{
			return _userBookingsDataSource.getBookingsCount ();
		}

		public BookingDetails getBookingDetailsById(int bookingId)
		{
			return _userBookingsDataSource.getBookingDetailsById (bookingId);
		}

		public String getRestaurantNameById(int restaurantId)
		{
			return _restaurantDataSource.getRestaurantById (restaurantId).Name;
		}
	}
}

