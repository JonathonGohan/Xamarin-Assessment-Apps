using System;
using System.Collections.Generic;
using Android.Content;
using Parse;
using System.Threading.Tasks;
using Android.Net;
using Android.Widget;
using Android.App;
using System.Threading;
using Android.Util;

namespace ToDoLibrary
{
	public class ToDoManager
	{
		List<ToDoClass> toDos = new List<ToDoClass> ();
		public ToDoParse todoParse;
		Context myContext;
		NetworkInfo netInfo;

		public int Count {
			get{ return toDos.Count; }
		}

		private int _currentIndex;

		public ToDoClass Current 
		{
			get 
			{ 
				if (_currentIndex < 0 || _currentIndex >= Count)
					throw new IndexOutOfRangeException ("The position : " + _currentIndex + " is invalid");
				return toDos [_currentIndex]; 
			}
			private set 
			{
				Current = value;
			}
		}

		public ToDoManager (Context context)
		{
			myContext = context;
			todoParse = new ToDoParse (context, this);
		}

		public bool isOnline ()
		{
			var cm = (ConnectivityManager)myContext.GetSystemService (Context.ConnectivityService);
			netInfo = cm.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public async Task<IEnumerable<ParseObject>> getDataFromParse (String username)
		{
			toDos = new List<ToDoClass> ();
			ParseQuery<ParseObject> query = ParseObject.GetQuery("ToDoClass").WhereEqualTo("Username", username);
			IEnumerable<ParseObject> listParseObjs = await query.FindAsync ();
			foreach (ParseObject obj in listParseObjs) 
			{
				ToDoClass tdclass = new ToDoClass ();
				tdclass.ObjectId = obj.ObjectId ;
				tdclass.todoId = obj.Get<int> ("todoId");
				tdclass.Username = obj.Get<String> ("Username");
				tdclass.Title = obj.Get<String> ("Title");
				tdclass.Description = obj.Get<String> ("Description");
				tdclass.DueDate = obj.Get<String> ("DueDate");
				tdclass.isCompleted = obj.Get<bool> ("isCompleted");
				tdclass.DueTime = obj.Get<String> ("DueTime");
				toDos.Add (tdclass);
			}
			return null;
		}

		public void MoveTo (int position)
		{
			_currentIndex = position;
		}

		public int AddToDo (ToDoClass todo)
		{
			todo.todoId = toDos.Count;
			if (isOnline () == true) 
			{
				todoParse.addToDO (todo);
				toDos.Add (todo);
				return todo.todoId;
			}
			else
				return -10;
		}

		public void setNewToDoObjectId(String ObjectId)
		{
			toDos [toDos.Count - 1].ObjectId = ObjectId;
		}

		public int UpdateToDo (int position, ToDoClass todo)
		{
			if (isOnline ()) {
				ToDoClass oldToDo = toDos [position];

				todo.ObjectId = oldToDo.ObjectId;
				todo.todoId = oldToDo.todoId;
				toDos [position] = todo;
				todoParse.updateToDo (todo);
				return 1;
			} else
				return -10;
		}
	}
}

