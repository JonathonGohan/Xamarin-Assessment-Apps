using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using EventLibrary;

namespace EventManager
{
	[Activity (Label = "EventManager", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		EditText username;
		EditText password;
		Button loginButton;
		Button signUpButton;

		LoginDataSource datasource;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			username = FindViewById<EditText> (Resource.Id.usernameText);
			password = FindViewById<EditText> (Resource.Id.passwordText);

			loginButton = FindViewById<Button> (Resource.Id.loginButton);
			loginButton.Click += LoignButtonClicked;

			signUpButton = FindViewById<Button> (Resource.Id.SignUpButton);
			signUpButton.Click += SignUpButtonClicked;

			datasource = new LoginDataSource (this);
			datasource.open ();
		}

		public void LoignButtonClicked(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty (username.Text) == false && String.IsNullOrEmpty (password.Text) == false) 
			{
				if (datasource.checkLoginDetails (username.Text, password.Text)) 
				{
					Intent eventIntent = new Intent (this, typeof(EventActivity));
					eventIntent.PutExtra ("LoggedInUserName", username.Text);
					StartActivity (eventIntent);
				}
				else 
				{
					Toast.MakeText (this, " Username / Password is incorrect ", ToastLength.Long).Show();
				}
			} 
			else 
			{
				Toast.MakeText (this, " Enter Username and Password ", ToastLength.Long).Show();
			}
		}

		public void SignUpButtonClicked(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty (username.Text) == false && String.IsNullOrEmpty (password.Text) == false) 
			{
				if (datasource.checkUserNameExists (username.Text) == false ) 
				{
					if (datasource.insertLoginDetails (username.Text, password.Text)) 
					{
						Intent eventIntent = new Intent (this, typeof(EventActivity));
						eventIntent.PutExtra ("LoggedInUserName", username.Text);
						StartActivity (eventIntent);
					}
					else 
					{
						Toast.MakeText (this, " Error while Create New user ", ToastLength.Long);
					}
				} 
				else 
				{
					Toast.MakeText (this, " Username already Exists ", ToastLength.Long).Show();
				}
			}
			else 
			{
				Toast.MakeText (this, " Enter Username and Password ", ToastLength.Long).Show();
			}
		}


		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			return true;
		}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
			int menuItemId = item.ItemId;
			switch (menuItemId) 
			{
				case Resource.Id.action_exit:
					Finish ();	
					break;
			}

            return true;
        }

	}
}


