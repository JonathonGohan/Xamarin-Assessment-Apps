using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RestaurantLibrary;
using System.Collections.Generic;

namespace RestaurantActivity
{
	[Activity (Label = "Book Restaurant", MainLauncher = true, Icon = "@drawable/icon")]
	public class LoginActivity : Activity
	{
		TextView _newUserLinkText;
		Button _loginButton;
		UserDetailsDataSource _userDataSource;
		ProgressDialog _progress;
		TextView _emailTextView;
		TextView _passwordTextView;
		String _email = "";
		String _password = "";
		TextView _registerTextView;
		UserDetailsManager _userDetailsManager;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);
			_newUserLinkText = FindViewById <TextView>(Resource.Id.NewUserLinkText);
			_newUserLinkText.SetText (Resource.String.newUser);

			_userDetailsManager = new UserDetailsManager (this);

			_userDataSource = UserDetailsDataSource.Instance;

			_progress = new ProgressDialog (this);
			_progress.SetCancelable (false);
			_progress.SetProgressStyle (ProgressDialogStyle.Spinner);
			_progress.SetMessage (" Please wait !! ");

			_emailTextView = FindViewById<TextView> (Resource.Id.EmaiEditText);
			_passwordTextView = FindViewById<TextView> (Resource.Id.PasswordEditText);

			_loginButton = FindViewById<Button> (Resource.Id.LoginButton);
			_loginButton.Click +=  chkLogin;

			_registerTextView = FindViewById<TextView> (Resource.Id.NewUserLinkText);
			_registerTextView.Click += OnRegisterButtonClicked;
		}

		public void OnRegisterButtonClicked(object sender, EventArgs e)
		{
			Intent registerIntent = new Intent (this, typeof(RegisterUser));
			StartActivity (registerIntent);
			OverridePendingTransition (Resource.Animation.pushdownin, Resource.Animation.pushdownout);
		}

		public void chkLogin(object sender, EventArgs e)
		{
			UserDetailsDataSource.isUserValid = false;
			validateLogin ();
		}

		public async void validateLogin()
		{
			_email = _emailTextView.Text;
			_password = _passwordTextView.Text;
			try
			{
				if (String.IsNullOrEmpty (_email) == false && String.IsNullOrEmpty (_password) == false) 
				{
					if ( Android.Util.Patterns.EmailAddress.Matcher(_email).Matches() == true )
					{
						_progress.Show ();
						int count = await _userDetailsManager.getUserDetails (_email, _password);
						if( count != -10 )
						{
								Toast.MakeText (this, "Login Successfull." , ToastLength.Short).Show ();
								Intent act2 = new Intent (this, typeof(HomeActivity));
								StartActivity (act2);
								OverridePendingTransition (Resource.Animation.leftin, Resource.Animation.leftout);
						}
						else if( count == 0)
							Toast.MakeText (this, "Invalid User." , ToastLength.Short).Show ();
						else
							Toast.MakeText (this, "No Internet connection" , ToastLength.Short).Show ();
					}
					else
					{
						Toast.MakeText (this, "Enter a valied email address." , ToastLength.Short).Show ();
					}
				}
				else 
				{
					Toast.MakeText (this, "Enter both Username and Password", ToastLength.Short).Show ();
				}
			}
			catch(Exception e) 
			{
				Toast.MakeText (this, e.Message, ToastLength.Short).Show ();
			}
			finally
			{
				_progress.Dismiss ();
			}
		}
	}
}


