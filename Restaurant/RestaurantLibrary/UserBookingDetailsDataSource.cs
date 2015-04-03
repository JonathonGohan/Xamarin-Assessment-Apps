using System;
using System.Collections.Generic;

namespace RestaurantLibrary
{
	public class UserBookingDetailsDataSource
	{
		static readonly UserBookingDetailsDataSource _userBookingDataSourceInstance = new UserBookingDetailsDataSource();
		List<BookingDetails> _bookingDetails;
		BookingDetails _currentBookingDetails;
		int _currentSelectedBooking;

		public static UserBookingDetailsDataSource Instance
		{
			get
			{
				return _userBookingDataSourceInstance;
			}
		}

		private UserBookingDetailsDataSource ()
		{
			_bookingDetails = new List<BookingDetails> ();
			_currentBookingDetails = new BookingDetails();
		}

		public BookingDetails CurrentSelectedBookingInfo
		{
			get
			{
				return _bookingDetails [_currentSelectedBooking];
			}
		}

		public void MoveTo(int position)
		{
			_currentSelectedBooking = position;
		}

		public BookingDetails getBookingDetailsByPosition(int position)
		{
			return _bookingDetails [position];
		}

		public BookingDetails getBookingDetailsById(int bookingId)
		{
			return _bookingDetails.Find (e => e.Id == bookingId);
		}

		public void setBookingDetails(List<BookingDetails> details)
		{
			_bookingDetails = details;
		}

		public int getBookingsCount()
		{
			return _bookingDetails.Count;
		}
	}
}

