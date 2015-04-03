using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestaurantLibrary;
using System.Threading.Tasks;

namespace RestaurantActivity
{
	[Activity (Label = "RegisterUser")]			
	public class RegisterUser : Activity
	{
		TextView _userNameTextView;
		TextView _emailTextView;
		TextView _phoneTextView;
		ProgressDialog _progress;
		TextView _passwordTextView;
		Button _registerButton;
		String UserName = "", Email ="", Password ="" , Phone ="";
		UserDetailsManager _userDetailsManager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Register);
			_userNameTextView = FindViewById <TextView>(Resource.Id.RegisterNameTextView);
			_emailTextView = FindViewById <TextView>(Resource.Id.RegisterMailTextView);
			_phoneTextView = FindViewById <TextView>(Resource.Id.RegisterPhoneTextView);
			_passwordTextView = FindViewById <TextView>(Resource.Id.RegisterPasswordTextView);
			_registerButton = FindViewById <Button>(Resource.Id.RegisterButton);
			_registerButton.Click += OnRegisterButtonClick; 
			_progress = new ProgressDialog (this);
			_progress.SetCancelable (false);
			_progress.SetProgressStyle (ProgressDialogStyle.Spinner);
			_progress.SetMessage (" Please wait !! ");
			_userDetailsManager = new UserDetailsManager (this);
		}

		public void OnRegisterButtonClick (object sender, EventArgs e)
		{

			UserName = _userNameTextView.Text;
			Email = _emailTextView.Text;
			Password = _passwordTextView.Text;
			Phone = _phoneTextView.Text;
			String msg = "";

			if (String.IsNullOrEmpty (UserName) == true)
				msg = "Enter the Username";
			else if (String.IsNullOrEmpty (Email) == true)
				msg = "Enter the Email Id";
			else if (String.IsNullOrEmpty (Password) == true)
				msg = "Enter the Password";
			else if (String.IsNullOrEmpty (Phone) == true)
				msg = "Enter the Phone number";
			else if (Android.Util.Patterns.EmailAddress.Matcher (Email).Matches () == false) 
				msg = "Enter a valid email address";

			if (msg == "") 
			{
				_progress.Show ();
				chkExistinguser ();
			}
			else
				Toast.MakeText (this, msg, ToastLength.Short).Show ();
		}

		public async void chkExistinguser()
		{
			int result = await _userDetailsManager.chkExistingUser (Email);
			_progress.Dismiss ();
			if (result == -10) 
			{
				Toast.MakeText (this, "No Internet connection", ToastLength.Short).Show ();
			}
			else if( result == 0)
			{
				UserDetails user = new UserDetails ();
				user.Username = UserName;
				user.EmailId = Email;
				user.Phone = Convert.ToInt32 (Phone);
				user.Password = Password;
				int resultAdd = _userDetailsManager.addUser (user);
				if( resultAdd == -10 )
					Toast.MakeText (this, "No Internet connection", ToastLength.Short).Show ();
				else
				{
					Toast.MakeText (this, "Registeration successfull", ToastLength.Short).Show ();
					Finish ();
					OverridePendingTransition (Resource.Animation.pushupin, Resource.Animation.pushupout);
				}
			}
			else
			{
				Toast.MakeText(this,"EmailId already registered",ToastLength.Short).Show();
			}
		}

		public override void OnBackPressed ()
		{
			Finish ();
			OverridePendingTransition (Resource.Animation.pushupin, Resource.Animation.pushupout);
		}
	}
}

