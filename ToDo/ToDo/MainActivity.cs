using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ToDoLibrary;
using System.Collections.Generic;

namespace ToDo
{
	[Activity (Label = "ToDo", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		Button LoginButton;
		Button SignUpButton;
		ProgressDialog progressDialog;
		String username ;
		String password;
		TextView userNameTextView;
		TextView passwordTextView;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
			InitialiseControls ();
		}

		private void InitialiseControls()
		{
			LoginButton = FindViewById<Button> (Resource.Id.loginButton);
			SignUpButton = FindViewById<Button> (Resource.Id.SignUpButton);
			LoginButton.Click += LoginButtonClicked;
			SignUpButton.Click += SignUpButtonClicked;
			userNameTextView = FindViewById<TextView> (Resource.Id.usernameText);
			passwordTextView = FindViewById<TextView> (Resource.Id.passwordText);
			progressDialog = new ProgressDialog (this);
			progressDialog.SetProgressStyle (ProgressDialogStyle.Spinner);
		}

		private void LoginButtonClicked(object sender, EventArgs e)
		{
			username = userNameTextView.Text;
			password = passwordTextView.Text;
			if (String.IsNullOrEmpty (username) == false && String.IsNullOrEmpty (password) == false) 
			{
				progressDialog.SetMessage ("Logging in....Please wait!!!!!");
				progressDialog.Show ();
				checkLogin ("login" , username, password);
			}
			else
			{
				Toast.MakeText (this, "Enter both Username and Password", ToastLength.Short).Show ();
			}
		}

		private void SignUpButtonClicked(object sender, EventArgs e)
		{
			username = userNameTextView.Text;
			password = passwordTextView.Text;
			if (String.IsNullOrEmpty (username) == false && String.IsNullOrEmpty (password) == false) 
			{
				progressDialog.SetMessage ("Signing Up....Please wait!!!!!");
				progressDialog.Show ();
				checkLogin ("sign" , username, password);
			}
			else
			{
				Toast.MakeText (this, "Enter both Username and Password", ToastLength.Short).Show();
			}
		}

		private async void checkLogin( String action , String username, String password)
		{
			ToDoParse parseHelper = new ToDoParse (this , null);
			ToDoParse.InitParse ();
			String msg = "";
			bool isSuccess = false;
			try 
			{
				int count = await parseHelper.getUserFromParse(username, password);
				if ( count == -10 )
				{
					msg = "No Internet Connection Available.";
				}
				else
				{
					if ( action == "login" )
					{
						if( count > 0 )
						{
							msg ="Login Successfull" ;
							isSuccess = true;
						}
						else 
						{
							msg = "Invalid user";
						}
					}
					else if ( action == "sign")
					{
						if( count > 0 )
							msg = "Username already Exists";
						else
						{
							int result = parseHelper.addToDOUser(username,password);
							if ( result == -10 )
							{
								msg = "No Internet Connection Available.";
							}
							else if( result > 0 )
							{
								msg ="Signing in Successfull";
								isSuccess = true;
							}
							else
							{
								msg ="Error while Signing In.";
							}
						}
					}
				}
			}
			catch(Exception e) 
			{
				Toast.MakeText (this, e.Message, ToastLength.Short).Show ();
			}
			finally
			{
				progressDialog.Dismiss ();
				Toast.MakeText (this, msg, ToastLength.Short).Show ();
				if (isSuccess == true) 
				{
					Intent toDoActivity = new Intent (this, typeof(ToDoActivity));
					toDoActivity.PutExtra("UserName",username);
					StartActivity (toDoActivity);
				}
			}
		}
	}
}


