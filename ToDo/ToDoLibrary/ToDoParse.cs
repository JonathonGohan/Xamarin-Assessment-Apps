using System;
using Parse;
using Android.Net;
using Android.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoLibrary
{
	public class ToDoParse
	{
		Context context;
		NetworkInfo netInfo;
		ToDoManager _todoManager;

		public ToDoParse (Context ctx, ToDoManager manager)
		{
			context = ctx;
			InitParse ();
			_todoManager = manager;
		}

		public static void InitParse()
		{
			ParseClient.Initialize ("2sTQ5tfHy9XsU5kYejna4jEaQsxbuilAAZ61MCEs", "rIY9wjYTQhVCCdCxReuq7INC04OH8RvHFRfDcOI5");
		}

		public async void addToDO (ToDoClass todo)
		{
			String newObjectId = "";
			ParseObject parseObject = new ParseObject ("ToDoClass");
			parseObject ["todoId"] = todo.todoId;
			parseObject ["Username"] = todo.Username;
			parseObject ["Title"] = todo.Title;
			parseObject ["Description"] = todo.Description;
			parseObject ["DueDate"] = todo.DueDate;
			parseObject ["isCompleted"] = todo.isCompleted;
			parseObject ["DueTime"] = todo.DueTime;
			await parseObject.SaveAsync ().ContinueWith (t => 
			{
					_todoManager.setNewToDoObjectId(parseObject.ObjectId);
			});
		}

		public async void updateToDo (ToDoClass todo)
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("ToDoClass");
			ParseObject todoToUpdate = await query.GetAsync(todo.ObjectId);
			todoToUpdate ["todoId"] = todo.todoId;
			todoToUpdate ["Username"] = todo.Username;
			todoToUpdate ["Title"] = todo.Title;
			todoToUpdate ["Description"] = todo.Description;
			todoToUpdate ["DueDate"] = todo.DueDate;
			todoToUpdate ["DueTime"] = todo.DueTime;
			todoToUpdate ["isSynced"] = todo.isSynced;
			todoToUpdate ["isCompleted"] = todo.isCompleted;
			todoToUpdate.SaveAsync ().Start ();
		}

		public int addToDOUser (String username, String password)
		{
			if (isOnline () == true) 
			{
				ParseObject parseObject = new ParseObject ("User");
				parseObject ["username"] = username;
				parseObject ["password"] = password;
				parseObject.SaveAsync ().Start ();
				return 1;
			}
			else 
			{
				return -10;
			}
		}

		public bool isOnline ()
		{
			var cm = (ConnectivityManager)context.GetSystemService (Context.ConnectivityService);
			netInfo = cm.ActiveNetworkInfo;
			return netInfo != null && netInfo.IsConnectedOrConnecting;
		}

		public async Task<int> getUserFromParse (String username, String password)
		{
			if ( isOnline()  == true) 
			{
				ToDoParse.InitParse ();
				ParseQuery<ParseObject> query = ParseObject.GetQuery ("User").WhereEqualTo ("username", username).WhereEqualTo ("password", password);
				IEnumerable<ParseObject> listParseObjs = await query.FindAsync ();
				int count = 0;
				foreach (ParseObject obj in listParseObjs) {
					count++;
				}
				return count;
			}
			return -10;
		}
	}
}

