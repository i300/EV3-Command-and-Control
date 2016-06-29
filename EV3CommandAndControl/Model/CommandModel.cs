using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EV3CommandAndControl
{
	public class CommandEventArgs : EventArgs
	{
		public readonly Command command;

		public CommandEventArgs(Command c)
		{
			this.command = c;
		}
	}

	public class CommandModel
	{
		private static CommandModel instance;

		Dictionary<int, Command> commands;

		public event EventHandler<CommandEventArgs> CommandAddedEvent;
		public event EventHandler<CommandEventArgs> CommandChangedEvent;
		public event EventHandler<CommandEventArgs> CommandRemovedEvent;
		
		public CommandModel()
		{
			commands = new Dictionary<int, Command>();
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

		public Command NewCommand()
		{
			int newID = 0;

			for (;;)
			{
				if (!commands.ContainsKey(++newID))
				{
					break;
				}
			}

			Command c = new Command(newID);

			commands.Add(newID, c);

			OnRaiseCommandAddedEvent(new CommandEventArgs(c));

			return c;
		}

		public void ChangeCommandName(int id, string name)
		{
			if (commands.ContainsKey(id))
			{
				Command c = commands[id];
				c.name = name;
				commands[id] = c;

				OnRaiseCommandChangedEvent(new CommandEventArgs(c));
			}
		}

		public void RemoveCommand(int id)
		{
			if (commands.ContainsKey(id))
			{
				Command c = commands[id];
				commands.Remove(id);

				OnRaiseCommandRemovedEvent(new CommandEventArgs(c));;
			}
		}

		protected virtual void OnRaiseCommandAddedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandAddedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseCommandChangedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandChangedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnRaiseCommandRemovedEvent(CommandEventArgs e)
		{
			EventHandler<CommandEventArgs> handler = CommandRemovedEvent;

			if (handler != null)
			{
				handler(this, e);
			}
		}

	}
}

