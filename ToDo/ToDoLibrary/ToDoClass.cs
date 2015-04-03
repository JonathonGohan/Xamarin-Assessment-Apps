using System;

namespace ToDoLibrary
{
	public class ToDoClass
	{
		public int todoId{ get; set; }
		public String Username{ get; set; }
		public String Title{ get; set; }
		public String Description{ get; set; }
		public String DueDate{ get; set; }
		public String DueTime{ get; set; }
		public bool isSynced{ get; set; }
		public bool isCompleted = false;
		public String ObjectId{ get; set; }
	}
}

