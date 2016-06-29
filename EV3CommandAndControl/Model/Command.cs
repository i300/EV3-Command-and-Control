using System;
namespace EV3CommandAndControl
{
	public class Command
	{
		public readonly int id;
		public string name;

		public Command(int id)
		{
			this.id = id;
		}
	}
}

