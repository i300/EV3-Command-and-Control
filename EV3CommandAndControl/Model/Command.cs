using System;
namespace EV3CommandAndControl
{
	public class Command
	{
		protected int id;
		public string name;
		public int parameter;

		public Command(int id)
		{
			this.id = id;
		}

		public int ID
		{
			get
			{
				return id;
			}
		}
	}
}

