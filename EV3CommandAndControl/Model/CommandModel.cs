using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EV3CommandAndControl
{
	public class CommandModel
	{
		LinkedList<Command> commands;
		ObservableCollection<int> program;
		
		public CommandModel()
		{
			commands = new LinkedList<Command>();
			program = new ObservableCollection<int>();
		}

		public void AddCommand(Command c)
		{
			commands.AddLast(c);
		}

		public void ChangeCommand(Command c, Command newC)
		{
			if (c.id == newC.id)
			{
				commands.AddAfter(commands.Find(c), newC);
				commands.Remove(c);
			}
		}

		public void AddCommandToProgram(Command c)
		{
			program.Add(c.id);
		}

		public void MoveCommandInProgram(int oldIndex, int newIndex)
		{
			program.Move(oldIndex, newIndex);
		}

		public LinkedList<Command> GetCommands()
		{
			return commands;
		}

		public ObservableCollection<int> GetProgram()
		{
			return program;
		}

		public string ExportJSON()
		{
			string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
			return json;
		}
	}
}

