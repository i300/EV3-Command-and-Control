using System;
namespace EV3CommandAndControl
{
	public class ProgramCommand
	{
		public readonly Command command;
		public int parameter;
		public int index;

		public ProgramCommand(int index, Command command)
		{
			this.command = command;
			this.index = index;
		}
	}
}

