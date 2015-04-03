using System;
using Android.Widget;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using ToDoLibrary;
using Android.Util;
using System.Threading.Tasks;

namespace ToDo
{
	public class ToDoListViewAdapter : BaseExpandableListAdapter
	{
		readonly Activity myContext;

		public static int myExpandedGroup = -1;

		public ToDoManager todoManager;
		TextView inListTodoTitleTextView;
		TextView inListTodoDescTextView;
		TextView inListTodoDateTextView;
		Button SaveTodoButton;
		Button MarkCompletedButton;
		ExpandableListView expanListView;
		ImageView toDoStatusImage;
		ImageView dateButton;
		bool isNewChildLayout= false;
		bool isNewGroupExpanded= false;

		String Username;
		public ToDoActivity myToDoActivity;

		public ToDoListViewAdapter (Activity newContext, String name) : base ()
		{
			myToDoActivity = (ToDoActivity)newContext;
			todoManager = new ToDoManager (newContext);
			Username = name;
			mygetDataFromParse ();
			myContext = newContext;
		}

		public async void mygetDataFromParse()
		{
			await todoManager.getDataFromParse (Username);
			NotifyDataSetChanged ();
		}

		public void SaveData(bool Completed)
		{
			ToDoClass todo = new ToDoClass ();
			todo.Username = Username;
			todo.Title = inListTodoTitleTextView.Text;
			todo.Description = inListTodoDescTextView.Text;
			todo.DueDate = inListTodoDateTextView.Text;
			todo.DueTime = "";//todo.DueTime = inListTodoTimeTextView.Text;
			todo.isCompleted = Completed;
			todo.isSynced = true;
			int result = todoManager.UpdateToDo (myExpandedGroup, todo);
			if (result == -10) 
			{
				Toast.MakeText (myContext, "No Internet Connection Available", ToastLength.Short).Show ();
			}
			else if (result > 0) 
			{
				NotifyDataSetChanged ();
				Toast.MakeText (myContext, "ToDo Updated successfully", ToastLength.Short).Show ();
			}
			else 
			{
				Toast.MakeText (myContext, "Error while updating information", ToastLength.Short).Show ();
			}
		}

		public void SaveToDo (object sender, EventArgs e)
		{
			SaveData (false);
		}

		public void MarkCompletedButtonClicked(object sender, EventArgs e)
		{
			if (((Button)sender).Text == "Mark Done")
				SaveData (true);
			else
				SaveData (false);
		}

		public void DateImageButtonClicked(object sender, EventArgs e)
		{
			ToDoActivity.AlertDialogParent = "Edit";
			myContext.ShowDialog (1111);
		}

		public void AlertDateImageButtonClicked(String myDate)
		{
			inListTodoDateTextView.Text = myDate;
		}

		public void NewToDo (ToDoClass todo)
		{
			int result = todoManager.AddToDo (todo);
			if (result == -10) 
				Toast.MakeText (myContext, "No Internet Connection Available", ToastLength.Short).Show ();
			else if (result >= 0) 
				NotifyDataSetChanged ();
		}

		#region implemented abstract members of BaseExpandableListAdapter

		public override View GetGroupView (int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			expanListView = (ExpandableListView)parent;
			View header = convertView;
			if (header == null) {
				header = myContext.LayoutInflater.Inflate (Resource.Layout.ToDoExpandableListViewHeader, null);
			}
			Log.WriteLine (LogPriority.Info, " My Posistion Error ", " GetGroupView : groupPosition : " + groupPosition);
			todoManager.MoveTo (groupPosition);
			header.FindViewById<TextView> (Resource.Id.ToDoListHeader).Text = todoManager.Current.Title;

			toDoStatusImage = header.FindViewById<ImageView> (Resource.Id.IndicatorImage);

			if (todoManager.Current.isCompleted == false) 
			{
				String date = todoManager.Current.DueDate;
				String time = todoManager.Current.DueTime;
				if (String.IsNullOrEmpty (date)) 
				{
					toDoStatusImage.SetImageResource(Resource.Drawable.grey);
				}
				else 
				{
					DateTime dueDate = Convert.ToDateTime (date);
					if (dueDate > DateTime.Now)
						toDoStatusImage.SetImageResource (Resource.Drawable.orange);
					else if (dueDate.Date == DateTime.Now.Date)
						toDoStatusImage.SetImageResource (Resource.Drawable.green);
					else if (dueDate.Date < DateTime.Now.Date)
						toDoStatusImage.SetImageResource (Resource.Drawable.red);
				}
			}
			else 
			{
				toDoStatusImage.SetImageResource (Resource.Drawable.tick26black);
			}

			return header;
		}

		public override void OnGroupExpanded (int groupPosition)
		{
			base.OnGroupExpanded (groupPosition);  
			isNewGroupExpanded = false;
			if (myExpandedGroup != groupPosition) 
			{
				expanListView.CollapseGroup (myExpandedGroup);
				isNewGroupExpanded = true;
			}

			myExpandedGroup = groupPosition;
		}

		public override View GetChildView (int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			isNewChildLayout = false;
			View row = convertView;
			if (row == null) 
			{
				isNewChildLayout = true;
				row = myContext.LayoutInflater.Inflate (Resource.Layout.ToDoLayout, null);
			}

			//if (isNewGroupExpanded == true) 
			{
				inListTodoTitleTextView = row.FindViewById<TextView> (Resource.Id.ToDoTitleEditText);
				inListTodoDescTextView = row.FindViewById<TextView> (Resource.Id.ToDoDescEditText);
				inListTodoDateTextView = row.FindViewById<TextView> (Resource.Id.ToDoDureDateTextView);
				dateButton = row.FindViewById<ImageView> (Resource.Id.DateButton);

				SaveTodoButton = row.FindViewById<Button> (Resource.Id.CreateToDoButton);
				MarkCompletedButton = row.FindViewById<Button> (Resource.Id.MarkCompleted);
				SaveTodoButton.Text = "Save";
				todoManager.MoveTo (myExpandedGroup);
				inListTodoTitleTextView.Text = todoManager.Current.Title;
				inListTodoDescTextView.Text = todoManager.Current.Description;
				inListTodoDateTextView.Text = todoManager.Current.DueDate;
				isNewGroupExpanded = false;
			}

			if (isNewChildLayout == true) 
			{
				MarkCompletedButton.Click += MarkCompletedButtonClicked;
				SaveTodoButton.Click += SaveToDo;
				dateButton.Click += DateImageButtonClicked;
			}

			if (todoManager.Current.isCompleted == true)
				MarkCompletedButton.Text = "Mark ToDo";
			else
				MarkCompletedButton.Text = "Mark Done";
			return row;
		}

		public override Java.Lang.Object GetChild (int groupPosition, int childPosition)
		{
			return null;
		}

		public override long GetChildId (int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override Java.Lang.Object GetGroup (int groupPosition)
		{
			return null;
		}

		public override long GetGroupId (int groupPosition)
		{
			return groupPosition;
		}

		public override bool IsChildSelectable (int groupPosition, int childPosition)
		{
			return true;
		}

		public override int GetChildrenCount (int groupPosition)
		{
			return 1;
		}

		public override int GroupCount 
		{
			get 
			{
				return todoManager.Count;
			}
		}

		public override bool HasStableIds {
			get {
				return true;
			}
		}

		#endregion
	}
}