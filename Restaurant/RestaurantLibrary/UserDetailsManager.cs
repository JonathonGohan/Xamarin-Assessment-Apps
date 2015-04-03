using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;

namespace RestaurantLibrary
{
	public class UserDetailsManager
	{
		Context _context;
		NetworkInfo netInfo;
		ParseHelper _parseHelper;
		UserDetailsDataSource _userDetailsDataSource;

		public UserDetailsManager (Context ctx)
		{
			_context = ctx;
			_parseHelper = new ParseHelper (_context);
			_userDetailsDataSource = UserDetailsDataSource.Instance;
		}

		bool isOnline()
		{
			var manager = (ConnectivityManager) _context.GetSystemService (Context.ConnectivityService);
			netInfo = manager.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public async Task<int> chkExistingUser(String emailid)
		{
			if (isOnline ()) 
			{
				try 
				{
					UserDetailsDataSource.isUserValid = false;
					UserDetails user = new UserDetails ();
					var parseObject = await _parseHelper.chkExistingUser(emailid);
					user.Username = parseObject.Get<String>("Username");
					user.Phone = parseObject.Get<int>("Phone");
					user.EmailId = parseObject.Get<String>("EmailId");
					_userDetailsDataSource.setUserDetails(user);
					return 1;
				}
				catch 
				{
					return 0;
				}
			}

			return -10;
		}

		public int addUser(UserDetails user)
		{
			if (isOnline ()) 
			{
				_parseHelper.addNewUser (user);
				return 0;
			}
			return -10;
		}

		public async Task<int> getUserDetails(String emailid, String password)
		{
			UserDetailsDataSource.isUserValid = false;
			if (isOnline ()) 
			{
				UserDetails user = new UserDetails ();
				try
				{
					var parseUser = await _parseHelper.getUserDetails (emailid, password);
					user.Username = parseUser.Get<String> ("Username");
					user.EmailId = parseUser.Get<String> ("EmailId");
					user.Phone = parseUser.Get<int> ("Phone");
					_userDetailsDataSource.setUserDetails (user);
					UserDetailsDataSource.isUserValid = true;
					return 1;
				}
				catch ( Exception e )
				{
					return 0;
				}
			}
			else
				return -10;
		}

		public UserDetails getCurrentUser()
		{
			return _userDetailsDataSource.UserInfo;
		}

		public void setBookingDetails(BookingDetails bookingDetails)
		{
			_userDetailsDataSource.setBookingDetails (bookingDetails);
		}

	}
}

