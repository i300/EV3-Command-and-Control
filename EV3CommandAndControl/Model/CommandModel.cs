using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EV3CommandAndControl
{
	public class CommandModel
	{
		private static CommandModel instance;

		ObservableCollection<Command> commands;
		ObservableCollection<Command> program;

		int idCounter = 0;
		
		public CommandModel()
		{
			commands = new ObservableCollection<Command>();
			program = new ObservableCollection<Command>();
		}

		public void SetCommandsChangedHandler(System.Collections.Specialized.NotifyCollectionChangedEventHandler handler)
		{
			commands.CollectionChanged += handler;
		}

		public void SetProgramChangedHandler(System.Collections.Specialized.NotifyCollectionChangedEventHandler handler)
		{
			program.CollectionChanged += handler;
		}

		public static CommandModel Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CommandModel();
				}
				return instance;
			}
		}

		public int NewCommand()
		{
			Command c = new Command(idCounter++);
			commands.Add(c);

			return c.ID;
		}

		public void ChangeCommandName(int id, string name)
		{
			commands.
		}

		public void RemoveCommand(Command c)
		{
			commands.Remove(c);
			program.Remove(c);
		}

		public void AddCommandToProgram(Command c)
		{
			program.Add(c);
		}

		public void MoveCommandInProgram(int oldIndex, int newIndex)
		{
			program.Move(oldIndex, newIndex);
		}

		public ObservableCollection<Command> GetCommands()
		{
			return commands;
		}

		public ObservableCollection<Command> GetProgram()
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

