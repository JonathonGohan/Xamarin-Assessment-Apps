using System;

namespace RestaurantLibrary
{
	public sealed class UserDetailsDataSource
	{
		static readonly UserDetailsDataSource _userDataSourceInstance = new UserDetailsDataSource();
		UserDetails _userDetails;
		BookingDetails _currentBookingDetais;

		public static bool isUserValid = false;

		public static UserDetailsDataSource Instance
		{
			get
			{
				return _userDataSourceInstance;
			}
		}

		public BookingDetails BookingInfo
		{
			get
			{
				return _currentBookingDetais;
			}
		}

		public UserDetails UserInfo
		{
			get
			{
				return _userDetails;
			}

			private set
			{
				_userDetails = value;
			}
		}

		private UserDetailsDataSource ()
		{
			_userDetails = new UserDetails ();
			_currentBookingDetais = new BookingDetails ();
		}

		public void setUserDetails(UserDetails user)
		{
			_userDetails = user;
		}

		public void setBookingDetails(BookingDetails details)
		{
			_currentBookingDetais = details;
		}
	}
}

